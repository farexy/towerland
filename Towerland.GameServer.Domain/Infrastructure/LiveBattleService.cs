using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.State;
using Towerland.GameServer.Common.Logic;
using Towerland.GameServer.Common.Logic.ActionResolver;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Core.DataAccess;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Domain.Helpers;
using Towerland.GameServer.Domain.Interfaces;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Infrastructure
{
  public class LiveBattleService : ILiveBattleService, IBattleInitializationService
  {
    private static readonly ConcurrentDictionary<Guid, int> _battles;

    private readonly IBattleRepository _battleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProvider<LiveBattleModel> _provider;
    private readonly IStateChangeRecalculator _recalculator;
    private readonly IFieldFactory _fieldFactory;
    private readonly IStatsLibrary _statsLibrary;
    private readonly IMapper _mapper;

    static LiveBattleService()
    {
      _battles = new ConcurrentDictionary<Guid, int>();
    }
    
    public LiveBattleService(
      IBattleRepository repo,
      IUserRepository userRepo,
      IProvider<LiveBattleModel> provider, 
      IStateChangeRecalculator recalc, 
      IFieldFactory fieldFactory, 
      IStatsLibrary statsLibrary,
      IMapper mapper)
    {
      _battleRepository = repo;
      _userRepository = userRepo;
      _provider = provider;
      _recalculator = recalc;
      _fieldFactory = fieldFactory;
      _statsLibrary = statsLibrary;
      _mapper = mapper;
    }

    public Field GetField(Guid battleId)
    {
      if (!_battles.ContainsKey(battleId))
      {
        throw new ArgumentException("No such battle");
      }
      return _provider.Find(battleId).State;
    }

    public bool CheckChanged(Guid battleId, int version)
    {
      if (!_battles.ContainsKey(battleId))
      {
        throw new ArgumentException("No such battle");
      }
      return _battles[battleId] != version;
    }

    public FieldState GetFieldState(Guid battleId)
    {
      if (!_battles.ContainsKey(battleId))
      {
        throw new ArgumentException("No such battle");
      }
      return _provider.Find(battleId).State.State;
    }

    public LiveBattleModel GetActualBattleState(Guid battleId, out int revision)
    {
      if (!_battles.ContainsKey(battleId))
      {
        throw new ArgumentException("No such battle");
      }

      revision = _battles[battleId];
      return _provider.Find(battleId);
    }

    public Guid InitNewBattle(Guid monstersPlayer, Guid towersPlayer)
    {
      var id = Guid.NewGuid();
      while (!_battles.TryAdd(id, 0)) ;
      CreateBattleAsync(id, monstersPlayer, towersPlayer);
      return id;
    }

    public async Task RecalculateAsync(StateChangeCommand command, int curTick)
    {
      await Task.Run(async () =>
      {
        var fieldSerialized = _provider.Find(command.BattleId);
        var fieldState = fieldSerialized.State;
        var ticks = fieldSerialized.Ticks.Take(curTick);
        
        ResolveActions(fieldState, ticks);
    
        if (command.UnitCreationOptions != null)
        {
          foreach (var opt in command.UnitCreationOptions)
          {
            _recalculator.AddNewUnit(fieldState, opt.Type, _mapper.Map<CreationOptions>(opt));
          }
        }
        if (command.TowerCreationOptions != null)
        {
          foreach (var opt in command.TowerCreationOptions)
          {
            _recalculator.AddNewTower(fieldState, opt.Type, _mapper.Map<CreationOptions>(opt));
          }
        }
        if (command.Money != 0)
        {
          _recalculator.AddMoney(fieldState, command.Money);
        }
        
        var calc = new StateCalculator(_statsLibrary, fieldState);
        var newTicks = calc.CalculateActionsByTicks();
        fieldSerialized.Ticks = newTicks;
        _provider.Update(fieldSerialized);
        
        await IncrementBattleVersionAsync(command.BattleId);
      });
    }

    public async Task TryEndBattleAsync(Guid battleId, Guid userId)
    {
      await Task.Run(async () => 
      {
        var battle = _provider.Find(battleId);
        var entity = _battleRepository.Find(battleId);

        PlayerSide winSide;
        Guid? left = null;
        if (battle.State.StaticData.EndTimeUtc > DateTime.UtcNow)
        {
          winSide = entity.Monsters_UserId == userId ? PlayerSide.Towers : PlayerSide.Monsters;
          left = userId;
        }
        else
        {
          ResolveActions(battle.State, battle.Ticks);
          winSide = battle.State.State.Castle.Health > 0 ? PlayerSide.Towers : PlayerSide.Monsters;
        }
        entity.Winner = (int) winSide;
        entity.EndTime = DateTime.UtcNow;
        using (var ts = new TransactionScopeWrapper())
        {
          _battleRepository.Update(entity);
          _userRepository.IncrementExperience(entity.Monsters_UserId, CalcUserExp(entity, entity.Monsters_UserId, left));
          _userRepository.IncrementExperience(entity.Towers_UserId, CalcUserExp(entity, entity.Towers_UserId, left));

          ts.Complete();
        }

        battle.Ticks = CreateBattleEndTick(winSide);

        await IncrementBattleVersionAsync(battleId);
        _provider.Update(battle);      
      });
    }


    private async Task<bool> IncrementBattleVersionAsync(Guid battleId)
    {
      return await Task.Run(() =>
      {
        while (_battles.TryGetValue(battleId, out var curValue))
        {
          if (_battles.TryUpdate(battleId, curValue + 1, curValue))
            return true;
        }
        return false;
      });
    }

    private async void CreateBattleAsync(Guid battleId, Guid monstersPlayer, Guid towersPlayer)
    {
      await Task.Run(() =>
      {
        var newBattle = new LiveBattleModel
        {
          Id = battleId,
          State = (Field)_fieldFactory.ClassicField.Clone(),
          Ticks = Enumerable.Empty<GameTick>()
        };
        _provider.Create(newBattle);
        _battleRepository.Create(new Battle
        {
          Id = battleId,
          StartTime = DateTime.UtcNow,
          Monsters_UserId = monstersPlayer,
          Towers_UserId = towersPlayer
        });
      });
    }

    private static IEnumerable<GameTick> CreateBattleEndTick(PlayerSide winner)
    {
      return new[]
      {
        new GameTick
        {
          Actions = new []
          {
            new GameAction
            {
              ActionId = winner == PlayerSide.Monsters ? ActionId.MonsterPlayerWins : ActionId.TowerPlayerWins
            }
          }
        }
      };
    }

    private static int CalcUserExp(Battle b, Guid uid, Guid? left)
    {
      return b.IsWinner(uid)
        ? GameConstants.UserWonExp
        : left.HasValue && uid == left
          ? GameConstants.UserLeftExp
          : GameConstants.UserLoosedExp;
    }
      
    private static void ResolveActions(Field f, IEnumerable<GameTick> ticks)
    {
      if (ticks == null)
      {
        return;
      }
      var resolver = new FieldStateActionResolver(f);
      foreach (var tick in ticks)
      {
        if (tick.HasNoActions)
        {
          continue;
        }
        foreach (var action in tick.Actions)
        {
          resolver.Resolve(action);
        }
      }
    }
  }
}

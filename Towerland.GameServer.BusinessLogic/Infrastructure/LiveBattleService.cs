using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using ServiceStack.Logging;
using Towerland.GameServer.BusinessLogic.Helpers;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.BusinessLogic.Lockers;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Data.DataAccess;
using Towerland.GameServer.Data.Entities;
using Towerland.GameServer.Logic.ActionResolver;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.BusinessLogic.Infrastructure
{
  public class LiveBattleService : ILiveBattleService, IBattleInitializationService
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly ConcurrentDictionary<Guid, int> _battles;

    private readonly IBattleRepository _battleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProvider<LiveBattleModel> _provider;
    private readonly IStateChangeRecalculator _recalculator;
    private readonly IFieldFactory _fieldFactory;
    private readonly IStatsLibrary _statsLibrary;
    private readonly ICheatCommandManager _cheatCommandManager;
    private readonly IMapper _mapper;

    public LiveBattleService(
      IBattleRepository repo,
      IUserRepository userRepo,
      IProvider<LiveBattleModel> provider,
      IStateChangeRecalculator recalc,
      IFieldFactory fieldFactory,
      IStatsLibrary statsLibrary,
      ICheatCommandManager cheatCommandManager,
      IMapper mapper)
    {
      _battles = new ConcurrentDictionary<Guid, int>();
      _battleRepository = repo;
      _userRepository = userRepo;
      _provider = provider;
      _recalculator = recalc;
      _fieldFactory = fieldFactory;
      _statsLibrary = statsLibrary;
      _cheatCommandManager = cheatCommandManager;
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

    public LiveBattleModel GetActualBattleState(Guid battleId, out int revision)
    {
      if (!_battles.ContainsKey(battleId))
      {
        throw new ArgumentException("No such battle");
      }

      revision = _battles[battleId];
      return _provider.Find(battleId);
    }

    public async Task<Guid> InitNewBattleAsync(Guid monstersPlayer, Guid towersPlayer)
    {
      var id = Guid.NewGuid();
      while (!_battles.TryAdd(id, 0)) ;
      await CreateBattleAsync(id, monstersPlayer, towersPlayer);
      return id;
    }

    public async Task<Guid> InitNewMultiBattleAsync(Guid monstersPlayer, Guid towersPlayer)
    {
      var id = Guid.NewGuid();
      while (!_battles.TryAdd(id, 0)) ;
      await CreateMultiBattleAsync(id, monstersPlayer, towersPlayer);
      return id;
    }

    public PlayerSide AddToMultiBattle(Guid battleId, Guid player, out bool isAcceptNewPlayers)
    {
      var battle = _provider.Find(battleId);
      var mbInfo = battle.MultiBattleInfo;
      PlayerSide side;
//      if (mbInfo.TowerPlayers.Count > mbInfo.MonsterPlayers.Count)
//      {
//        mbInfo.MonsterPlayers.Add(player);
//        side = PlayerSide.Monsters;
//      }
//      else
//      {
//        mbInfo.TowerPlayers.Add(player);
//        side = PlayerSide.Towers;
//      }
      if (!player.IsComputerPlayer())
      {
        mbInfo.TowerPlayers.Add(player);
        side = PlayerSide.Towers;
      }
      else
      {
        mbInfo.MonsterPlayers.Add(player);
        side = PlayerSide.Monsters;
      }
      _battleRepository.UpdateMultiBattle(battleId, mbInfo).ContinueWith(t => t.Status);

      isAcceptNewPlayers = mbInfo.TowerPlayers.Count == MultiBattleInfo.MaxUserOnSide;
      return side;
    }

    public bool CheckMultiBattleAcceptNewPlayers(Guid battleId)
    {
      var battle = _provider.Find(battleId);
      return battle != null && battle.State.StaticData.EndTimeUtc - DateTime.UtcNow > TimeSpan.FromMinutes(3);
    }

    public async Task RecalculateAsync(StateChangeCommand command, int curTick)
    {
      using (new BattleLocker(command.BattleId))
      {
        var fieldSerialized = _provider.Find(command.BattleId);
        var fieldState = fieldSerialized.State;
        var ticks = fieldSerialized.Ticks.Take(curTick);

        await Task.Run(() => ResolveActions(fieldState, ticks, _statsLibrary));

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

        if (!string.IsNullOrWhiteSpace(command.CheatCommand))
        {
          _cheatCommandManager.ResolveCommand(command.CheatCommand, fieldState);
        }

        var calc = new StateCalculator(_statsLibrary, fieldState);
        fieldSerialized.Ticks = await Task.Run(() => calc.CalculateActionsByTicks());

        _provider.Update(fieldSerialized);

        IncrementBattleVersion(command.BattleId);
      }
    }

    public async Task TryEndBattleAsync(Guid battleId, Guid userId)
    {
      Guid? left = null;
      using (new BattleLocker(battleId))
      {
        var battle = _provider.Find(battleId);
        var entity = await _battleRepository.FindAsync(battleId);
        var expCalc = new UserExperienceCalculator();

        PlayerSide winSide;
        if (battle.State.StaticData.EndTimeUtc > DateTime.UtcNow)
        {
          if (battle.MultiBattleInfo != null)
          {
            var mbInfo = battle.MultiBattleInfo;
            mbInfo.TowerPlayers.Remove(userId);
            mbInfo.MonsterPlayers.Remove(userId);
            if (mbInfo.TowerPlayers.Count == 0)
            {
              winSide = PlayerSide.Monsters;
            }
            else if (mbInfo.MonsterPlayers.Count == 0)
            {
              winSide = PlayerSide.Monsters;
            }
            else
            {
              return;
            }
          }
          else
          {
            winSide = entity.Monsters_UserId == userId ? PlayerSide.Towers : PlayerSide.Monsters;
            left = userId;
          }
        }
        else
        {
          if (entity.WinnerId != Guid.Empty)
          {
            return;
          }
          ResolveActions(battle.State, battle.Ticks, _statsLibrary);
          winSide = battle.State.State.Castle.Health > 0 ? PlayerSide.Towers : PlayerSide.Monsters;
        }

        entity.WinnerId = winSide == PlayerSide.Monsters ? entity.Monsters_UserId : entity.Towers_UserId;
        entity.EndTime = DateTime.UtcNow;

        await Task.WhenAll(_battleRepository.UpdateAsync(entity),
          IncrementUserXp(entity.Monsters_UserId, expCalc.CalcUserExp(entity, entity.Monsters_UserId, left)),
          IncrementUserXp(entity.Towers_UserId, expCalc.CalcUserExp(entity, entity.Towers_UserId, left)));

        battle.Ticks = CreateBattleEndTick(winSide);

        IncrementBattleVersion(battleId);
        _provider.Update(battle);
        DisposeBattleAsync(battleId);
      }
    }


    private bool IncrementBattleVersion(Guid battleId)
    {
      if (!_battles.TryGetValue(battleId, out var curValue))
      {
        return false;
      }

      return _battles.TryUpdate(battleId, curValue + 1, curValue);
    }

    private async Task CreateBattleAsync(Guid battleId, Guid monstersPlayer, Guid towersPlayer)
    {
      var newBattle = new LiveBattleModel
      {
        Id = battleId,
        State = (Field) _fieldFactory.ClassicField.Clone(),
        Ticks = Enumerable.Empty<GameTick>()
      };
      _provider.Create(newBattle);
      await _battleRepository.CreateAsync(new Battle
      {
        Id = battleId,
        StartTime = DateTime.UtcNow,
        Monsters_UserId = monstersPlayer,
        Towers_UserId = towersPlayer
      });
    }

    private async Task CreateMultiBattleAsync(Guid battleId, Guid monstersPlayer, Guid towersPlayer)
    {
      var mbInfo = new MultiBattleInfo
      {
        MonsterPlayers = new List<Guid> {monstersPlayer},
        TowerPlayers = new List<Guid> {towersPlayer},
      };
      var newBattle = new LiveBattleModel
      {
        Id = battleId,
        State = (Field) _fieldFactory.ClassicField.Clone(),
        Ticks = Enumerable.Empty<GameTick>(),
        MultiBattleInfo = mbInfo
      };
      _provider.Create(newBattle);
      await _battleRepository.CreateAsync(new Battle
      {
        Id = battleId,
        StartTime = DateTime.UtcNow,
        Monsters_UserId = monstersPlayer,
        Towers_UserId = towersPlayer,
        MultiBattleInfo = mbInfo
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

    private async Task IncrementUserXp(Guid userId, int xp)
    {
      if (!userId.IsComputerPlayer())
      {
        await _userRepository.IncrementExperienceAsync(userId, xp);
      }
    }

    private async Task DisposeBattleAsync(Guid battleId)
    {
      await Task.Delay(20000);
      _provider.Delete(battleId);
      _battles.TryRemove(battleId, out _);
    }

    private static void ResolveActions(Field f, IEnumerable<GameTick> ticks, IStatsLibrary statsLibrary)
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
          try
          {
            resolver.Resolve(action);
          }
          catch (Exception e)
          {
            Logger.Error(e);
          }
        }
      }
    }
  }
}

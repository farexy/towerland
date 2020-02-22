using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
    private readonly IFieldStorage _fieldStorage;
    private readonly IStatsLibrary _statsLibrary;
    private readonly ICheatCommandManager _cheatCommandManager;
    private readonly IAnalyticsService _analyticsService;

    public LiveBattleService(
      IBattleRepository repo,
      IUserRepository userRepo,
      IProvider<LiveBattleModel> provider,
      IStateChangeRecalculator recalc,
      IFieldStorage fieldStorage,
      IStatsLibrary statsLibrary,
      ICheatCommandManager cheatCommandManager,
      IAnalyticsService analyticsService)
    {
      _battles = new ConcurrentDictionary<Guid, int>();
      _battleRepository = repo;
      _userRepository = userRepo;
      _provider = provider;
      _recalculator = recalc;
      _fieldStorage = fieldStorage;
      _statsLibrary = statsLibrary;
      _cheatCommandManager = cheatCommandManager;
      _analyticsService = analyticsService;
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

    public async Task<Guid> InitNewBattleAsync(Guid monstersPlayer, Guid towersPlayer, GameMode mode)
    {
      var id = Guid.NewGuid();
      while (!_battles.TryAdd(id, 0)) ;
      await CreateBattleAsync(id, monstersPlayer, towersPlayer, mode);
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
      _battleRepository.UpdateMultiBattleAsync(battleId, mbInfo).ContinueWith(t =>
      {
        if (t.Status is TaskStatus.Faulted)
        {
          Logger.Error("Error while updating multi battle", t.Exception);
        }
      });

      isAcceptNewPlayers = mbInfo.TowerPlayers.Count == MultiBattleInfo.MaxUserOnSide;
      return side;
    }

    public bool CheckMultiBattleAcceptNewPlayers(Guid battleId)
    {
      var battle = _provider.Find(battleId);
      return battle != null && battle.State.StaticData.EndTimeUtc - DateTime.UtcNow > TimeSpan.FromMinutes(3);
    }

    public async Task RecalculateAsync(StateChangeCommand command)
    {
      if (command.IsEmpty)
      {
        return;
      }
      var liveBattleModel = _provider.Find(command.BattleId);
      var fieldState = liveBattleModel.State;

      if (fieldState.StaticData.EndTimeUtc < DateTime.UtcNow)
      {
        await TryEndBattleAsync(command.BattleId, ComputerPlayer.Id);
        return;
      }

      using (new BattleLocker(command.BattleId))
      {
        ResolveActions(liveBattleModel);

        var stateChangeActions = new List<GameAction>();
        if (command.UnitCreationOptions != null)
        {
          foreach (var opt in command.UnitCreationOptions)
          {
            stateChangeActions.AddRange(_recalculator.AddNewUnit(fieldState, opt.Type, opt));
          }
        }

        if (command.TowerCreationOptions != null)
        {
          foreach (var opt in command.TowerCreationOptions)
          {
            stateChangeActions.AddRange(_recalculator.AddNewTower(fieldState, opt.Type, opt));
          }
        }

        if (!string.IsNullOrWhiteSpace(command.CheatCommand))
        {
          stateChangeActions.AddRange(_cheatCommandManager.ResolveCommand(command.CheatCommand, fieldState));
        }

        var calc = new StateCalculator(_statsLibrary, fieldState, stateChangeActions);
        liveBattleModel.Ticks = await Task.Run(() => calc.CalculateActionsByTicks());

        _provider.Update(liveBattleModel);

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
              winSide = PlayerSide.Towers;
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
          ResolveActions(battle);
          winSide = battle.State.State.Castle.Health > 0 ? PlayerSide.Towers : PlayerSide.Monsters;
        }

        entity.WinnerId = winSide == PlayerSide.Monsters ? entity.Monsters_UserId : entity.Towers_UserId;
        entity.EndTime = DateTime.UtcNow;

        await Task.WhenAll(
          _analyticsService.LogEndAsync(winSide, battle.State),
          _battleRepository.UpdateAsync(entity),
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

    private async Task CreateBattleAsync(Guid battleId, Guid monstersPlayer, Guid towersPlayer, GameMode mode)
    {
      var compPlayerSide = monstersPlayer.IsComputerPlayer() && towersPlayer.IsComputerPlayer()
        ? PlayerSide.Both
        : monstersPlayer.IsComputerPlayer()
          ? PlayerSide.Monsters
          : towersPlayer.IsComputerPlayer()
            ? PlayerSide.Towers
            : PlayerSide.Undefined;
      
      var field = _fieldStorage.Create(0);
      
      var newBattle = new LiveBattleModel
      {
        Id = battleId,
        State = field,
        Ticks = Enumerable.Empty<GameTick>(),
        Mode = mode,
        CompPlayerSide = compPlayerSide
      };
      _provider.Create(newBattle);
      await _battleRepository.CreateAsync(new Battle
      {
        Id = battleId,
        StartTime = DateTime.UtcNow,
        Monsters_UserId = monstersPlayer,
        Towers_UserId = towersPlayer,
        Mode = mode,
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
        State = _fieldStorage.Create(0),
        Ticks = Enumerable.Empty<GameTick>(),
        MultiBattleInfo = mbInfo,
        Mode = GameMode.MultiBattle
      };
      _provider.Create(newBattle);
      await _battleRepository.CreateAsync(new Battle
      {
        Id = battleId,
        StartTime = DateTime.UtcNow,
        Monsters_UserId = monstersPlayer,
        Towers_UserId = towersPlayer,
        MultiBattleInfo = mbInfo,
        Mode = GameMode.MultiBattle
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

    public void ResolveActions(LiveBattleModel battle)
    {
      if (battle.Ticks == null)
      {
        return;
      }

      var field = battle.State;
      var resolver = new FieldStateActionResolver(field);
      battle.TicksHistory = battle.TicksHistory ?? new List<GameTick>();

      foreach (var tick in battle.Ticks)
      {
        battle.TicksHistory.Add(tick);
        field.State.Units.ForEach(DecrementWaitTicks);
        field.State.Towers.ForEach(DecrementWaitTicks);

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

        if (tick.RelativeTime >= DateTime.UtcNow)
        {
          break;
        }
      }
    }

    private static void DecrementWaitTicks(GameObject gameObject)
    {
      if (gameObject.WaitTicks > 0)
      {
        gameObject.WaitTicks--;
      }

      if (gameObject.Effect.Duration > 0)
      {
        gameObject.Effect.Duration--;
      }
    }
  }
}

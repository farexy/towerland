using System;
using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Logic.Factories;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Logic.Calculators
{
    public class StateCalculator
    {
      private readonly BattleContext _battleContext;
      private readonly BehaviourFactory _behaviourFactory;
      private readonly MoneyCalculator _moneyCalculator;

      private List<List<GameAction>> Ticks => _battleContext.Ticks;
      public Field Field => _battleContext.Field;

      public StateCalculator(IStatsLibrary statsLibrary, Field fieldState, List<GameAction> stageChangingActions)
      {
        _battleContext = new BattleContext((Field)fieldState.Clone());
        _behaviourFactory = new BehaviourFactory(_battleContext, statsLibrary);
        _moneyCalculator = new MoneyCalculator(statsLibrary);
        stageChangingActions.ForEach(_battleContext.AddAction);
      }

      public GameTick[] GetIdleTicks()
      {
        for (int i = 0; i < 500; i++)
        {
          if (i % 5 == 0)
          {
            AddMoneyByTimer();
          }
        }
        return new GameTick[0];
      }

      public GameTick[] CalculateActionsByTicks()
      {
        NextTick();
        while (Field.State.Castle.Health > 0 && Field.State.Units.Any())
        {
          GetUnitActions();
          GetTowerActions();
          GetTickEndActions();
          AddMoneyByTimer();

          AddGameObjects();
          RemoveGameObjects();

          NextTick();
        }
        GetActionsAfterCalculation();

        var result = new GameTick[Ticks.Count];
        var time = DateTime.UtcNow + TimeSpan.FromSeconds(FieldStaticData.TickSecond);
        for (int i = 0; i < Ticks.Count; i++)
        {
          result[i] = new GameTick
          {
            RelativeTime = time,
            Actions = Ticks[i]
          };
          time += TimeSpan.FromSeconds(FieldStaticData.TickSecond);
        }
        return result;
      }

      private void NextTick()
      {
        Ticks.Add(_battleContext.CurrentTick.ToList());
        _battleContext.CurrentTick.Clear();
      }

      private void GetUnitActions()
      {
        foreach (var unit in Field.State.Units)
        {
          var behaviour = _behaviourFactory.CreateUnitBehaviour(unit);
          if (!behaviour.CanDoAction())
          {
            continue;
          }

          if (!behaviour.ApplyPreActionEffect())
          {
            continue;
          }

          behaviour.DoAction();
          behaviour.ApplyPostActionEffect();
        }
      }

      private void GetTowerActions()
      {
        foreach (var tower in Field.State.Towers)
        {
          if (_battleContext.TowersToRemove.Contains(tower.GameId))
          {
            continue; // tower may be destroyed before action
          }
          var behaviour = _behaviourFactory.CreateTowerBehaviour(tower);
          if (!behaviour.CanDoAction())
          {
            continue;
          }

          if (!behaviour.ApplyPreActionEffect())
          {
            continue;
          }

          behaviour.DoAction();
          behaviour.ApplyPostActionEffect();
        }
      }

      private void GetTickEndActions()
      {
        foreach (var unit in Field.State.Units)
        {
          _behaviourFactory.CreateUnitBehaviour(unit).TickEndAction();
        }
        foreach (var tower in Field.State.Towers)
        {
          _behaviourFactory.CreateTowerBehaviour(tower).TickEndAction();
        }
      }

      private void AddGameObjects()
      {
        _battleContext.CurrentTick.AddRange(_battleContext.UnitsToAdd.Select(unit =>
          new GameAction
          {
            ActionId = ActionId.UnitAppears, UnitId = unit.GameId, GoUnit = unit
          }));
        _battleContext.CurrentTick.AddRange(_battleContext.TowersToAdd.Select(tower =>
          new GameAction
          {
            ActionId = ActionId.TowerAppears, TowerId = tower.GameId, GoTower = tower
          }));

        Field.AddMany(_battleContext.UnitsToAdd);
        Field.AddMany(_battleContext.TowersToAdd);

        _battleContext.UnitsToAdd.Clear();
        _battleContext.TowersToAdd.Clear();
      }

      private void RemoveGameObjects()
      {
        _battleContext.CurrentTick.AddRange(_battleContext.UnitsToRemove.Select(unitId =>
          new GameAction
          {
            ActionId = ActionId.UnitDisappears, UnitId = unitId, Position = Field[unitId].Position
          }));
        _battleContext.CurrentTick.AddRange(_battleContext.TowersToRemove.Select(towerId =>
          new GameAction
          {
            ActionId = ActionId.TowerCollapses, TowerId = towerId, Position = Field[towerId].Position
          }));

        Field.RemoveMany(_battleContext.UnitsToRemove);
        Field.RemoveMany(_battleContext.TowersToRemove);

        _battleContext.UnitsToRemove.Clear();
        _battleContext.TowersToRemove.Clear();
      }

      private void AddMoneyByTimer()
      {
        if (Ticks.Count % 5 == 0)
        {
          var moneyAmount = _moneyCalculator.GetGuaranteedMoneyByTimer(Field);
          _battleContext.AddAction(new GameAction{ActionId = ActionId.MonsterPlayerReceivesMoney, Money = moneyAmount});
          _battleContext.AddAction(new GameAction{ActionId = ActionId.TowerPlayerReceivesMoney, Money = moneyAmount});
        }
      }

      private void GetActionsAfterCalculation()
      {
        if (Field.State.Castle.Health <= 0)
        {
          Ticks.Add(new List<GameAction>
          {
            new GameAction{ActionId = ActionId.MonsterPlayerWins}
          });
        }
      }
    }
}

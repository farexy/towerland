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

      public StateCalculator(IStatsLibrary statsLibrary, Field fieldState)
      {
        _battleContext = new BattleContext((Field)fieldState.Clone());
        _behaviourFactory = new BehaviourFactory(_battleContext, statsLibrary);
        _moneyCalculator = new MoneyCalculator(statsLibrary);
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
        while (Field.State.Castle.Health > 0
          && Field.State.Units.Any())
        {
          _battleContext.CurrentTick.Clear();
          _battleContext.UnitsToRemove.Clear();
          _battleContext.UnitsToAdd.Clear();
          _battleContext.TowersToRemove.Clear();
          _battleContext.TowersToAdd.Clear();

          GetUnitActions();
          GetTowerActions();
          GetTickEndActions();
          AddMoneyByTimer();

          Field.AddMany(_battleContext.UnitsToAdd);
          Field.AddMany(_battleContext.TowersToAdd);
          RemoveGameObjects();

          Ticks.Add(_battleContext.CurrentTick.ToList());
        }
        GetActionsAfterCalculation();

        var result = new GameTick[Ticks.Count];
        for (int i = 0; i < Ticks.Count; i++)
        {
          result[i] = new GameTick
          {
            RelativeTime = i,
            Actions = Ticks[i]
          };
        }
        return result;
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
      }

      private void AddMoneyByTimer()
      {
        if (Ticks.Count % 5 == 0)
        {
          var moneyAmount = _moneyCalculator.GetGuaranteedMoneyByTimer(Field);
          _battleContext.CurrentTick.Add(new GameAction{ActionId = ActionId.PlayersReceivesMoney, Money = moneyAmount});
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

using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Common.Logic.Factories;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Common.Logic.Calculators
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

      public void SetState(FieldState fieldState)
      {
        Field.SetState(fieldState);
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

          GetUnitActions();
          GetTowerActions();
          AddMoneyByTimer();

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
        _battleContext.UnitsToRemove.Clear();

        foreach (var unit in Field.State.Units.ToArray())
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

        Field.RemoveMany(_battleContext.UnitsToRemove);
      }

      private void GetTowerActions()
      {
        foreach (var tower in Field.State.Towers)
        {
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

      private void AddMoneyByTimer()
      {
        if (Ticks.Count % 5 == 0)
        {
          var moneyAmount = _moneyCalculator.GetGuaranteedMoneyByTimer(Field);
          _battleContext.CurrentTick.Add(new GameAction{ActionId = ActionId.PlayersRecievesMoney, Money = moneyAmount});
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

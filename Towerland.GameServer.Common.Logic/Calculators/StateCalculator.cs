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

      private List<List<GameAction>> Ticks => _battleContext.Ticks;

      public Field Field => _battleContext.Field;

      public StateCalculator(IStatsLibrary statsLibrary, Field fieldState)
      {
        _battleContext = new BattleContext((Field)fieldState.Clone());
        _behaviourFactory = new BehaviourFactory(_battleContext, statsLibrary);
      }

      public void SetState(FieldState fieldState)
      {
        Field.SetState(fieldState);
      }

      public GameTick[] CalculateActionsByTicks()
      {
        while (Field.State.Castle.Health > 0
          && Field.State.Units.Any())
        {
          _battleContext.CurrentTick.Clear();

          GetUnitActions();
          GetTowerActions();

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

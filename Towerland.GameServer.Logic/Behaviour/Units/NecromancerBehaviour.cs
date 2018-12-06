using System.Linq;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Factories;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Units
{
  public class NecromancerBehaviour : BaseUnitBehaviour
  {
    private const int AbilityWaitTicks = 1;

    public NecromancerBehaviour(Unit unit, BattleContext battleContext, IStatsLibrary statsLibrary) : base(unit, battleContext, statsLibrary)
    {
    }

    public override bool ApplyPreActionEffect()
    {
      base.ApplyPreActionEffect();
      var deadUnitAction = FindDeadUnitAction();

      if (!deadUnitAction.HasValue)
      {
        return true;
      }
      var action = deadUnitAction.Value;
      var uFactory = new UnitFactory(StatsLibrary);

      var possiblePath = Field.GetPossiblePathIds(action.Position).ToArray();
      var pathId = possiblePath[GameMath.Rand.Next(possiblePath.Length)];
      var newUnitId = Field.GenerateGameObjectId();
      var skeleton = uFactory.Create(GameObjectType.Unit_Skeleton,
        new CreationOptions {GameId = newUnitId, Position = action.Position, PathId = pathId});

      BattleContext.UnitsToAdd.Add(skeleton);
      Field.State.RevivedUnits.OldIds.Add(action.UnitId);
      Field.State.RevivedUnits.NewIds.Add(newUnitId);

      BattleContext.CurrentTick.Add(new GameAction
      {
        ActionId = ActionId.UnitRevives,
        UnitId = action.UnitId,
        Position = action.Position
      });
      BattleContext.CurrentTick.Add(new GameAction
      {
        ActionId = ActionId.UnitAppears,
        UnitId = newUnitId,
        Position = action.Position,
        GoUnit = (Unit) skeleton.Clone()
      });
      BattleContext.CurrentTick.Add(new GameAction
      {
        ActionId = ActionId.UnitAppliesEffect_DarkMagic,
        UnitId = Unit.GameId,
        WaitTicks = AbilityWaitTicks
      });
      Unit.WaitTicks += AbilityWaitTicks;

      return false;
    }

    private GameAction? FindDeadUnitAction()
    {
      var len = BattleContext.Ticks.Count;
      for (int i = len - 1; i >= 0 && i >= len - Stats.Speed; i--)
      {
        foreach (var action in BattleContext.Ticks[i])
        {
          if (action.ActionId == ActionId.TowerKills
              && !Field.State.RevivedUnits.OldIds.Contains(action.UnitId)
              && !Field.State.RevivedUnits.NewIds.Contains(action.UnitId))
          {
            return action;
          }
        }
      }

      return null;
    }
  }
}
using System;
using System.Linq;
using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Factories;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Behaviour.Units
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

      var lastTicks = BattleContext.Ticks.Skip(BattleContext.Ticks.Count - Stats.Speed).ToArray();

      bool DeadUnitPredicate(GameAction act) =>
        act.ActionId == ActionId.UnitDies && !BattleContext.RevivedUnits.OldIds.Contains(act.UnitId) && !BattleContext.RevivedUnits.NewIds.Contains(act.UnitId);
      var deadUnitAction = lastTicks
        .FirstOrDefault(t => t.Any(DeadUnitPredicate))
        ?.FirstOrDefault(DeadUnitPredicate);

      if (!deadUnitAction.HasValue || deadUnitAction.Value.ActionId == default)
      {
        return true;
      }
      var action = deadUnitAction.Value;
      var uFactory = new UnitFactory(StatsLibrary);

      var possiblePath = Field.GetPossiblePathIds(action.Position).ToArray();
      var pathId = possiblePath[GameMath.Rand.Next(possiblePath.Length)];
      var skeleton = uFactory.Create(GameObjectType.Unit_Skeleton,
        new CreationOptions {Position = action.Position, PathId = pathId});
      var newUnitId = Field.AddGameObject(skeleton);

      BattleContext.RevivedUnits.OldIds.Add(action.UnitId);
      BattleContext.RevivedUnits.NewIds.Add(newUnitId);

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
  }
}
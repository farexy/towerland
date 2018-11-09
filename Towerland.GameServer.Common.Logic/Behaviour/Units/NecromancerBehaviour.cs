using System.Collections.Generic;
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
    public NecromancerBehaviour(Unit unit, BattleContext battleContext, IStatsLibrary statsLibrary) : base(unit, battleContext, statsLibrary)
    {
    }

    public override void DoAction()
    {
      var diedUnitActions = default(IEnumerable<GameAction>);
      //var diedUnitActions = data.CalculatedTicks.LastOrDefault()?.Where(a => a.ActionId == ActionId.UnitDies);
      if (diedUnitActions == null || !diedUnitActions.Any())
      {
        base.DoAction();
        return;
      }

      var uFactory = new UnitFactory(StatsLibrary);
      foreach (var deadUnitAction in diedUnitActions)
      {
        var possiblePath = Field.GetPossiblePathIds(deadUnitAction.Position).ToArray();
        var pathId = possiblePath[GameMath.Rand.Next(possiblePath.Length)];
        var skeleton = uFactory.Create(GameObjectType.Unit_Skeleton,
          new CreationOptions {Position = deadUnitAction.Position, PathId = pathId});
        var newUnitId = Field.AddGameObject(skeleton);
        BattleContext.CurrentTick.Add(new GameAction
        {
          ActionId = ActionId.UnitAppears,
          UnitId = newUnitId,
          Position = deadUnitAction.Position,
          GoUnit = (Unit) skeleton.Clone()
        });
      }

      BattleContext.CurrentTick.Add(new GameAction
      {
        ActionId = ActionId.UnitAppliesEffect_DarkMagic,
        UnitId = Unit.GameId,
      });
    }
  }
}
using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Extensions;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Logic.SpecialAI;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Towers
{
  public class BurstTowerBehaviour : BaseTowerBehaviour
  {
    public BurstTowerBehaviour(Tower tower, BattleContext battleContext, IStatsLibrary statsLibrary) : base(tower, battleContext, statsLibrary)
    {
    }

    public override void DoAction()
    {
      var targetFinder = new TargetFinder(StatsLibrary);

      var targetPoint = targetFinder.FindBurstTarget(Field, Tower);
      if (targetPoint.HasValue && targetPoint.Value != Field.StaticData.Finish)
      {
        BattleContext.AddAction(new GameAction
        {
          ActionId = ActionId.TowerAttacksPosition,
          TowerId = Tower.GameId,
          Position = targetPoint.Value,
          Damage = Stats.Damage,
          WaitTicks = Stats.AttackSpeed
        });

        var points = new HashSet<Point>();
        foreach (var pathId in Field.GetPossiblePathIds(targetPoint.Value))
        {
          var path = Field.StaticData.Path[pathId];
          points.Add(Field.StaticData.Start == targetPoint ? Point.NotExisting : path.GetPrevious(targetPoint.Value));
          points.Add(targetPoint.Value);
          points.Add(path.GetNext(targetPoint.Value));
        }
        var units = Field.FindTargetsAt(points.ToArray()).Where(u => !BattleContext.UnitsToRemove.Contains(u.GameId));

        foreach (var unit in units)
        {
          DamageUnit(unit);
        }
      }
    }
  }
}
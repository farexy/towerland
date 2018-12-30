using System.Linq;
using Towerland.GameServer.Logic.Calculators;
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
      var gameCalculator = new GameCalculator(StatsLibrary);

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

        var path = Field.StaticData.Path[Field.GetPossiblePathIds(targetPoint.Value).FirstOrDefault()];
        var prev = Field.StaticData.Start == targetPoint ? Point.NotExisting : path.GetPrevious(targetPoint.Value);
        var next = path.GetNext(targetPoint.Value);
        var units = Field.FindTargetsAt(prev, targetPoint.Value, next).Where(u => !BattleContext.UnitsToRemove.Contains(u.GameId));
//                foreach (var point in _field.GetNeighbourPoints(targetPoint, 1, FieldObject.Road))
//                {
//                  units = units.Union(_field.FindUnitsAt(point));
//                }
        foreach (var unit in units)
        {
          var damage = gameCalculator.CalculateDamage(unit.Type, Stats);

          if (damage == 0)
            continue;

          ApplyEffectOnAttack(unit);

          BattleContext.AddAction(new GameAction
          {
            ActionId = ActionId.UnitReceivesDamage,
            UnitId = unit.GameId,
            Damage = damage
          });
          if (unit.Health - damage <= 0)
          {
            var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = Tower.GameId, UnitId = unit.GameId, Position = unit.Position};

            BattleContext.AddAction(killAction);

            var moneyCalc = new MoneyCalculator(StatsLibrary);
            var towerReward = moneyCalc.GetTowerReward(Field, killAction);
            var unitReward = moneyCalc.GetUnitReward(Field, killAction);

            BattleContext.AddAction(new GameAction {ActionId = ActionId.TowerPlayerReceivesMoney, Money = towerReward});
            BattleContext.AddAction(new GameAction {ActionId = ActionId.MonsterPlayerReceivesMoney, Money = unitReward});

            BattleContext.UnitsToRemove.Add(unit.GameId);
          }
        }
      }
    }
  }
}
using System.Linq;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Logic.SpecialAI;
using Towerland.GameServer.Models.GameActions;
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
      if (targetPoint.HasValue)
      {
        Tower.WaitTicks = Stats.AttackSpeed;

        BattleContext.CurrentTick.Add(new GameAction
        {
          ActionId = ActionId.TowerAttacksPosition,
          TowerId = Tower.GameId,
          Position = targetPoint.Value,
          Damage = Stats.Damage,
          WaitTicks = Stats.AttackSpeed
        });
        var units = Field.FindUnitsAt(targetPoint.Value).Where(u => BattleContext.UnitsToRemove.Contains(u.GameId));
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

          BattleContext.CurrentTick.Add(new GameAction
          {
            ActionId = ActionId.UnitRecievesDamage,
            UnitId = unit.GameId,
            Damage = damage
          });
          unit.Health -= damage;
          if (unit.Health <= 0)
          {
            var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = Tower.GameId, UnitId = unit.GameId, Position = unit.Position};

            BattleContext.CurrentTick.Add(killAction);

            var moneyCalc = new MoneyCalculator(StatsLibrary);
            var towerReward = moneyCalc.GetTowerReward(Field, killAction);
            var unitReward = moneyCalc.GetUnitReward(Field, killAction);

            Field.State.MonsterMoney += unitReward;
            Field.State.TowerMoney += towerReward;

            BattleContext.CurrentTick.Add(new GameAction {ActionId = ActionId.TowerPlayerRecievesMoney, Money = towerReward});
            BattleContext.CurrentTick.Add(new GameAction {ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});

            BattleContext.UnitsToRemove.Add(unit.GameId);
          }
        }
      }
    }
  }
}
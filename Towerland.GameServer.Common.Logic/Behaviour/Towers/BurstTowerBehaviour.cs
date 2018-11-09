using System.Collections.Generic;
using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Logic.SpecialAI;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Behaviour.Towers
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
        var units = Field.FindUnitsAt(targetPoint.Value);
//                foreach (var point in _field.GetNeighbourPoints(targetPoint, 1, FieldObject.Road))
//                {
//                  units = units.Union(_field.FindUnitsAt(point));
//                }
        var deadUnits = new List<int>();
        foreach (var unit in units)
        {
          var damage = gameCalculator.CalculateDamage(unit.Type, Stats);

          if (damage == 0)
            continue;

          BattleContext.CurrentTick.Add(new GameAction
          {
            ActionId = ActionId.UnitRecievesDamage,
            UnitId = unit.GameId,
            Damage = damage
          });
          unit.Health -= damage;
          if (unit.Health <= 0)
          {
            var dieAction = new GameAction {ActionId = ActionId.UnitDies, UnitId = unit.GameId, TowerId = Tower.GameId, Position = unit.Position};
            var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = Tower.GameId, UnitId = unit.GameId, Position = unit.Position};

            BattleContext.CurrentTick.Add(dieAction);
            BattleContext.CurrentTick.Add(killAction);

            var moneyCalc = new MoneyCalculator(StatsLibrary);
            var towerReward = moneyCalc.GetTowerReward(Field, dieAction);
            var unitReward = moneyCalc.GetUnitReward(Field, killAction);

            Field.State.MonsterMoney += unitReward;
            Field.State.TowerMoney += towerReward;

            BattleContext.CurrentTick.Add(new GameAction {ActionId = ActionId.TowerPlayerRecievesMoney, Money = towerReward});
            BattleContext.CurrentTick.Add(new GameAction {ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});

            deadUnits.Add(unit.GameId);
          }
        }

        Field.RemoveMany(deadUnits);
      }
    }
  }
}
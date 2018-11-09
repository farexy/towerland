using System.Linq;
using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Logic.SpecialAI;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Behaviour.Towers
{
  public class BaseTowerBehaviour : IBehaviour
  {
    protected readonly Tower Tower;
    protected readonly IStatsLibrary StatsLibrary;
    protected readonly BattleContext BattleContext;
    protected readonly TowerStats Stats;

    protected Field Field => BattleContext.Field;

    public BaseTowerBehaviour(Tower tower, BattleContext battleContext, IStatsLibrary statsLibrary)
    {
      Tower = tower;
      BattleContext = battleContext;
      StatsLibrary = statsLibrary;
      Stats = statsLibrary.GetTowerStats(tower.Type);
    }

    public bool CanDoAction()
    {
      if (Tower.WaitTicks != 0)
      {
        Tower.WaitTicks -= 1;
        return false;
      }

      return true;
    }

    public virtual bool ApplyPreActionEffect()
    {
      return true;
    }

    public virtual void DoAction()
    {
      var targetFinder = new TargetFinder(StatsLibrary);

      var targetId = targetFinder.FindTarget(Field, Tower);
      if (targetId.HasValue)
      {
        var unit = (Unit) Field[targetId.Value];
        var damage = CalculateDamage(unit);
        Tower.WaitTicks = Stats.AttackSpeed;

        BattleContext.CurrentTick.Add(new GameAction
        {
          ActionId = ActionId.TowerAttacks,
          TowerId = Tower.GameId,
          UnitId = targetId.Value,
          WaitTicks = Stats.AttackSpeed
        });
        BattleContext.CurrentTick.Add(new GameAction
        {
          ActionId = ActionId.UnitRecievesDamage,
          UnitId = targetId.Value,
          Damage = damage
        });

        ApplyEffectOnAttack(unit);
        unit.Health -= damage;
        if (unit.Health <= 0)
        {
          var unitTrue = Field.State.Units.First(u => u.GameId == targetId);
          var dieAction = new GameAction {ActionId = ActionId.UnitDies, UnitId = targetId.Value, TowerId = Tower.GameId, Position = unitTrue.Position};
          var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = Tower.GameId, UnitId = targetId.Value, Position = unitTrue.Position};

          BattleContext.CurrentTick.Add(dieAction);
          BattleContext.CurrentTick.Add(killAction);

          var moneyCalc = new MoneyCalculator(StatsLibrary);
          var towerReward = moneyCalc.GetTowerReward(Field, dieAction);
          var unitReward = moneyCalc.GetUnitReward(Field, killAction);

          Field.State.MonsterMoney += unitReward;
          Field.State.TowerMoney += towerReward;

          BattleContext.CurrentTick.Add(new GameAction{ActionId = ActionId.TowerPlayerRecievesMoney, Money = towerReward});
          BattleContext.CurrentTick.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});

          Field.RemoveGameObject(targetId.Value);
        }
      }
    }

    protected virtual int CalculateDamage(Unit unit)
    {
      var gameCalculator = new GameCalculator(StatsLibrary);
      return gameCalculator.CalculateDamage(unit.Type, Stats);
    }

    protected virtual void ApplyEffectOnAttack(Unit unit)
    {
    }

    public virtual void ApplyPostActionEffect()
    {
    }
  }
}
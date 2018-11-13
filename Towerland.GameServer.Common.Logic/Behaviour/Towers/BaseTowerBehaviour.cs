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

      var targetId = targetFinder.FindTarget(BattleContext, Tower);
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

        ApplyEffectOnAttack(unit);

        BattleContext.CurrentTick.Add(new GameAction
        {
          ActionId = ActionId.UnitRecievesDamage,
          UnitId = targetId.Value,
          Damage = damage
        });

        unit.Health -= damage;
        if (unit.Health <= 0)
        {
          var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = Tower.GameId, UnitId = targetId.Value, Position = unit.Position};
          BattleContext.CurrentTick.Add(killAction);

          var moneyCalc = new MoneyCalculator(StatsLibrary);
          var towerReward = moneyCalc.GetTowerReward(Field, killAction);
          var unitReward = moneyCalc.GetUnitReward(Field, killAction);

          Field.State.MonsterMoney += unitReward;
          Field.State.TowerMoney += towerReward;

          BattleContext.CurrentTick.Add(new GameAction{ActionId = ActionId.TowerPlayerRecievesMoney, Money = towerReward});
          BattleContext.CurrentTick.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});

          BattleContext.UnitsToRemove.Add(targetId.Value);
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

    public virtual void TickEndAction()
    {
    }
  }
}
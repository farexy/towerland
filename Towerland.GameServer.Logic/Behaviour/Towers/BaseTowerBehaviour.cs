using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Logic.Selectors;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Logic.Behaviour.Towers
{
  public class BaseTowerBehaviour : IBehaviour
  {
    protected readonly Tower Tower;
    protected readonly IStatsLibrary StatsLibrary;
    protected readonly BattleContext BattleContext;
    protected readonly TowerStats Stats;
    protected readonly PredictableRandom Random;

    protected Field Field => BattleContext.Field;

    public BaseTowerBehaviour(Tower tower, BattleContext battleContext, IStatsLibrary statsLibrary)
    {
      Tower = tower;
      BattleContext = battleContext;
      StatsLibrary = statsLibrary;
      Stats = statsLibrary.GetTowerStats(tower.Type);
      Random = new PredictableRandom(Field, tower);
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
      if (Tower.Effect.Id != EffectId.None)
      {
        Tower.Effect.Duration -= 1;
        if (Tower.Effect.Duration == 0)
        {
          BattleContext.AddAction(new GameAction {ActionId = ActionId.TowerEffectCanceled, TowerId = Tower.GameId});
        }
      }

      return true;
    }

    public virtual void DoAction()
    {
      var targetFinder = new TargetFinder(StatsLibrary);

      var targetId = targetFinder.FindTarget(BattleContext, Tower);
      if (targetId.HasValue)
      {
        var unit = (Unit) Field[targetId.Value];

        BattleContext.AddAction(new GameAction
        {
          ActionId = ActionId.TowerAttacks,
          TowerId = Tower.GameId,
          UnitId = targetId.Value,
          WaitTicks = Stats.AttackSpeed
        });

        DamageUnit(unit);
      }
    }

    protected virtual int CalculateDamage(Unit unit)
    {
      var gameCalculator = new GameCalculator(StatsLibrary);
      return gameCalculator.CalculateDamage(unit.Type, Stats);
    }

    protected virtual void DamageUnit(Unit unit)
    {
      var damage = CalculateDamage(unit);

      ApplyEffectOnAttack(unit);
      
      if (damage == 0)
        return;

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

        BattleContext.AddAction(new GameAction{ActionId = ActionId.TowerPlayerReceivesMoney, Money = towerReward});
        BattleContext.AddAction(new GameAction{ActionId = ActionId.MonsterPlayerReceivesMoney, Money = unitReward});

        BattleContext.UnitsToRemove.Add(unit.GameId);
      }
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
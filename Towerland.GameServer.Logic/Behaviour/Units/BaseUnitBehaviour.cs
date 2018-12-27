using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Logic.Behaviour.Units
{
  public class BaseUnitBehaviour : IBehaviour
  {
    protected readonly Unit Unit;
    protected readonly IStatsLibrary StatsLibrary;
    protected readonly BattleContext BattleContext;
    protected readonly UnitStats Stats;

    protected Field Field => BattleContext.Field;

    public BaseUnitBehaviour(Unit unit, BattleContext battleContext, IStatsLibrary statsLibrary)
    {
      Unit = unit;
      BattleContext = battleContext;
      StatsLibrary = statsLibrary;
      Stats = statsLibrary.GetUnitStats(unit.Type);
    }

    public bool CanDoAction()
    {
      if (Unit.WaitTicks != 0)
      {
        Unit.WaitTicks -= 1;
        return false;
      }

      return true;
    }

    public virtual bool ApplyPreActionEffect()
    {
      if (Unit.Effect.Id != EffectId.None)
      {
        Unit.Effect.Duration -= 1;
        if (Unit.Effect.Duration == 0)
        {
          Unit.Effect = SpecialEffect.Empty;
          BattleContext.CurrentTick.Add(new GameAction {ActionId = ActionId.UnitEffectCanceled, UnitId = Unit.GameId});
        }
      }

      return true;
    }

    public virtual void DoAction()
    {
      var path = Field.StaticData.Path[Unit.PathId.Value];
      if (path.End == Unit.Position)
      {
        AttackCastle();
      }
      else
      {
        Move(path);
      }
    }

    protected virtual void Move(Path path)
    {
      var nextPoint = path.GetNext(Unit.Position);
      var speed = CalculateSpeed();
      Unit.Position = nextPoint;
      Unit.WaitTicks = speed;
      BattleContext.CurrentTick.Add(new GameAction
      {
        ActionId = ActionId.UnitMoves,
        Position = nextPoint,
        UnitId = Unit.GameId,
        WaitTicks = speed
      });
    }

    protected virtual void AttackCastle()
    {
      var attackAction = new GameAction
      {
        ActionId = ActionId.UnitAttacksCastle,
        UnitId = Unit.GameId,
        Damage = Stats.Damage
      };
      BattleContext.CurrentTick.Add(attackAction);
      Field.State.Castle.Health -= Stats.Damage;
      BattleContext.UnitsToRemove.Add(Unit.GameId);

      var moneyCalc = new MoneyCalculator(StatsLibrary);
      var unitReward = moneyCalc.GetUnitReward(Field, attackAction);
      Field.State.MonsterMoney += unitReward;

      BattleContext.CurrentTick.Add(new GameAction{ActionId = ActionId.MonsterPlayerReceivesMoney, Money = unitReward});
    }

    protected virtual int CalculateSpeed()
    {
      return Unit.Effect.Id == EffectId.UnitFreezed
        ? (int)(Stats.Speed * Unit.Effect.EffectValue)
        : Stats.Speed;
    }

    public virtual void ApplyPostActionEffect()
    {
      if (Unit.Effect.Id == EffectId.UnitPoisoned)
      {
        var damage = Unit.Health * Unit.Effect.EffectValue;
        if (Unit.Health - damage <= 0)
        {
          damage = Unit.Health - 1;
        }

        Unit.Health -= (int)damage;
        BattleContext.CurrentTick.Add(new GameAction
        {
          ActionId = ActionId.UnitReceivesDamage, UnitId = Unit.GameId, Damage = (int)damage
        });
      }
    }

    public virtual void TickEndAction()
    {
      if (Unit.Health <= 0)
      {
        BattleContext.UnitsToRemove.Add(Unit.GameId);
      }
    }
  }
}
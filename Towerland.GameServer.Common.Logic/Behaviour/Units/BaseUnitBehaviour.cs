using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Behaviour.Units
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
      if (Unit.Health <= 0)
      {
        BattleContext.UnitsToRemove.Add(Unit.GameId);
        return false;
      }
      if (Unit.WaitTicks != 0)
      {
        Unit.WaitTicks -= 1;
        return false;
      }

      return true;
    }

    public virtual bool ApplyPreActionEffect()
    {
      if (Unit.Effect != null && Unit.Effect.Id != EffectId.None)
      {
        Unit.Effect.Duration -= 1;
        if (Unit.Effect.Duration == 0)
        {
          Unit.Effect = SpecialEffect.Empty;
          BattleContext.CurrentTick.Add(new GameAction {ActionId = ActionId.UnitEffectCanseled, UnitId = Unit.GameId});
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
      Unit.Position = nextPoint;
      Unit.WaitTicks = Stats.Speed;
      BattleContext.CurrentTick.Add(new GameAction
      {
        ActionId = ActionId.UnitMoves,
        Position = nextPoint,
        UnitId = Unit.GameId,
        WaitTicks = Stats.Speed
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

      BattleContext.CurrentTick.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});
    }

    public virtual void ApplyPostActionEffect()
    {
    }
  }
}
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.ActionResolver
{
  public class FieldStateActionResolver : BaseActionResolver
  {
    public FieldStateActionResolver(Field filed) : base(filed)
    {
    }

    protected override void ResolveReservedAction(GameAction action)
    {
    }

    protected override void ResolveUnitAction(GameAction action)
    {
      switch (action.ActionId)
      {
        case ActionId.UnitMoves:
          if (_field[action.UnitId] != null)
          {
            _field.MoveUnit(action.UnitId, action.Position, action.WaitTicks);
          }
          break;

        case ActionId.UnitAttacksCastle:
          _field.State.Castle.Health -= action.Damage;
          break;

        case ActionId.UnitReceivesDamage:
          ((Unit)_field[action.UnitId]).Health -= action.Damage;
          break;

        case ActionId.UnitGetsEffect:
          _field[action.UnitId].Effect = new SpecialEffect{Id = action.EffectId, Duration = action.WaitTicks, EffectValue = action.EffectValue};
          break;

        case ActionId.UnitEffectCanceled:
          _field[action.UnitId].Effect = SpecialEffect.Empty;
          break;

        case ActionId.UnitDisappears:
            _field.RemoveGameObject(action.UnitId);
          break;

        case ActionId.UnitAppears:
          _field.AddGameObject(action.GoUnit);
          _field.State.RevivedUnits.NewIds.Add(action.UnitId);
          break;

        case ActionId.UnitRevives:
          _field.State.RevivedUnits.OldIds.Add(action.UnitId);
          break;

        case ActionId.UnitAppliesSkill:
          _field[action.UnitId].WaitTicks += action.WaitTicks;
          break;
      }
    }

    protected override void ResolveTowerAction(GameAction action)
    {
      switch (action.ActionId)
      {
        case ActionId.TowerAttacks:
        case ActionId.TowerAttacksPosition:
          _field[action.TowerId].WaitTicks = action.WaitTicks;
          break;

        case ActionId.TowerGetsEffect:
          _field[action.TowerId].Effect = new SpecialEffect{Id = action.EffectId, Duration = action.WaitTicks, EffectValue = action.EffectValue};
          break;

        case ActionId.TowerEffectCanceled:
          _field[action.TowerId].Effect = SpecialEffect.Empty;
          break;

        case ActionId.TowerCollapses:
          _field.RemoveGameObject(action.TowerId);
          break;
      }
    }

    protected override void ResolveOtherAction(GameAction action)
    {
      switch (action.ActionId)
      {
        case ActionId.MonsterPlayerReceivesMoney:
          _field.State.MonsterMoney += action.Money;
          break;
        case ActionId.TowerPlayerReceivesMoney:
          _field.State.TowerMoney += action.Money;
          break;
        case ActionId.PlayersReceivesMoney:
          _field.State.TowerMoney += action.Money;
          _field.State.MonsterMoney += action.Money;
          break;
        case ActionId.TowerPlayerLosesMoney:
          _field.State.TowerMoney -= action.Money;
          break;
        case ActionId.MonsterPlayerLosesMoney:
          _field.State.MonsterMoney -= action.Money;
          break;
      }
    }
  }
}

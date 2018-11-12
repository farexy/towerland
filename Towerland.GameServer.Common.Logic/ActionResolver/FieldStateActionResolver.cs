using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Factories;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.ActionResolver
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
          _field.RemoveGameObject(action.UnitId);
          _field.State.Castle.Health -= action.Damage;
          break;

        case ActionId.UnitRecievesDamage:
          ((Unit)_field[action.UnitId]).Health -= action.Damage;
          break;

        case ActionId.UnitFreezes:
          _field[action.UnitId].Effect = new SpecialEffect{Duration = action.WaitTicks, Id = EffectId.UnitFreezed};
          break;

        case ActionId.UnitDies:
            _field.RemoveGameObject(action.UnitId);
          break;

        case ActionId.UnitEffectCanseled:
          _field[action.UnitId].Effect = SpecialEffect.Empty;
          break;

        case ActionId.UnitAppears:
          _field.AddGameObject(action.GoUnit);
          break;

        case ActionId.UnitAppliesEffect_DarkMagic:
          _field[action.UnitId].WaitTicks += action.WaitTicks;
          break;;
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
      }
    }

    protected override void ResolveOtherAction(GameAction action)
    {
      switch (action.ActionId)
      {
        case ActionId.MonsterPlayerRecievesMoney:
          _field.State.MonsterMoney += action.Money;
          break;
        case ActionId.TowerPlayerRecievesMoney:
          _field.State.TowerMoney += action.Money;
          break;
        case ActionId.PlayersRecievesMoney:
          _field.State.TowerMoney += action.Money;
          _field.State.MonsterMoney += action.Money;
          break;
      }
    }
  }
}

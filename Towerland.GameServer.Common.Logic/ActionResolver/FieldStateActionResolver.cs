using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic.SpecialAI;

namespace Towerland.GameServer.Common.Logic.ActionResolver
{
  public class FieldStateActionResolver : BaseActionResolver
  {
    public FieldStateActionResolver(Field filed) : base(filed)
    {
    }

    protected override void ResolveReservedAction(GameAction action)
    {
      throw new System.NotImplementedException();
    }

    protected override void ResolveUnitAction(GameAction action)
    {
      switch (action.ActionId)
      {
        case ActionId.UnitMoves:
          _field.MoveUnit(action.UnitId, action.Position, action.WaitTicks);
          break;

        case ActionId.UnitAttacksCastle:
          _field.RemoveGameObject(action.UnitId);
          _field.Castle.Health -= action.Damage;
          break;

        case ActionId.UnitRecievesDamage:
          ((Unit)_field[action.UnitId]).Health -= action.Damage;
          break;

        case ActionId.UnitDies:
          _field.RemoveGameObject(action.UnitId);
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

      }
    }

    protected override void ResolveOtherAction(GameAction action)
    {
      throw new System.NotImplementedException();
    }
  }
}

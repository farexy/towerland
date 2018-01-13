﻿using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
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
        case ActionId.UnitMovesFreezed:
          _field.MoveUnit(action.UnitId, action.Position, action.WaitTicks);
          break;

        case ActionId.UnitAttacksCastle:
          _field.RemoveGameObject(action.UnitId);
          _field.State.Castle.Health -= action.Damage;
          break;

        case ActionId.UnitRecievesDamage:
          ((Unit)_field[action.UnitId]).Health -= action.Damage;
          break;

        case ActionId.UnitFreezes:
          _field[action.UnitId].Effect = new SpecialEffect{Duration = action.WaitTicks, Effect = EffectId.UnitFreezed};
          break;
          
        case ActionId.UnitDies:
            _field.RemoveGameObject(action.UnitId);
          break;
          
        case ActionId.UnitEffectCanseled:
          _field[action.UnitId].Effect = SpecialEffect.Empty;
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
      switch (action.ActionId)
      {
        case ActionId.MonsterPlayerRecievesMoney:
          _field.State.MonsterMoney += action.Money;
          break;
        case ActionId.TowerPlayerRecievesMoney:
          _field.State.TowerMoney += action.Money;
          break;
      }
    }
  }
}

using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Common.Logic.ActionResolver
{
  public abstract class BaseActionResolver : IActionResolver
  {
    protected Field _field;

    protected BaseActionResolver(Field filed)
    {
      _field = filed;
    }

    public void Resolve(GameAction action)
    {
      if(action.ActionId > ActionId.Reserved && action.ActionId < ActionId.Tower)
        ResolveReservedAction(action);
      
      if(action.ActionId > ActionId.Tower && action.ActionId < ActionId.Unit)
        ResolveTowerAction(action);

      if(action.ActionId > ActionId.Unit && action.ActionId < ActionId.Other)
        ResolveUnitAction(action);

      if(action.ActionId > ActionId.Other)
        ResolveOtherAction(action);
    }

    protected abstract void ResolveReservedAction(GameAction action);
    protected abstract void ResolveUnitAction(GameAction action);
    protected abstract void ResolveTowerAction(GameAction action);
    protected abstract void ResolveOtherAction(GameAction action);
  }
}

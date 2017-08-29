using GameServer.Common.Models.GameActions;

namespace Towerland.GameServer.Common.Logic.ActionResolver
{
  public interface IActionResolver
  {
    void Resolve(GameAction action);
  }
}

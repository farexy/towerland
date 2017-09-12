using GameServer.Common.Models.GameActions;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IActionResolver
  {
    void Resolve(GameAction action);
  }
}

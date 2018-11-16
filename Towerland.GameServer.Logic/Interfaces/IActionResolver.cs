using Towerland.GameServer.Models.GameActions;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IActionResolver
  {
    void Resolve(GameAction action);
  }
}

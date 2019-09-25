using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface ITowerSelector
  {
    (GameObjectType type, Point position)? GetNewTower(Field field);
  }
}
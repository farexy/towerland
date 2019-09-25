using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IUnitSelector
  {
    (GameObjectType type, int pathId)? GetNewUnit(Field field);
  }
}
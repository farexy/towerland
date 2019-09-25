using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IPathChooser
  {
    int GetFastestPath(Path[] paths, Point position);
    int GetOptimalPath(Field field, GameObjectType unitType);
  }
}

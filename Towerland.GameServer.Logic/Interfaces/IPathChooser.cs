using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IPathChooser
  {
    int GetFastestPath(Path[] paths, Unit unit);
    int GetOptimalPath(Field field, Unit unit);
  }
}

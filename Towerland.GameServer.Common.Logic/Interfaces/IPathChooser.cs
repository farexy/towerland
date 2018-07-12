using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IPathChooser
  {
    int GetFastestPath(Path[] paths, Unit unit);
    int GetOptimalPath(Field field, Unit unit);
  }
}

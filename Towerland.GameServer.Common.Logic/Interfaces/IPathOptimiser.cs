using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IPathOptimiser
  {
    int GetFastestPath(Path[] paths, Unit unit);
    int GetOptimalPath(Path[] paths, Field field, Unit unit);
  }
}

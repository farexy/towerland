using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface ITargetFindingOptimizer
  {
    int GetOptimalTarget(Field field, Tower tower);
  }
}
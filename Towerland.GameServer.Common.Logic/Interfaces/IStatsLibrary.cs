using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IStatsLibrary
  {
    UnitStats GetUnitStats(GameObjectType type);
    TowerStats GetTowerStats(GameObjectType type);
  }
}

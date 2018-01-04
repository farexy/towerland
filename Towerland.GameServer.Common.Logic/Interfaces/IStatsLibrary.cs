using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IStatsLibrary
  {
    UnitStats GetUnitStats(GameObjectType type);
    TowerStats GetTowerStats(GameObjectType type);
    double GetDefenceCoeff(UnitStats.DefenceType defType, TowerStats.AttackType attackType);
  }
}

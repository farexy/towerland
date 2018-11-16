using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IStatsLibrary
  {
    UnitStats GetUnitStats(GameObjectType type);
    TowerStats GetTowerStats(GameObjectType type);
    double GetDefenceCoeff(UnitStats.DefenceType defType, TowerStats.AttackType attackType);
  }
}

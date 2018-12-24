using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IStatsLibrary
  {
    IStats GetStats(GameObjectType type);
    UnitStats GetUnitStats(GameObjectType type);
    TowerStats GetTowerStats(GameObjectType type);
    double GetDefenceCoeff(UnitStats.DefenceType defType, TowerStats.AttackType attackType);
    Skill GetSkill(SkillId id, GameObjectType goType);
  }
}

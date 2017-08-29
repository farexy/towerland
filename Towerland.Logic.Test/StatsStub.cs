using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.Stats;
using Towerland.GameServer.Common.Logic;

namespace Towerland.Logic.Test
{
  class StatsStub : IStatsLibrary
  {
    public UnitStats GetUnitStats(GameObjectType type)
    {
      return new UnitStats
      {
        UnitType = type,
        Damage = 20,
        Health = 100,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Random,
        Speed = 4
      };
    }

    public TowerStats GetTowerStats(GameObjectType type)
    {
      return new TowerStats
      {
        Type = type,
        Attack = TowerStats.AttackType.Usual,
        AttackSpeed = 8,
        Damage = 10,
        Range = 5,
      };
    }
  }
}

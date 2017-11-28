using System.Collections.Generic;
using System.Linq;
using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.Stats;
using Towerland.GameServer.Common.Logic;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.Logic.Test
{
  class StatsLibrary : IStatsLibrary
  {
    private readonly Dictionary<GameObjectType, IStats> _objects;
    
    public StatsLibrary()
    {
      var factory = new StatsFactory();
      _objects = factory.Towers
        .Cast<IStats>()
        .Union(factory.Units.Cast<IStats>())
        .ToDictionary(el => el.Type, el => el);

    }
    
    public UnitStats GetUnitStats(GameObjectType type)
    {
      return (UnitStats) _objects[type];
    }

    public TowerStats GetTowerStats(GameObjectType type)
    {
      return (TowerStats) _objects[type];
    }
  }
}

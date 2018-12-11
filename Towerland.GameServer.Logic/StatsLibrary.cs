using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Logic
{
  public class StatsLibrary : IStatsLibrary
  {
    private readonly Dictionary<GameObjectType, IStats> _objects;
    private readonly IEnumerable<DefenceCoeff> _deffCoeffs;

    public StatsLibrary(IStatsProvider statsProvider)
    {
      _objects = statsProvider.GetTowerStats()
        .Cast<IStats>()
        .Union(statsProvider.GetUnitStats().Cast<IStats>())
        .ToDictionary(el => el.Type, el => el);
      _deffCoeffs = statsProvider.GetDefenceCoeffs();
    }

    public IStats GetStats(GameObjectType type)
    {
      return _objects[type];
    }

    public UnitStats GetUnitStats(GameObjectType type)
    {
      return (UnitStats) _objects[type];
    }

    public TowerStats GetTowerStats(GameObjectType type)
    {
      return (TowerStats) _objects[type];
    }

    public double GetDefenceCoeff(UnitStats.DefenceType defType, TowerStats.AttackType attackType)
    {
      return _deffCoeffs
        .First(defCoeff => defCoeff.Attack == attackType && defCoeff.Defence == defType)
        .Coeff;
    }
  }
}

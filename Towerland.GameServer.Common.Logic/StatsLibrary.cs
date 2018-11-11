using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Common.Logic.Factories;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic
{
  public class StatsLibrary : IStatsLibrary
  {
    private readonly Dictionary<GameObjectType, TowerStats> _towers;
    private readonly Dictionary<GameObjectType, UnitStats> _units;
    private readonly IEnumerable<DefenceCoeff> _deffCoeffs;

    public StatsLibrary(IStatsProvider provider)
    {
      _towers = provider.GetTowerStats().ToDictionary(el => el.Type);
      _units = provider.GetUnitStats().ToDictionary(el => el.Type);
      _deffCoeffs = provider.GetDefenceCoeffs();
    }

    public UnitStats GetUnitStats(GameObjectType type)
    {
      return _units[type];
    }

    public TowerStats GetTowerStats(GameObjectType type)
    {
      return _towers[type];
    }

    public double GetDefenceCoeff(UnitStats.DefenceType defType, TowerStats.AttackType attackType)
    {
      return _deffCoeffs
        .First(defCoeff => defCoeff.Attack == attackType && defCoeff.Defence == defType)
        .Coeff;
    }
  }
}

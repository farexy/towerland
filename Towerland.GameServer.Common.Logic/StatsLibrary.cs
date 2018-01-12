﻿using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Common.Logic.Factories;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic
{
  public class StatsLibrary : IStatsLibrary
  {
    private readonly Dictionary<GameObjectType, IStats> _objects;
    private readonly IEnumerable<DefenceCoeff> _deffCoeffs;
      
    public StatsLibrary()
    {
      var factory = new StatsFactory();
      _objects = factory.Towers
        .Cast<IStats>()
        .Union(factory.Units.Cast<IStats>())
        .ToDictionary(el => el.Type, el => el);
      _deffCoeffs = factory.DefenceCoeffs;
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

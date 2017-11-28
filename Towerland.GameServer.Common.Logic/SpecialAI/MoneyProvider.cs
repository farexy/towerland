using System;
using System.Linq;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.Stats;
using Towerland.GameServer.Common.Logic.ActionResolver;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic.SpecialAI
{
  public class MoneyProvider
  {
    private const double HealthCoeff = 0.5;
    private const double DamageCoeff = 0.2;
    private const double SpeedCoeff = 0.3;
    
    private readonly IStatsLibrary _statsLibrary;
    private readonly Field _field;
    
    public MoneyProvider(Field field, IStatsLibrary stats)
    {
      _statsLibrary = stats;
      _field = field;
    }

    public int GetTowerReward(GameAction action)
    {
      if (action.ActionId == ActionId.TowerKills)
      {
        var towerStats = _statsLibrary.GetTowerStats(_field[action.TowerId].Type);
        var unitStats = _statsLibrary.GetUnitStats(_field[action.UnitId].Type);
        return GetTowerReward(towerStats, unitStats);
      }

      return 0;
    }

    public int GetUnitReward(GameAction action)
    {
      if (action.ActionId == ActionId.TowerKills)
      {
        var unit = (Unit)_field[action.UnitId];
        var unitStats = _statsLibrary.GetUnitStats(unit.Type);
        var path = unit.PathId.HasValue 
          ? _field.StaticData.Path[unit.PathId.Value] 
          : new Path(Enumerable.Empty<Point>());
        
        return GetUnitReward(path, action.Position, unitStats);
      }

      return 0;
    }

    private static int GetTowerReward(TowerStats tower, UnitStats unit)
    {
      return (int)Math.Round(unit.Health / HealthCoeff + unit.Speed / SpeedCoeff + unit.Damage / DamageCoeff);
    }
    
    private static int GetUnitReward(Path p, Point f, UnitStats u)
    {
      double pathPercent = (double) p.PointOnThePathPosition(f) / p.Length;
      return (int)pathPercent * 100;
    }
  }
}
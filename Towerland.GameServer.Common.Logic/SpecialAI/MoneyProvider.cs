using System;
using System.Linq;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.Stats;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic.SpecialAI
{
  public class MoneyProvider
  {
    private const double HealthCoeff = 0.5;
    private const double DamageCoeff = 0.2;
    private const double SpeedCoeff = 0.3;
    
    private readonly IStatsLibrary _statsLibrary;
    
    public MoneyProvider(IStatsLibrary stats)
    {
      _statsLibrary = stats;
    }

    public int GetTowerReward(Field field, GameAction action)
    {
      if (action.ActionId == ActionId.UnitDies)
      {
        var towerStats = _statsLibrary.GetTowerStats(field[action.TowerId].Type);
        var unitStats = _statsLibrary.GetUnitStats(field[action.UnitId].Type);
        return GetTowerReward(towerStats, unitStats);
      }

      return 0;
    }

    public int GetUnitReward(Field field, GameAction action)
    {
      if (action.ActionId == ActionId.TowerKills)
      {
        var unit = (Unit)field[action.UnitId];
        var unitStats = _statsLibrary.GetUnitStats(unit.Type);
        var path = unit.PathId.HasValue 
          ? field.StaticData.Path[unit.PathId.Value] 
          : new Path(new Point[0]);
        
        return GetUnitReward(path, action.Position, unitStats);
      }
      if (action.ActionId == ActionId.UnitAttacksCastle)
      {
        var unit = (Unit)field[action.UnitId];
        var unitStats = _statsLibrary.GetUnitStats(unit.Type);
        var path = unit.PathId.HasValue 
          ? field.StaticData.Path[unit.PathId.Value] 
          : new Path(new Point[0]);
        
        return GetUnitReward(path, path.End, unitStats);
      }

      return 0;
    }

    private static int GetTowerReward(TowerStats tower, UnitStats unit)
    {
      return (int)Math.Round(unit.Health / HealthCoeff + unit.Speed / SpeedCoeff + unit.Damage / DamageCoeff) / 2;
    }
    
    private static int GetUnitReward(Path p, Point f, UnitStats u)
    {
      double pathPercent = (double) p.PointOnThePathPosition(f) / p.Length;
      return (int)(pathPercent * 200);
    }
  }
}
using System;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Logic.Calculators
{
  public class MoneyCalculator
  {
    private const int GuaranteedMoneyBase = 10;
    private const int TowerCoeff = 2;
    private const double HealthCoeff = 0.5;
    private const double DamageCoeff = 0.7;
    private const double SpeedCoeff = 0.8;

    private readonly IStatsLibrary _statsLibrary;

    public MoneyCalculator(IStatsLibrary stats)
    {
      _statsLibrary = stats;
    }

    public int GetTowerReward(Field field, GameAction action)
    {
      if (action.ActionId == ActionId.TowerKills)
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

    public int GetGuaranteedMoneyByTimer(Field field)
    {
      return GuaranteedMoneyBase + field.State.Towers.Count * TowerCoeff;
    }

    private static int GetTowerReward(TowerStats tower, UnitStats unit)
    {
      return (int)Math.Round(unit.Health / HealthCoeff + unit.Speed / SpeedCoeff + unit.Damage / DamageCoeff) / 3;
    }

    private static int GetUnitReward(Path p, Point f, UnitStats u)
    {
      double pathPercent = (double) p.PointOnThePathPosition(f) / p.Length;
      return (int)(pathPercent * 200);
    }
  }
}
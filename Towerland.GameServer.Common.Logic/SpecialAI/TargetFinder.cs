using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.SpecialAI
{
  public class TargetFinder
  {
    private readonly IStatsLibrary _statsLibrary;
    private readonly AdditiveConvolutionCalculator _additiveConvolutionCalculator;
    private readonly GameCalculator _gameCalculator;

    private static readonly double[] OptimalTargetCriteriaWeights =
    {
      0.6, // first criteria - chance to optimally kill unit (the more resulted negative health of unit close to 0, the better, if health is positive - ignore)
      0.2, // second criteria - ??
      0.2, // third criteria - optimality of units defence type for tower (the more result damage close to stats tower damage, the better)
    };

    public TargetFinder(IStatsLibrary statsLibrary)
    {
      _additiveConvolutionCalculator = new AdditiveConvolutionCalculator(OptimalTargetCriteriaWeights);
      _statsLibrary = statsLibrary;
      _gameCalculator = new GameCalculator(statsLibrary);
    }

    public int? FindTarget(Field field, GameObject tower)
    {
      var x = tower.Position.X;
      var y = tower.Position.Y;
      var stats = _statsLibrary.GetTowerStats(tower.Type);

      var units = new List<Unit>();
      for (int range = 1; range <= stats.Range; range++)
      {
        var p = new Point(x, y + range);
        units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

        p = new Point(x, y - range);
        units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

        p = new Point(x + range, y - range);
        units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

        p = new Point(x + range, y);
        units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

        p = new Point(x + range, y + range);
        units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

        p = new Point(x - range, y + range);
        units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

        p = new Point(x - range, y);
        units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

        p = new Point(x - range, y - range);
        units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

        if (units.Any())
        {
          return units[GameMath.Rand.Next(units.Count)].GameId;
        }
        units.Clear();
      }

      return default;
    }

    public Point? FindBurstTarget(Field field, GameObject tower)
    {
      return FindPointWithManyTargets(field, tower.Position, _statsLibrary.GetTowerStats(tower.Type).Range);
    }

    public int GetOptimalTarget(Field field, Tower tower)
    {
      var possibleTargets = field.FindePossibleTargetsForTower(tower, _statsLibrary);
      var tableToAnalize = new double[possibleTargets.Length, _additiveConvolutionCalculator.NumberOfCriterias];
      var towerStats = _statsLibrary.GetTowerStats(tower.Type);

      for (int i = 0; i < possibleTargets.Length; i++)
      {
        var unit = (Unit)field[possibleTargets[i]];

        tableToAnalize[i, 0] = 1 - GetKilledUnitHealthDeltaWithZero(towerStats, unit);
        tableToAnalize[i, 1] = 0;
        tableToAnalize[i, 2] = _gameCalculator.CalculateDamage(unit.Type, towerStats) / (double)towerStats.Damage;
      }
      return possibleTargets[_additiveConvolutionCalculator.FindOptimalVariantIndex(tableToAnalize)];
    }

    private static Point? FindPointWithManyTargets(Field field, Point center, int radius)
    {
      var x = center.X;
      var y = center.Y;

      var maxPoint = default(Point?);
      var maxCount = 0;

      for (int rng = 1; rng <= radius; rng++)
      {
        var p = new Point(x, y + rng);
        UpdMaxUnitsPoint(field, p, ref maxPoint, ref maxCount);

        p = new Point(x + rng, y + rng);
        UpdMaxUnitsPoint(field, p, ref maxPoint, ref maxCount);

        p = new Point(x - rng, y);
        UpdMaxUnitsPoint(field, p, ref maxPoint, ref maxCount);

        p = new Point(x, y - rng);
        UpdMaxUnitsPoint(field, p, ref maxPoint, ref maxCount);

        p = new Point(x - rng, y - rng);
        UpdMaxUnitsPoint(field, p, ref maxPoint, ref maxCount);
      }

      return maxPoint;
    }

    private static void UpdMaxUnitsPoint(Field field, Point p, ref Point? maxPoint, ref int maxCount)
    {
      if (p != field.StaticData.Finish)
      {
        var countUnits = field.FindUnitsAt(p).Count();
        if (countUnits > maxCount)
        {
          maxCount = countUnits;
          maxPoint = p;
        }
      }
    }

    private double GetKilledUnitHealthDeltaWithZero(TowerStats stats, Unit unit)
    {
      var damage = _gameCalculator.CalculateDamage(unit.Type, stats);
      var resultedHealth = unit.Health - damage;
      return resultedHealth > 0 ? 1 : 0 - (double)resultedHealth / damage;
    }
  }
}
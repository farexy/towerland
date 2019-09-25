using System;
using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Extensions;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Selectors
{
  public class PathChooser : IPathChooser
  {
    private readonly IStatsLibrary _statsLib;
    private readonly AdditiveConvolutionCalculator _additiveConvolutionCalculator;
    private readonly GameCalculator _gameCalculator;

    private static readonly double[] OptimalPathCriteriaWeights =
    {
      0.1, // first criteria - number of towers on the path
      0.5, // second criteria - total damage of towers on the path for the monster
      0.3, // third criteria - remoteness of towers from beginning (length of the possible path)
      0.1, // fourth criteria - ratio of towers with special coef
    };

    public PathChooser(IStatsLibrary statsLibrary)
    {
      _gameCalculator = new GameCalculator(statsLibrary);
      _additiveConvolutionCalculator = new AdditiveConvolutionCalculator(OptimalPathCriteriaWeights);
      _statsLib = statsLibrary;
    }

    public int GetFastestPath(Path[] path, Unit unit)
    {
      var pos = unit.Position;
      var possible = GetPossiblePath(path, pos).ToArray();

      int[] coefs = new int[possible.Length];

      for (int i = 0; i < possible.Length; i++)
      {
        coefs[i] = possible[i].Length - possible[i].PointOnThePathPosition(pos);
      }

      return Array.IndexOf(coefs, coefs.Min());
    }

    public int GetOptimalPath(Field field, Unit unit)
    {
      var paths = field.StaticData.Path;
      double minDamage = double.MaxValue;
      int minDamagePathId = 0;
      for (int i = 0; i < paths.Length; i++)
      {
        var towersOnPath = FindTowersThatCanAttackPath(field, i);
        var towers = towersOnPath.Select(t => field[t].Type).ToArray();
        var pathDamage = GetTotalAttackDamage(towers, unit);

        if (pathDamage < minDamage)
        {
          minDamagePathId = i;
          minDamage = pathDamage;
        }
      }

      return minDamagePathId;
// TODO optimize
//      var tableToAnalize = new double[paths.Length, _additiveConvolutionCalculator.NumberOfCriterias];
//
//      for (int i = 0; i < paths.Length; i++)
//      {
//        var towersOnPath = FindTowersThatCanAttackPath(field, i);
//        var towers = towersOnPath.Select(t => field[t]).ToArray();
//        var towersTypes = towers.Select(t => t.Type).ToArray();
//
//        tableToAnalize[i, 0] = 1 - (double)towersOnPath.Count / field.State.Towers.Count;
//        tableToAnalize[i, 1] = GetTotalAttackDamage(towersTypes, unit);
//        tableToAnalize[i, 2] = GetAvgTowersRemoteness(paths[i], towers);
//        tableToAnalize[i, 3] = GetTowersWithSpecialEffectRate(towersTypes);
//      }
//      return _additiveConvolutionCalculator.FindOptimalVariantIndex(tableToAnalize);
    }

    private double GetTotalAttackDamage(IEnumerable<GameObjectType> towerTypes, Unit unit)
    {
      return 1 - (double)towerTypes.Sum(t => _gameCalculator.CalculateDamage(unit.Type, t)) / unit.Health;
    }

    private double GetTowersWithSpecialEffectRate(ICollection<GameObjectType> towerTypes)
    {
      return 1 - (double)towerTypes.Count(t => _statsLib.GetTowerStats(t).Skill != SkillId.None) / towerTypes.Count;
    }

    private double GetAvgTowersRemoteness(Path path, ICollection<GameObject> towers)
    {
      double remotenessSum = 0;
      foreach (var tower in towers)
      {
        var pos = path.First(point => _gameCalculator.IsTowerCanAttack(tower, point));
        remotenessSum += (double)path.PointOnThePathPosition(pos) / path.Length;
      }

      return remotenessSum / towers.Count;
    }

    private static IEnumerable<Path> GetPossiblePath(IEnumerable<Path> path, Point position)
    {
      return path.Where(p => p.PointOnThePathPosition(position) != -1);
    }

    private HashSet<int> FindTowersThatCanAttackPath(Field field, int pathId)
    {
      var path = field.StaticData.Path[pathId];
      var towers = new HashSet<int>();
      foreach (var point in path)
      {
        towers.UnionWith(field.FindTowersThatCanAttack(point, _statsLib));
      }
      return towers;
    }
  }
}

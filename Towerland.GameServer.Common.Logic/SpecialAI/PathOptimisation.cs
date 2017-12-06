using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic.SpecialAI
{
  public class PathOptimisation : IPathOptimiser
  {
    private readonly IStatsLibrary _statsLib;

    public PathOptimisation(IStatsLibrary statsLibrary)
    {
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

    public int GetOptimalPath(Path[] paths, Field field, Unit unit)
    {
      var optimalPathIndex = 0;
      var minTowerCount = int.MaxValue;
      for (int i = 0; i < paths.Length; i++)
      {
        var towersOnPath = FindTowersThatCanAttackPath(field, i);
        if (towersOnPath.Count < minTowerCount)
        {
          minTowerCount = towersOnPath.Count;
          optimalPathIndex = i;
        }
      }
      return optimalPathIndex;
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

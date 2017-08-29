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

    public int GetOptimalPath(Path[] paths, Unit unit)
    {
      throw new NotImplementedException();
    }

    private static IEnumerable<Path> GetPossiblePath(IEnumerable<Path> path, Point position)
    {
      return path.Where(p => p.PointOnThePathPosition(position) != -1);
    }
  }
}

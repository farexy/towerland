﻿using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Extensions
{
  internal static class FieldExtensions
  {
    public static void MoveUnit(this Field field, int gameId, Point position, int wait)
    {
      var unit = field[gameId];
      unit.Position = position;
      unit.WaitTicks = wait;
    }

    public static Tower FindTowerAt(this Field field, Point position)
    {
      return field.State.Towers.FirstOrDefault(t => t.Position == position);
    }

    public static IEnumerable<Unit> FindTargetsAt(this Field field, Point position)
    {
      if (position.Y < 0 || position.X < 0 || position.Y > field.StaticData.Height - 1 || position.X > field.StaticData.Width - 1
          || !field.StaticData.Cells[position.X, position.Y].Object.HasFlag(FieldObject.Road))
      {
        return Enumerable.Empty<Unit>();
      }
      return field.State.Units.Where(u => u.Position == position);
    }

    public static IEnumerable<Unit> FindTargetsAt(this Field field, IEnumerable<Point> positions)
    {
      if (positions.Contains(field.StaticData.Finish))
      {
        return Enumerable.Empty<Unit>();
      }
      return field.State.Units.Where(u => positions.Contains(u.Position));
    }

    public static int[] FindTowersThatCanAttack(this Field field, Point position, IStatsLibrary stats)
    {
      var gameCalc = new GameCalculator(stats);
      return field.FindGameObjects(obj =>
          obj.IsTower
          && gameCalc.IsTowerCanAttack(obj, position))
        .Select(obj => obj.GameId)
        .ToArray();
    }

    public static int[] FindPossibleTargetsForTower(this Field field, Tower tower, IStatsLibrary stats)
    {
      var gameCalc = new GameCalculator(stats);
      return field.FindGameObjects(obj =>
          obj.IsUnit
          && gameCalc.IsTowerCanAttack(tower, obj.Position))
        .Select(obj => obj.GameId)
        .ToArray();
    }

    public static IEnumerable<int> GetPossiblePathIds(this Field field, Point position)
    {
      for (int i = 0; i < field.StaticData.Path.Length; i++)
      {
        if (field.StaticData.Path[i].PointOnThePathPosition(position) != -1)
        {
          yield return i;
        }
      }
    }

    public static IEnumerable<Point> GetNeighbourPoints(this Field f, Point p, int range, FieldObject fieldObjectToMatch)
    {
      for (int x = -range; x < range; x++)
      {
        for (int y = -range; y < range; y++)
        {
          if (p.X + x >= 0 && p.X + x < f.StaticData.Width && p.Y + y >= 0 && p.Y + y < f.StaticData.Height
            && f.StaticData.Cells[p.X + x, p.Y + y].Object.HasFlag(fieldObjectToMatch))
          {
            yield return new Point(p.X + x, p.Y + y);
          }
        }
      }
    }
  }
}

using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Calculators
{
  internal static class FieldExtensions
  {
    public static void MoveUnit(this Field field, int gameId, Point position, int wait)
    {
      var unit = field[gameId];
      unit.Position = position;
      unit.WaitTicks = wait;
    }

    public static IEnumerable<Unit> FindUnitsAt(this Field field, Point position)
    {
      return field.State.Units.Where(u => u.Position == position);
    }

    public static int[] FindTowersThatCanAttack(this Field field, Point position, IStatsLibrary stats)
    {
      var gameCalc = new GameCalculator(stats);
      return field.FindGameObjects(obj =>
          obj.ResolveType() == GameObjectType.Tower
          && gameCalc.IsTowerCanAttack(obj, position))
        .Select(obj => obj.GameId)
        .ToArray();
    }

    public static int[] FindePossibleTargetsForTower(this Field field, Tower tower, IStatsLibrary stats)
    {
      var gameCalc = new GameCalculator(stats);
      return field.FindGameObjects(obj =>
          obj.ResolveType() == GameObjectType.Unit
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

    public static IEnumerable<Point> GetNeighbourPoints(this Field f, Point p, int range, FieldObject cellTypeToMatch)
    {
      for (int x = -range; x < range; x++)
      {
        for (int y = -range; y < range; y++)
        {
          if (p.X + x >= 0 && p.X + x < f.StaticData.Width && p.Y + y >= 0 && p.Y + y < f.StaticData.Height
              && f.StaticData.Cells[p.X + x, p.Y + y].Object == cellTypeToMatch)
          {
            yield return new Point(p.X + x, p.Y + y);
          }
        }
      }
    }
  }
}

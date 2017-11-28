using System.Collections.Generic;
using System.Linq;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic.SpecialAI
{
  static class FieldExtensions
  {
    public static void MoveUnit(this Field field, int gameId, Point position, int wait)
    {
      var unit = (Unit) field[gameId];
      var path = field.StaticData.Path[unit.PathId.Value];
      unit.Position = position;
      unit.WaitTicks = wait;
    }

    public static Unit[] FindUnitsAt(this Field field, Point position)
    {
      return field.FindGameObjects(obj => obj.Position == position).OfType<Unit>().ToArray();
    }

    public static int[] FindTowersThatCanAttack(this Field field, Point position, IStatsLibrary stats)
    {
      return field.FindGameObjects(obj => 
          obj.ResolveType() == GameObjectType.Tower 
          && stats.GetTowerStats(obj.ResolveType()).Range >= GameMath.Distance(obj.Position, position))
        .Select(obj => obj.GameId)
        .ToArray();
    }
    
    public static IEnumerable<Path> GetPossiblePath(this Field field, Point position)
    {
      return field.StaticData.Path.Where(p => p.PointOnThePathPosition(position) != -1);
    }
  }
}

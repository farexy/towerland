using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic.SpecialAI
{
  internal static class FieldExtensions
  {
    public static void MoveUnit(this Field field, int gameId, Point position, int wait)
    {
      var unit = field[gameId];
      unit.Position = position;
      unit.WaitTicks = wait;
    }

    public static Unit[] FindUnitsAt(this Field field, Point position)
    {
      return field.State.Units.Where(u => u.Position == position).ToArray();
    }

    public static int[] FindTowersThatCanAttack(this Field field, Point position, IStatsLibrary stats)
    {
      return field.FindGameObjects(obj => 
          obj.ResolveType() == GameObjectType.Tower 
          && stats.GetTowerStats(obj.Type).Range >= GameMath.Distance(obj.Position, position))
        .Select(obj => obj.GameId)
        .ToArray();
    }
    
    public static IEnumerable<Path> GetPossiblePath(this Field field, Point position)
    {
      return field.StaticData.Path.Where(p => p.PointOnThePathPosition(position) != -1);
    }
  }
}

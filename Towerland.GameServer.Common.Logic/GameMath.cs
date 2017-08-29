using System;
using GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Common.Logic
{
  static class GameMath
  {
    public static double Distance(Point p1, Point p2)
    {
      return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
    }
  }
}

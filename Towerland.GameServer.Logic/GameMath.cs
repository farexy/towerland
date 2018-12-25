using System;
using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Logic
{
  public static class GameMath
  {
    public static readonly Random Rand = new Random();
    
    public static double Distance(Point p1, Point p2)
    {
      return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
    }

    public static bool CalcProbableEvent(int probabilityPercent)
    {
      return Rand.Next(0, 100) <= probabilityPercent;
    }

    public static int Round(double num)
    {
      return (int) Math.Round(num);
    }
  }
}

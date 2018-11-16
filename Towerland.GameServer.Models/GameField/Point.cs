using Newtonsoft.Json;

namespace Towerland.GameServer.Models.GameField
{
  public struct Point
  {
    [JsonProperty("x")]public int X;
    [JsonProperty("y")]public int Y;

    public Point(int x, int y)
    {
      X = x;
      Y = y;
    }

    public static bool operator ==(Point p1, Point p2)
    {
      return p1.Equals(p2);
    }

    public static bool operator !=(Point p1, Point p2)
    {
      return !p1.Equals(p2);
    }

    public override string ToString()
    {
      return string.Format("({0},{1})", X, Y);
    }
  }
}

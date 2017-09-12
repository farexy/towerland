namespace GameServer.Common.Models.GameField
{
  public struct Point
  {
    public int X;
    public int Y;

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

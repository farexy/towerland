using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Towerland.GameServer.Common.Models.GameField
{
  public class Path : IEnumerable<Point>
  {
    private readonly ICollection<Point> _pts;
    
    public Path(ICollection<Point> way)
    {
      _pts = way;
      _way = way.ToArray();
    }

    private readonly Point[] _way;

    public Point End { get { return _way.Last(); } }

    public Point GetNext(Point current)
    {
      if(_way.Last() == current)
        throw new ArgumentException("Point is last on the path");

      for (int i = 0; i < _way.Length - 1; i++)
      {
        if (_way[i] == current)
        {
          return _way[i + 1];
        }
      }
      throw new ArgumentException("Point is not on the path");
    }

    public Point GetPrevious(Point current)
    {
      if (_way.First() == current)
        throw new ArgumentException("Point is first on the path");

      for (int i = 1; i < _way.Length; i++)
      {
        if (_way[i + 1] == current)
        {
          return _way[i];
        }
      }
      throw new ArgumentException("Point is not on the path");
    }

    public int PointOnThePathPosition(Point p)
    {
      return Array.IndexOf(_way, p);
    }

    public int Length
    {
      get { return _way.Length; }
    }

    public IEnumerator<Point> GetEnumerator()
    {
      return _pts.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public override string ToString()
    {
      return string.Join(",", _way.Select(p => p.ToString()).ToArray());
    }
  }
}

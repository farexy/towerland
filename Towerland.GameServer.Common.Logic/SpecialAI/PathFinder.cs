using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Common.Logic
{
  class PathFinder
  {
    private const int GroundIndicator = -10;
    private static readonly Point NullPoint = new Point(-1, -1);

    public IEnumerable<Path> ResolvePath(FieldCell[,] cells, int width, int height)
    {
      Point start, finish;
      int[,] map = CalcWave(cells, width, height, out start, out finish);
      bool[,] visited = new bool[width, height];
      for (int i = 0; i < width; i++)
      {
        for (int j = 0; j < height; j++)
        {
          if (map[i, j] == GroundIndicator)
            visited[i, j] = true;
        }
      }

      var path = new List<Path>();
      var forks = new Stack<Point>();

      int x = start.X;
      int y = start.Y;
      int current = 0;

      do
      {
        var way = new List<Point>();
        var fork = forks.Any() ? forks.Pop() : NullPoint;

        while (x != finish.X && y != finish.Y)
        {
          var currPoint = new Point(x, y);
          int countDirs = 0;
          var nextPoint = new Point(x, y);

          if (fork != currPoint)
          {
            if (x != 0 && map[x - 1, y] == current + 1)
            {
              nextPoint = new Point(x - 1, y);
              countDirs++;
            }
            if (y != 0 && map[x, y - 1] == current + 1)
            {
              nextPoint = new Point(x, y - 1);
              countDirs++;
            }
            //we need to add the comparison on equality, otherwise enemy will always go to left or down when the way values are equal
            if (x != width - 1 && map[x + 1, y] == current + 1)
            {
              nextPoint = new Point(x + 1, y);
              countDirs++;
            }
            if (y != height - 1 && map[x, y + 1] == current + 1)
            {
              nextPoint = new Point(x, y + 1);
              countDirs++;
            }
          }
          else
          {
            if (x != 0 && map[x - 1, y] == current + 1)
            {
              nextPoint = new Point(x - 1, y);
              countDirs++;
            }
            if (y != 0 && map[x, y - 1] == current + 1)
            {
              nextPoint = new Point(x, y - 1);
              countDirs++;
            }
            //we need to add the comparison on equality, otherwise enemy will always go to left or down when the way values are equal
            if (x != width - 1 && map[x + 1, y] == current + 1)
            {
              nextPoint = new Point(x + 1, y);
              countDirs++;
            }
            if (y != height - 1 && map[x, y + 1] == current + 1)
            {
              nextPoint = new Point(x, y + 1);
              countDirs++;
            }
          }

          if (countDirs > 1)
            forks.Push(currPoint);

          way.Add(currPoint);
          visited[x, y] = true;
          x = nextPoint.X;
          y = nextPoint.Y;
          current++;
        }



        x = start.X;
        y = start.Y;
        path.Add(new Path(way));
        
      } while (forks.Any());

      return path;
    }

    private static int[,] CalcWave(FieldCell[,] cells, int width, int height, out Point startPoint, out Point finishPoint)
    {
      startPoint = new Point();
      finishPoint = new Point();
      int[,] map = new int[width, height];

      int groundIndicator = GroundIndicator; //represents the wall
      int notVisited = -1; // -1 represents the cell, where we were not 
      int i, j, step = 0;

      //fill the cells with the values denending on whether it's wall or floor
      for (j = 0; j < width; j++)
        for (i = 0; i < height; i++)
        {
          var obj = cells[j, i].Object;

          if (obj == FieldObject.Ground)
            map[j, i] = groundIndicator;
          else if (obj == FieldObject.Road)
            map[j, i] = notVisited;
          else if (obj == FieldObject.Entrance)
          {
            map[j, i] = step;
            startPoint = new Point(j, i);
          }
          else if (obj == FieldObject.Castle)
            finishPoint = new Point(j, i);
          else map[j, i] = notVisited;
        }

      //we watch the cells of maze while target point not found
      while (step < cells.Length / 2)
      {
        for (i = 0; i < width; i++)
          for (j = 0; j < height; j++)
          {
            if (map[i, j] == step)
            {
              if (i != 0 && map[i - 1, j] != groundIndicator && map[i - 1, j] == notVisited)
                map[i - 1, j] = step + 1;
              if (j != 0 && map[i, j - 1] != groundIndicator && map[i, j - 1] == notVisited)
                map[i, j - 1] = step + 1;
              if (i != width - 1 && map[i + 1, j] != groundIndicator && map[i + 1, j] == notVisited)
                map[i + 1, j] = step + 1;
              if (j != height - 1 && map[i, j + 1] != groundIndicator && map[i, j + 1] == notVisited)
                map[i, j + 1] = step + 1;
            }
          }
        step++;
      }

      return map;
    }

    [Flags]
    private enum Direction
    {
      None, Up, Down, Left, Right
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Logic.SpecialAI
{
  public class PathFinder
  {
    private const int GroundIndicator = -10;
    private static readonly Point NullPoint = new Point(-1, -1);
    private static int _forkPool = 0;

    public IEnumerable<Path> ResolvePath(FieldCell[,] cells, Point start, Point finish)
    {
      int width = cells.GetLength(0);
      int height = cells.GetLength(1);
      var map = WrapField(cells);
      var startQueue = new Queue<Point>();
      startQueue.Enqueue(start);
      var finishQueue = new Queue<Point>();
      finishQueue.Enqueue(finish);
      while (startQueue.Any() || finishQueue.Any())
      {
        if(startQueue.Any())
          FindNextFork(startQueue, map, width, height, false);
        if(finishQueue.Any())
          FindNextFork(finishQueue, map, width, height, true);
      }
      return ResolvePaths(map, start, finish, width, height);
    }

    private static void FindNextFork(Queue<Point> queue, FieldCellWrapper[,] map, int width, int height, bool reverseDirections)
    {
      var fork = queue.Dequeue();
      int forkDirsCount = 0;
      Point point = FindNextPoint(fork, map, width, height, reverseDirections, ref forkDirsCount);
      while (point != NullPoint)
      {
        while (true)
        {
          int countDirs = 0;
          var nextPoint = FindNextPoint(point, map, width, height, reverseDirections, ref countDirs);
          if (nextPoint == NullPoint)
          {
            break;
          }

          if (countDirs > 1 && map[nextPoint.X, nextPoint.Y].Direction != Direction.Fork)
          {
            map[point.X, point.Y].Direction = Direction.Fork;
            map[point.X, point.Y].ForkId = _forkPool++;
            queue.Enqueue(point);
            break;
          }
          point = nextPoint;
        }
        if (fork == new Point(1, 3))
        {
          Console.WriteLine();
        }
        point = FindNextPoint(fork, map, width, height, reverseDirections, ref forkDirsCount);
        //Show(map, width, height);
        //Console.ReadKey();
        //Console.Clear();
      }
    }

    private static Point FindNextPoint(Point point, FieldCellWrapper[,] map, int width, int height, bool reverseDirections, ref int countDirections)
    {
      Point nextPoint = NullPoint;
      int x = point.X;
      int y = point.Y;

      if (x != 0 && map[x - 1, y].IsRoad && map[x - 1, y].NotVisited)
      {
        nextPoint = new Point(x - 1, y);
        if (map[x, y].Direction != Direction.Fork)
        {
          map[x, y].Direction = reverseDirections ? Direction.Right : Direction.Left;
        }
        countDirections++;
      }

      if (y != 0 && map[x, y - 1].IsRoad && map[x, y - 1].NotVisited)
      {
        nextPoint = new Point(x, y - 1);
        if (map[x, y].Direction != Direction.Fork)
        {
          map[x, y].Direction = reverseDirections ? Direction.Down : Direction.Up;
        }
        countDirections++;
      }
      //we need to add the comparison on equality, otherwise enemy will always go to left or down when the way values are equal

      if (x != width - 1 && map[x + 1, y].IsRoad && map[x + 1, y].NotVisited)
      {
        nextPoint = new Point(x + 1, y);
        if (map[x, y].Direction != Direction.Fork)
        {
          map[x, y].Direction = reverseDirections ? Direction.Left : Direction.Right;
        }
        countDirections++;
      }

      if (y != height - 1 && map[x, y + 1].IsRoad && map[x, y + 1].NotVisited)
      {
        nextPoint = new Point(x, y + 1);
        if (map[x, y].Direction != Direction.Fork)
        {
          map[x, y].Direction = reverseDirections ? Direction.Up : Direction.Down;
        }
        countDirections++;
      }

      if (nextPoint != NullPoint)
      {
        return nextPoint;
      }

      if (x != 0 && map[x, y].NotVisited && map[x - 1, y].Direction == Direction.Fork)
      {
        if (map[x, y].Direction != Direction.Fork)
        {
          map[x, y].Direction = reverseDirections ? Direction.Right : Direction.Left;
        }
        countDirections++;
      }
      if (y != 0 && map[x, y].NotVisited && map[x, y - 1].Direction == Direction.Fork)
      {
        if (map[x, y].Direction != Direction.Fork)
        {
          map[x, y].Direction = reverseDirections ? Direction.Down : Direction.Up;
        }
        countDirections++;
      }
      if (x != width - 1 && map[x, y].NotVisited && map[x + 1, y].Direction == Direction.Fork)
      {
        if (map[x, y].Direction != Direction.Fork)
        {
          map[x, y].Direction = reverseDirections ? Direction.Left : Direction.Right;
        }
        countDirections++;
      }
      if (y != height - 1 && map[x, y].NotVisited && map[x, y + 1].Direction == Direction.Fork)
      {
        if (map[x, y].Direction != Direction.Fork)
        {
          map[x, y].Direction = reverseDirections ? Direction.Up : Direction.Down;
        }
        countDirections++;
      }

      return nextPoint;
    }

    private static FieldCellWrapper[,] WrapField(FieldCell[,] cells)
    {
      int width = cells.GetLength(0);
      int height = cells.GetLength(1);
      var wrapper = new FieldCellWrapper[width,height];
      for (int i = 0; i < width; i++)
      {
        for (int j = 0; j < height; j++)
        {
          wrapper[i, j] = new FieldCellWrapper(cells[i, j]);
        }
      }
      return wrapper;
    }


    private static IEnumerable<Path> ResolvePaths(FieldCellWrapper[,] cells, Point start, Point end, int width, int height)
    {Show(cells, width, height);
      List<Path> pathList = new List<Path>();
      Stack<int> forkStack = new Stack<int>();
      do
      {
        List<Point> pathPoints = new List<Point>();
        for (Point curPoint = start; curPoint != end;)
        {
          int currFork = forkStack.Any() ? forkStack.Pop() : -1;
          var curCell = cells[curPoint.X, curPoint.Y];
          pathPoints.Add(curPoint);
          curCell.IncludedInPath = true;
          if (curPoint == new Point(8,7))
          {
            Console.WriteLine();
          }
          if (curCell.ForkId.HasValue)
          {
            if (!forkStack.Contains(curCell.ForkId.Value))
            {
              forkStack.Push(curCell.ForkId.Value);
            }
            if (curCell.ForkId == currFork)
            {
              curPoint = curPoint.X != 0 && cells[curPoint.X - 1, curPoint.Y].IsRoad && !cells[curPoint.X - 1, curPoint.Y].IncludedInPath 
                ? cells[curPoint.X - 1, curPoint.Y].Position
                : curPoint.X != width - 1 && cells[curPoint.X + 1, curPoint.Y].IsRoad && !cells[curPoint.X + 1, curPoint.Y].IncludedInPath 
                  ? cells[curPoint.X + 1, curPoint.Y].Position
                  : curPoint.Y != 0 && cells[curPoint.X, curPoint.Y - 1].IsRoad && !cells[curPoint.X, curPoint.Y - 1].IncludedInPath 
                    ? cells[curPoint.X, curPoint.Y - 1].Position 
                    : curPoint.Y != height - 1 && cells[curPoint.X, curPoint.Y + 1].IsRoad && !cells[curPoint.X, curPoint.Y + 1].IncludedInPath
                      ? cells[curPoint.X, curPoint.Y + 1].Position
                      : NullPoint;
            }
            else
            {
              curPoint = curPoint.X != 0 && cells[curPoint.X - 1, curPoint.Y].IsRoad && cells[curPoint.X - 1, curPoint.Y].Direction != Direction.Left
                ? cells[curPoint.X - 1, curPoint.Y].Position
                : curPoint.X != width - 1 && cells[curPoint.X + 1, curPoint.Y].IsRoad && cells[curPoint.X - 1, curPoint.Y].Direction != Direction.Right
                  ? cells[curPoint.X + 1, curPoint.Y].Position
                  : curPoint.Y != 0 && cells[curPoint.X, curPoint.Y - 1].IsRoad && cells[curPoint.X - 1, curPoint.Y].Direction != Direction.Up
                    ? cells[curPoint.X, curPoint.Y - 1].Position
                    : curPoint.Y != height - 1 && cells[curPoint.X, curPoint.Y + 1].IsRoad && cells[curPoint.X - 1, curPoint.Y].Direction != Direction.Down
                      ? cells[curPoint.X, curPoint.Y + 1].Position
                      : NullPoint;
            }
          }
          else
          {
            curPoint = GetNextCellByDirection(curCell);
          }
        }
        pathList.Add(new Path(pathPoints));
      } while (forkStack.Any());
      return pathList;
    }

    private static Point GetNextCellByDirection(FieldCellWrapper cell)
    {
      switch (cell.Direction)
      {
        case Direction.Left:
          return new Point(cell.Position.X - 1, cell.Position.Y);
        case Direction.Right:
          return new Point(cell.Position.X + 1, cell.Position.Y);
        case Direction.Up:
          return new Point(cell.Position.X, cell.Position.Y - 1);
        case Direction.Down:
          return new Point(cell.Position.X, cell.Position.Y + 1);
        default:
          return cell.Position;
      }
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

    //private static void Swap(Queue<>)

    private static void Show(FieldCellWrapper[,] map, int width, int height)
    {
      for (int i = 0; i < width; i++)
      {
        for (int j = 0; j < height; j++)
        {
          if (map[i, j].IsRoad)
          {
            switch (map[i, j].Direction)
            {
              case Direction.Up:
                Console.Write("l");
                break;
              case Direction.Down:
                Console.Write("r");
                break;
              case Direction.Left:
                Console.Write("u");
                break;
              case Direction.Right:
                Console.Write("d");
                break;
              case Direction.None:
                Console.Write("n");
                break;
              case Direction.Fork:
                Console.Write("f");
                break;
            }
          }
          else
          {
            Console.Write("0");
          }
        }
        Console.WriteLine();
      }
    }

    private static void Show2(FieldCellWrapper[,] map, int width, int height)
    {
      for (int i = 0; i < width; i++)
      {
        for (int j = 0; j < height; j++)
        {
          if (map[i, j].IsRoad)
          {
            switch (map[i, j].Direction)
            {
              case Direction.Up:
                Console.Write("l");
                break;
              case Direction.Down:
                Console.Write("r");
                break;
              case Direction.Left:
                Console.Write("u");
                break;
              case Direction.Right:
                Console.Write("d");
                break;
              case Direction.None:
                Console.Write("n");
                break;
              case Direction.Fork:
                Console.Write("f");
                break;
            }
          }
          else
          {
            Console.Write("0");
          }
        }
        Console.WriteLine();
      }
    }

    private enum Direction
    {
      None, Up, Down, Left, Right, Fork
    }

    private class FieldCellWrapper
    {
      public FieldCellWrapper(FieldCell cell)
      {
        _cell = cell;
      }

      private FieldCell _cell;
      public Direction Direction;
      public int? ForkId;
      public bool IncludedInPath;

      public Point Position { get { return _cell.Position; } }
      public FieldObject Obj {get {return _cell.Object;}}
      public bool IsRoad { get { return _cell.Object == FieldObject.Road; }}
      public bool NotVisited { get { return Direction == Direction.None; } }
    } 
  }
}

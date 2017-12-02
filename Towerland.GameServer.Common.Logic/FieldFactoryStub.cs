using System;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic
{
  public class FieldFactoryStub : IFieldFactory
  {
    private const int FieldRoadCoeff = 30;
    
    private static Field _classicField;


    private static readonly int[,] Cells = 
    {
      {1,1,1,1,1,1,1,1,1,1},
      {2,0,0,0,0,0,0,0,1,1},
      {1,1,0,1,1,1,1,0,0,1},
      {1,1,0,0,0,0,1,1,0,1},
      {1,1,0,1,1,0,1,1,0,1},
      {1,1,0,1,1,0,0,0,0,1},
      {1,1,0,1,1,1,1,0,1,1},
      {1,1,0,0,1,1,1,0,1,1},
      {1,1,1,0,0,0,0,0,1,1},
      {1,1,1,1,1,1,1,3,1,1}
    };

    private static readonly Point[] Path1 =
    {
      new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(1, 4), new Point(1, 5),
      new Point(1, 6), new Point(1, 7), new Point(2, 7), new Point(2, 8), new Point(3, 8), new Point(4, 8),  
      new Point(5, 8), new Point(5, 7), new Point(6, 7), new Point(7, 7), new Point(8, 7), new Point(9, 7),   
    };

    private static readonly Point[] Path2 =
    {
      new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(3, 2), new Point(3, 3),
      new Point(3, 4), new Point(3, 5), new Point(4, 5), new Point(5, 5), new Point(5, 6), new Point(5, 7), 
      new Point(6, 7), new Point(7, 7), new Point(8, 7), new Point(9, 7),
    };

    private static readonly Point[] Path3 =
    {
      new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(3, 2), new Point(4, 2),
      new Point(5, 2), new Point(6, 2), new Point(7, 2), new Point(7, 3), new Point(8, 3), new Point(8, 4), 
      new Point(8, 5), new Point(8, 6), new Point(8, 7), new Point(9, 7),
    }; 

    public Field ClassicField
    {
      get
      {
        if (_classicField != null)
          return _classicField;

        var cells = new FieldCell[Cells.GetLength(0), Cells.GetLength(1)];

        for (int i = 0; i < Cells.GetLength(0); i++)
        {
          for (int j = 0; j < Cells.GetLength(1); j++)
          {
            cells[i,j] = new FieldCell
            {
              Position = new Point(i,j),
              Object = (FieldObject)Cells[i,j]
            };
          }
        }

        _classicField = new Field(cells)
        {
          State =
          {
            Castle = new Castle
            {
              Health = 100,
              Position = new Point(7, 9)
            },
            MonsterMoney = 100,
            TowerMoney = 100,
          },
          StaticData =
          {
            Path = new[] { new Path(Path1), new Path(Path2), new Path(Path3) }
          }
        };
        
        return _classicField;
      }
    }

    public Field GenerateNewField(int width, int height, Point startPoint, Point endPoint)
    {
      int roadCount = width * height / FieldRoadCoeff;
      var map = CalcWave(width, height, startPoint);

      return new Field(new FieldCell[2,2]);
    }

    private static int[,] CalcWave(int width, int height, Point startPoint)
    {
      int[,] map = new int[width, height];

      //int groundIndicator = GroundIndicator; //represents the wall
      int notVisited = -1; // -1 represents the cell, where we were not 
      int i, j, step = 0;

      for (j = 0; j < width; j++)
      {
        for (i = 0; i < height; i++)
        {
          map[j, i] = notVisited;
        }
      }

      map[startPoint.X, startPoint.Y] = step;

      //we watch the cells of maze while target point not found
      while (step <= Math.Max(height, width) * 2)
      {
        for (i = 0; i < width; i++)
        for (j = 0; j < height; j++)
        {
          if (map[i, j] == step)
          {
            if (i != 0 && map[i - 1, j] == notVisited)
              map[i - 1, j] = step + 1;
            if (j != 0 && map[i, j - 1] == notVisited)
              map[i, j - 1] = step + 1;
            if (i != width - 1 && map[i + 1, j] == notVisited)
              map[i + 1, j] = step + 1;
            if (j != height - 1 && map[i, j + 1] == notVisited)
              map[i, j + 1] = step + 1;
          }
        }
        step++;
      }

      return map;
    }
  }
}

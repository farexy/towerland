using System;
using System.Linq;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic
{
  public class FieldFactoryStub : IFieldFactory
    {
        private const int FieldRoadCoeff = 30;
        private const int BattleDurationMinutes = 6;

        private static Field _classicField;


        private static readonly int[,] Cells2 =
        {
      {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
      {3,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,1,1},
      {1,1,0,1,1,1,1,0,0,0,0,0,0,0,1,0,1,1},
      {1,1,0,0,0,0,1,1,0,1,1,1,1,1,1,0,1,1},
      {1,1,0,1,1,0,1,1,0,1,1,1,1,1,1,0,1,1},
      {1,1,0,1,1,0,0,0,0,1,1,1,1,1,1,0,1,1},
      {1,1,0,1,1,1,1,0,1,1,1,1,1,1,1,0,1,1},
      {1,1,0,0,1,1,1,0,0,0,0,0,1,1,1,0,0,1},
      {1,1,1,0,0,0,0,0,1,1,1,0,1,1,1,1,0,1},
      {1,1,1,1,0,1,1,0,1,1,1,0,1,1,1,1,0,1},
      {1,1,1,1,0,1,1,0,1,1,1,0,1,1,1,1,0,1},
      {1,1,1,1,0,1,1,0,1,1,1,0,0,0,0,0,0,1},
      {1,1,0,0,0,1,1,0,1,1,1,1,1,1,0,1,1,1},
      {1,1,0,1,1,1,1,0,1,1,1,1,1,1,0,1,1,1},
      {1,1,0,0,1,1,1,0,1,1,1,1,1,1,0,1,1,1},
      {1,1,1,0,0,0,1,0,0,0,0,0,0,0,0,1,1,1},
      {1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1},
      {1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1}
    };

        private static readonly Point[] Path1 =
        {
      new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(1, 4), new Point(1, 5),
      new Point(1, 6), new Point(1, 7), new Point(2, 7), new Point(2, 8), new Point(2, 9), new Point(2, 10),
      new Point(2, 11), new Point(2, 12), new Point(2, 13), new Point(1,13), new Point(1,14), new Point(1,15),
      new Point(2,15), new Point(3,15), new Point(4,15), new Point(5,15), new Point(6,15), new Point(7,15),
      new Point(7,16), new Point(8,16), new Point(9,16), new Point(10,16), new Point(11,16), new Point(11,15),
      new Point(11,14), new Point(12,14), new Point(13,14), new Point(14,14), new Point(15,14), new Point(15,13),
      new Point(15,12), new Point(15,11), new Point(15,10), new Point(15,9), new Point(15,8), new Point(15,7),
      new Point(16,7), new Point(17,7),
    };
        
        private static readonly Point[] Path2 =
        {
            new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(1, 4), new Point(1, 5),
            new Point(1, 6), new Point(1, 7), new Point(2, 7), new Point(2, 8), new Point(3, 8), new Point(4, 8),
            new Point(5, 8), new Point(5, 7), new Point(6, 7), new Point(7,7), new Point(7,8), new Point(7,9),
            new Point(7,10), new Point(7,11), new Point(8,11), new Point(9,11), new Point(10,11), new Point(11,11),
            new Point(11,12), new Point(11,13), new Point(11,14), new Point(12,14), new Point(13,14), new Point(14,14),
            new Point(15,14), new Point(15,13), new Point(15,12), new Point(15,11), new Point(15,10), new Point(15,9),
            new Point(15,8), new Point(15,7), new Point(16,7), new Point(17,7),
        };
        
        private static readonly Point[] Path3 =
        {
            new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(1, 4), new Point(1, 5),
            new Point(1, 6), new Point(1, 7), new Point(2, 7), new Point(2, 8), new Point(3, 8), new Point(4, 8),
            new Point(5, 8), new Point(5, 7), new Point(6, 7), new Point(7,7), new Point(8,7), new Point(9,7),
            new Point(10,7), new Point(11,7), new Point(12,7), new Point(13,7), new Point(14,7), new Point(15,7),
            new Point(16,7), new Point(17,7),
        };
        
        private static readonly Point[] Path4 =
        {
            new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(3, 2), new Point(3, 3),
            new Point(3, 4), new Point(3, 5), new Point(4, 5), new Point(5, 5), new Point(5, 6), new Point(5, 7), 
            new Point(6, 7), new Point(7,7), new Point(8,7), new Point(9,7), new Point(10,7), new Point(11,7),
            new Point(12,7), new Point(13,7), new Point(14,7), new Point(15,7), new Point(16,7), new Point(17,7),
        };
        
        private static readonly Point[] Path5 =
        {
            new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(3, 2), new Point(4, 2),
            new Point(5, 2), new Point(6, 2), new Point(7, 2), new Point(7, 3), new Point(8, 3), new Point(8, 4), 
            new Point(9, 4), new Point(10,4), new Point(11,4), new Point(12,4), new Point(12,3), new Point(12,2),
            new Point(13,2), new Point(14,2), new Point(14,3), new Point(15,3), new Point(15,4), new Point(15,5),
            new Point(16,5), new Point(16,6), new Point(16,7), new Point(17,7),
        };

        
        public Field ClassicField
        {
            get
            {
                if (_classicField != null)
                {
                    _classicField.StaticData.EndTimeUtc = DateTime.UtcNow.AddMinutes(BattleDurationMinutes);
                    return _classicField;
                }

                var cells = new FieldCell[Cells2.GetLength(0), Cells2.GetLength(1)];

                for (int i = 0; i < Cells2.GetLength(0); i++)
                {
                    for (int j = 0; j < Cells2.GetLength(1); j++)
                    {
                        cells[i,j] = new FieldCell
                        {
                            Position = new Point(i,j),
                            Object = (FieldObject)Cells2[i,j]
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
                        MonsterMoney = 120,
                        TowerMoney = 120,
                    },
                    StaticData =
                    {
                        EndTimeUtc = DateTime.UtcNow.AddMinutes(BattleDurationMinutes),
                        Path = new[]
                        {
                            new Path(Path1.Reverse().ToArray()), new Path(Path2.Reverse().ToArray()), new Path(Path3.Reverse().ToArray()), new Path(Path4.Reverse().ToArray()), new Path(Path5.Reverse().ToArray()),
                        }
                    }
                };
        
                return _classicField;
            }
        }

        public Field GenerateNewField(int width, int height, Point startPoint, Point endPoint)
        {
            int roadCount = width * height / FieldRoadCoeff;
            var map = CalcWave(width, height, startPoint);

            return new Field(new FieldCell[2, 2]);
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

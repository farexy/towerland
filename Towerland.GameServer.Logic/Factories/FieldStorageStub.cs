using System;
using System.Linq;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Logic.SpecialAI;
using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Logic.Factories
{
  public class FieldStorageStub : IFieldStorage
  {
    private static readonly Lazy<Field>[] Fields = {
        new Lazy<Field>(() => Create(new[,]{
            {1,1,1,1,1,1,1,1,6,1,1,1,1,1,1,1,1,1},
            {3,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,1,1},
            {1,1,0,1,1,1,1,0,0,0,0,0,0,0,1,0,1,1},
            {1,1,0,0,0,0,1,1,0,1,1,1,1,1,1,0,1,1},
            {1,1,0,1,1,0,1,1,0,1,1,1,1,1,1,0,1,1},
            {1,1,0,1,1,0,0,0,0,1,1,1,1,1,1,0,1,1},
            {1,1,0,1,1,1,1,0,6,1,1,1,1,1,1,0,1,1},
            {1,1,0,0,1,1,1,0,0,0,0,0,1,1,1,0,0,1},
            {1,1,1,0,0,0,0,0,1,1,1,0,1,1,1,1,0,1},
            {1,1,1,1,0,1,1,0,1,1,1,0,1,1,1,1,0,1},
            {1,1,1,1,0,1,1,0,1,1,1,0,1,1,1,1,0,1},
            {1,1,1,1,0,1,1,0,1,1,1,0,0,0,0,0,0,1},
            {1,1,0,0,0,1,1,0,1,1,1,1,1,1,0,1,1,1},
            {1,1,0,1,1,1,1,0,1,1,1,1,1,1,0,1,1,6},
            {1,1,0,0,1,1,1,0,1,1,1,1,1,1,0,1,6,1},
            {1,1,1,0,0,0,1,0,0,0,0,0,0,0,0,1,1,1},
            {1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1},
            {6,6,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1}
        },
            FieldTheme.SunnyGlade,
        new[]
        {
            new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(1, 4), new Point(1, 5),
            new Point(1, 6), new Point(1, 7), new Point(2, 7), new Point(2, 8), new Point(2, 9), new Point(2, 10),
            new Point(2, 11), new Point(2, 12), new Point(2, 13), new Point(1, 13), new Point(1, 14), new Point(1, 15),
            new Point(2, 15), new Point(3, 15), new Point(4, 15), new Point(5, 15), new Point(6, 15), new Point(7, 15),
            new Point(7, 16), new Point(8, 16), new Point(9, 16), new Point(10, 16), new Point(11, 16),
            new Point(11, 15), new Point(11, 14), new Point(12, 14), new Point(13, 14), new Point(14, 14), new Point(15, 14),
            new Point(15, 13), new Point(15, 12), new Point(15, 11), new Point(15, 10), new Point(15, 9), new Point(15, 8),
            new Point(15, 7), new Point(16, 7), new Point(17, 7),
        }.Reverse().ToArray(),
        new[]
        {
            new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(1, 4), new Point(1, 5),
            new Point(1, 6), new Point(1, 7), new Point(2, 7), new Point(2, 8), new Point(3, 8), new Point(4, 8),
            new Point(5, 8), new Point(5, 7), new Point(6, 7), new Point(7, 7), new Point(7, 8), new Point(7, 9),
            new Point(7, 10), new Point(7, 11), new Point(8, 11), new Point(9, 11), new Point(10, 11),
            new Point(11, 11), new Point(11, 12), new Point(11, 13), new Point(11, 14), new Point(12, 14), new Point(13, 14),
            new Point(14, 14), new Point(15, 14), new Point(15, 13), new Point(15, 12), new Point(15, 11), new Point(15, 10),
            new Point(15, 9), new Point(15, 8), new Point(15, 7), new Point(16, 7), new Point(17, 7),
        }.Reverse().ToArray(),
        new[]
        {
            new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(1, 4), new Point(1, 5),
            new Point(1, 6), new Point(1, 7), new Point(2, 7), new Point(2, 8), new Point(3, 8), new Point(4, 8),
            new Point(5, 8), new Point(5, 7), new Point(6, 7), new Point(7, 7), new Point(8, 7), new Point(9, 7),
            new Point(10, 7), new Point(11, 7), new Point(12, 7), new Point(13, 7), new Point(14, 7), new Point(15, 7),
            new Point(16, 7), new Point(17, 7),
        }.Reverse().ToArray(),
        new[]
        {
            new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(3, 2), new Point(3, 3),
            new Point(3, 4), new Point(3, 5), new Point(4, 5), new Point(5, 5), new Point(5, 6), new Point(5, 7),
            new Point(6, 7), new Point(7, 7), new Point(8, 7), new Point(9, 7), new Point(10, 7), new Point(11, 7),
            new Point(12, 7), new Point(13, 7), new Point(14, 7), new Point(15, 7), new Point(16, 7), new Point(17, 7),
        }.Reverse().ToArray(),
        new[]
        {
            new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(3, 2), new Point(4, 2),
            new Point(5, 2), new Point(6, 2), new Point(7, 2), new Point(7, 3), new Point(8, 3), new Point(8, 4),
            new Point(9, 4), new Point(10, 4), new Point(11, 4), new Point(12, 4), new Point(12, 3), new Point(12, 2),
            new Point(13, 2), new Point(14, 2), new Point(14, 3), new Point(15, 3), new Point(15, 4), new Point(15, 5),
            new Point(16, 5), new Point(16, 6), new Point(16, 7), new Point(17, 7),
        }.Reverse().ToArray()
        ))
    };


        private static Field Create(int[,] cells, FieldTheme theme, params Point[][] paths)
        {
            var field = new FieldFactory().Create(cells, theme);
            var pathFinder = new PathFinder(field.StaticData);

            foreach (var path in paths)
            {
                pathFinder.AddPath(path);
            }

            return field;
        }

        public Field Get(int index)
        {
            var field = Fields[index].Value;
            field.StaticData.EndTimeUtc = DateTime.UtcNow.AddMinutes(FieldStaticData.BattleDurationMinutes);
            return field;
        }

        public Field GetRandom()
        {
           return Get(GameMath.Rand.Next(Fields.Length));
        }
  }
}
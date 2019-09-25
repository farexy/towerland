using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Logic.Factories;
using Towerland.GameServer.Logic.Selectors;
using Towerland.GameServer.Models.GameField;
using Xunit;

namespace Towerland.GameServer.Logic.UnitTests
{
    public class FieldTest
    {
        [Fact]
        public void TestClassicField()
        {
            var field = new FieldStorageStub().Get(0);
            
            Assert.NotEmpty(field.StaticData.Path);
            Assert.NotEmpty(field.StaticData.Cells);

            foreach (var path in field.StaticData.Path)
            {
                CheckPath(path, field);
            }
        }
        
        [Fact]
        public void TestFieldFactoryFactory()
        {
            var field = new FieldFactory()
            .Create(new[,]
            {
                {1,2,1,1,1},
                {1,0,0,0,1},
                {1,0,1,0,1},
                {1,0,1,0,1},
                {1,3,0,0,1},
            }, FieldTheme.SunnyGlade);
            var pathFinder = new PathFinder(field.StaticData);
            pathFinder.AddPath(new List<Point>{field.StaticData.Start, field.StaticData.Finish});
            pathFinder.AddPath(new List<Point>{field.StaticData.Start, new Point(3, 3), field.StaticData.Finish});
            
            Assert.NotEmpty(field.StaticData.Path);
            Assert.NotEmpty(field.StaticData.Cells);

            foreach (var path in field.StaticData.Path)
            {
                CheckPath(path, field);
            }
        }

        private void CheckPath(Path path, Field field)
        {
            Assert.Equal(path.First(), field.StaticData.Start);
            Assert.Equal(path.Last(), field.StaticData.Finish);
            foreach (var point in path)
            {
                if (point != field.StaticData.Finish)
                {
                    var next = path.GetNext(point);
                    Assert.Equal(1, (int)GameMath.Distance(point, next));
                }
            }
        }
    }
}
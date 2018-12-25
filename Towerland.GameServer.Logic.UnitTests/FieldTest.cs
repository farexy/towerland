using System.Linq;
using Towerland.GameServer.Logic.Factories;
using Towerland.GameServer.Models.GameField;
using Xunit;

namespace Towerland.GameServer.Logic.UnitTests
{
    public class FieldTest
    {
        [Fact]
        public void TestFieldFactory()
        {
            var field = new FieldFactoryStub().ClassicField;
            
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
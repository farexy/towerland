using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IFieldFactory
  {
    Field ClassicField { get; }
    Field Create(int[,] map);
    Field GenerateNewField(int width, int height, Point startPoint, Point endPoint);
  }
}

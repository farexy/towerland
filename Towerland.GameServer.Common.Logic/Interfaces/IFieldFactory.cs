using Towerland.GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IFieldFactory
  {
    Field ClassicField { get; }
    Field GenerateNewField(int width, int height, Point startPoint, Point endPoint);
  }
}

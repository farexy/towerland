using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IFieldStorage
  {
    Field Get(int index);
    Field GetRandom();
  }
}

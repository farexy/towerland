using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IStateChangeRecalculator
  {
    void AddMoney(Field field, int money);
    void AddNewUnit(Field field, GameObjectType type, CreationOptions? opt);
    void AddNewTower(Field field, GameObjectType type, CreationOptions? opt);
  }
}

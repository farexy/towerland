using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.State;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IStateChangeRecalculator
  {
    void AddMoney(Field field, int money, PlayerSide side);
    void AddNewUnit(Field field, GameObjectType type, CreationOptions? opt);
    void AddNewTower(Field field, GameObjectType type, CreationOptions? opt);
  }
}

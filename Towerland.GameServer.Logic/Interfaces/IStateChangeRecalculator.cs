using System.Collections.Generic;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IStateChangeRecalculator
  {
    List<GameAction> AddMoney(Field field, int money, PlayerSide side);
    List<GameAction> AddNewUnit(Field field, GameObjectType type, UnitCreationOption opt = null);
    List<GameAction> AddNewTower(Field field, GameObjectType type, TowerCreationOption opt);
  }
}

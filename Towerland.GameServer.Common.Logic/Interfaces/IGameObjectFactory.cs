using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IGameObjectFactory<out T> where T : GameObject
  {
    T Create(GameObjectType type, CreationOptions? options);
  }
}

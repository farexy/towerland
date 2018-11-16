using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IGameObjectFactory<out T> where T : GameObject
  {
    T Create(GameObjectType type, CreationOptions? options);
  }
}

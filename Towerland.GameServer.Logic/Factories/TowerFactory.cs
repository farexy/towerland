using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Factories
{
  public class TowerFactory : IGameObjectFactory<Tower>
  {
    private readonly IStatsLibrary _statsLib;

    public TowerFactory(IStatsLibrary stats)
    {
      _statsLib = stats;
    }

    public Tower Create(GameObjectType type, CreationOptions? options = null)
    {
      return new Tower
      {
        GameId = options?.GameId ?? default,
        Type = type,
        Position = options?.Position ?? default,
      };
    }
  }
}

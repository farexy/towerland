using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Factories
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
        Type = type,
        Position = options.HasValue ? options.Value.Position : default(Point),
      };
    }
  }
}

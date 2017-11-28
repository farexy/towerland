using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic
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

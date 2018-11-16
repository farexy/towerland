using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Factories
{
  public class UnitFactory : IGameObjectFactory<Unit>
  {
    private readonly IStatsLibrary _statsLib;

    public UnitFactory(IStatsLibrary stats)
    {
      _statsLib = stats;
    }

    public Unit Create(GameObjectType type, CreationOptions? options = null)
    {
      return new Unit
      {
        GameId = options?.GameId ?? default,
        Type = type,
        PathId = options?.PathId,
        Health = _statsLib.GetUnitStats(type).Health,
        Position = options?.Position ?? new Point()
      };
    }
  }
}

using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Factories
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

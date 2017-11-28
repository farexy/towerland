﻿using GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic
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
        Type = type,
        PathId = options.HasValue ? options.Value.PathId : default(int?),
        Health = _statsLib.GetUnitStats(type).Health
      };
    }
  }
}

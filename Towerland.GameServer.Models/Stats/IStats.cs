﻿using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Models.Stats
{
  public interface IStats
  {
    GameObjectType Type { get; set; }
  }
}
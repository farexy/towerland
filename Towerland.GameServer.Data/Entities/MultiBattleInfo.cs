using System;
using System.Collections.Generic;

namespace Towerland.GameServer.Data.Entities
{
  public class MultiBattleInfo
  {
    public const int MaxUserOnSide = 5;

    public List<Guid> MonsterPlayers { get; set; }
    public List<Guid> TowerPlayers { get; set; }
  }
}
using System;
using System.Collections.Generic;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Domain.Models
{
  public class LiveBattleModel
  {
    public Guid Id { set; get; }
    public Field State { set; get; }
    public IEnumerable<GameTick> Ticks { set; get; }
  }
}
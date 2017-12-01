using System;
using System.Collections.Generic;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Domain.Models
{
  public class LiveBattleModel
  {
    public Guid Id { set; get; }
    public Field State { set; get; }
    public IEnumerable<IEnumerable<GameAction>> Actions { set; get; }
  }
}
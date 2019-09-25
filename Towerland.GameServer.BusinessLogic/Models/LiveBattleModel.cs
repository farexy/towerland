using System;
using System.Collections.Generic;
using Towerland.GameServer.Data.Entities;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.BusinessLogic.Models
{
  public class LiveBattleModel
  {
    public Guid Id { set; get; }
    public Field State { set; get; }
    public List<GameTick> TicksHistory { set; get; }
    public IEnumerable<GameTick> Ticks { set; get; }
    public MultiBattleInfo MultiBattleInfo { set; get; }
  }
}
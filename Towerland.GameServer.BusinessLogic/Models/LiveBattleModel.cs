using System;
using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Data.Entities;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.BusinessLogic.Models
{
  public class LiveBattleModel
  {
    public Guid Id { set; get; }
    public Field State { set; get; }
    public List<GameTick> TicksHistory { set; get; }
    public IEnumerable<GameTick> Ticks { set; get; }
    public MultiBattleInfo MultiBattleInfo { set; get; }
    public GameMode Mode { get; set; }
    public PlayerSide CompPlayerSide { get; set; }

    public LiveBattleModel CreateCopy()
    {
      return new LiveBattleModel
      {
        Id = Id,
        State = State.Clone() as Field,
        TicksHistory = new List<GameTick>(),
        Ticks = Ticks.ToList(),
        MultiBattleInfo = null
      };
    }
  }
}
using System;
using GameServer.Common.Models.State;

namespace GameServer.Api.Models
{
  public class BattleSearchCheckResponseModel
  {
    public bool Found { get; set; }
    public Guid BattleId { get; set; }
    public PlayerSide Side { get; set; }
  }
}
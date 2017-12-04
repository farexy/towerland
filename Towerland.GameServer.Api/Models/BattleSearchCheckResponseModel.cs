using System;

namespace GameServer.Api.Models
{
  public class BattleSearchCheckResponseModel
  {
    public bool Found { get; set; }
    public Guid BattleId { get; set; }
  }
}
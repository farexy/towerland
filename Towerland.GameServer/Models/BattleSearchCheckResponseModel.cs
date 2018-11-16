using System;
using Newtonsoft.Json;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.Models
{
  public class BattleSearchCheckResponseModel
  {
    [JsonProperty("found")] public bool Found { get; set; }
    [JsonProperty("battleId")] public Guid BattleId { get; set; }
    [JsonProperty("side")] public PlayerSide Side { get; set; }
  }
}
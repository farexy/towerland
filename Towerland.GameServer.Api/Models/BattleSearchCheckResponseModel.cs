using System;
using GameServer.Common.Models.State;
using Newtonsoft.Json;

namespace GameServer.Api.Models
{
  public class BattleSearchCheckResponseModel
  {
    [JsonProperty("found")] public bool Found { get; set; }
    [JsonProperty("battleId")] public Guid BattleId { get; set; }
    [JsonProperty("side")] public PlayerSide Side { get; set; }
  }
}
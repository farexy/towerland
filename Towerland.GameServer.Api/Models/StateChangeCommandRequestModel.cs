using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.State;

namespace Towerland.GameServer.Api.Models
{
  public class StateChangeCommandRequestModel
  {
    [JsonProperty("battleId")] public Guid BattleId { set; get; }
    [JsonProperty("unitCreationOptions")] public IEnumerable<UnitCreationOption> UnitCreationOptions { set; get; }
    [JsonProperty("towerCreationOptions")] public IEnumerable<TowerCreationOption> TowerCreationOptions { set; get; }
    [JsonProperty("money")] public int Money { get; set; }
    
    [JsonProperty("currentTick")] public int CurrentTick { set; get; }
  }
}
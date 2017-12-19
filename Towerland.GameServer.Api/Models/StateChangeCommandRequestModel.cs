using System;
using System.Collections.Generic;
using GameServer.Common.Models.State;
using Newtonsoft.Json;

namespace GameServer.Api.Models
{
  public class StateChangeCommandRequestModel
  {
    [JsonProperty("id")] public CommandId Id { set; get; }
    [JsonProperty("battleId")] public Guid BattleId { set; get; }
    [JsonProperty("unitCreationOptions")] public IEnumerable<UnitCreationOption> UnitCreationOptions { set; get; }
    [JsonProperty("towerCreationOptions")] public IEnumerable<TowerCreationOption> TowerCreationOptions { set; get; }
    [JsonProperty("money")] public int Money { get; set; }
    
    [JsonProperty("currentTick")] public int CurrentTick { set; get; }
  }
}
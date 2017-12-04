using System;
using System.Collections.Generic;
using GameServer.Common.Models.State;
using Newtonsoft.Json;

namespace GameServer.Api.Models
{
  public class StateChangeCommandRequestModel
  {
    [JsonProperty("i")] public CommandId Id { set; get; }
    [JsonProperty("b")] public Guid BattleId { set; get; }
    [JsonProperty("u")] public IEnumerable<UnitCreationOption> UnitCreationOptions { set; get; }
    [JsonProperty("t")] public IEnumerable<TowerCreationOption> TowerCreationOptions { set; get; }
    
    public int CurrentTick { set; get; }
  }
}
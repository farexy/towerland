using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameServer.Common.Models.State
{
  public class StateChangeCommand
  {
    [JsonProperty("Id")] public CommandId Id { set; get; }
    [JsonProperty("BattleId")] public Guid BattleId { set; get; }
    [JsonProperty("UnitCreationOptions")] public IEnumerable<UnitCreationOption> UnitCreationOptions { set; get; }
    [JsonProperty("TowerCreationOptions")] public IEnumerable<TowerCreationOption> TowerCreationOptions { set; get; }
  }
}

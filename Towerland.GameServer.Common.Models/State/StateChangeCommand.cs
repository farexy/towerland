using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameServer.Common.Models.State
{
  public class StateChangeCommand
  {
    [JsonProperty("i")] public CommandId Id { set; get; }
    [JsonProperty("b")] public Guid BattleId { set; get; }
    [JsonProperty("u")] public IEnumerable<UnitCreationOption> UnitCreationOptions { set; get; }
    [JsonProperty("t")] public IEnumerable<TowerCreationOption> TowerCreationOptions { set; get; }
  }
}

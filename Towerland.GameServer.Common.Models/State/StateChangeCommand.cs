using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Towerland.GameServer.Common.Models.State
{
  public class StateChangeCommand
  {
    [JsonProperty("b")] public Guid BattleId { set; get; }
    [JsonProperty("u")] public IEnumerable<UnitCreationOption> UnitCreationOptions { set; get; }
    [JsonProperty("t")] public IEnumerable<TowerCreationOption> TowerCreationOptions { set; get; }
    [JsonProperty("c")] public string CheatCommand { get; set; }
  }
}

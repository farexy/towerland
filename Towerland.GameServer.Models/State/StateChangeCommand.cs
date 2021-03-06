﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Towerland.GameServer.Models.State
{
  public class StateChangeCommand
  {
    [JsonProperty("b")] public Guid BattleId { set; get; }
    [JsonProperty("u")] public IEnumerable<UnitCreationOption> UnitCreationOptions { set; get; }
    [JsonProperty("t")] public IEnumerable<TowerCreationOption> TowerCreationOptions { set; get; }
    [JsonProperty("c")] public string CheatCommand { get; set; }

    [JsonIgnore]
    public bool IsEmpty =>
        UnitCreationOptions is null && TowerCreationOptions is null && string.IsNullOrEmpty(CheatCommand);
  }
}

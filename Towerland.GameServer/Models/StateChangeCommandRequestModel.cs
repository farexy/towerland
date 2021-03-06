﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.Models
{
  public class StateChangeCommandRequestModel
  {
    [JsonProperty("battleId")] public Guid BattleId { set; get; }
    [JsonProperty("unitCreationOptions")] public IEnumerable<UnitCreationOption> UnitCreationOptions { set; get; }
    [JsonProperty("towerCreationOptions")] public IEnumerable<TowerCreationOption> TowerCreationOptions { set; get; }
    [JsonProperty("command")] public string CheatCommand { get; set; }
    
    [JsonProperty("currentTick")] public int CurrentTick { set; get; }
  }
}
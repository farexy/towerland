﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Towerland.GameServer.Common.Models.GameActions
{
  public class GameTick
  {
    [JsonProperty("t")] public int RelativeTime { get; set; }
    [JsonProperty("a")] public IEnumerable<GameAction> Actions { get; set; }

    [JsonIgnore]
    public bool HasNoActions
    {
      get { return Actions == null || !Actions.Any(); }
    }
  }
}

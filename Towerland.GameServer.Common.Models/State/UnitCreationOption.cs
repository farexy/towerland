﻿using GameServer.Common.Models.GameObjects;
using Newtonsoft.Json;

namespace GameServer.Common.Models.State
{
  public class UnitCreationOption
  {
    [JsonProperty("type")] public GameObjectType Type { set; get; }
  }
}


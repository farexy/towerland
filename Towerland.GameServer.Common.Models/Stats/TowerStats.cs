﻿using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Models.Stats
{
  public struct TowerStats : IStats
  {
    [JsonProperty("t")] public GameObjectType Type { set; get; }
    [JsonProperty("d")] public int Damage { set; get; }
    [JsonProperty("r")] public int Range { set; get; }
    [JsonProperty("s")] public int AttackSpeed { set; get; }
    [JsonProperty("a")] public AttackType Attack { set; get; }
    [JsonProperty("tp")] public AttackPriority TargetPriority { set; get; }
    [JsonProperty("e")] public SpecialEffect[] SpecialEffects { set; get; }
    [JsonProperty("c")] public int Cost { set; get; }
    [JsonProperty("p")] public StrokePriority Priority { set; get; }

    public enum AttackType
    {
      Usual,
      Magic,
      Burst
    }

    public enum AttackPriority
    {
      Random,
      Optimal
    }
  }
}

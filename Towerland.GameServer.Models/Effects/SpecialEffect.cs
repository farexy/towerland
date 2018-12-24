using System.Collections.Generic;
using Newtonsoft.Json;

namespace Towerland.GameServer.Models.Effects
{
  public class SpecialEffect
  {
    [JsonProperty("i")] public EffectId Id { set; get; }
    [JsonProperty("d")] public int Duration { set; get; }
    [JsonProperty("e")] public double EffectValue { get; set; }

    public static SpecialEffect Empty { get; } = new SpecialEffect{Id = EffectId.None, Duration = 0};
  }
}

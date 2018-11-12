using System.Collections.Generic;
using Newtonsoft.Json;

namespace Towerland.GameServer.Common.Models.Effects
{
  public class SpecialEffect
  {
    [JsonProperty("i")] public EffectId Id { set; get; }
    [JsonProperty("d")] public int Duration { set; get; }

    public static SpecialEffect Empty { get; } = new SpecialEffect{Id = EffectId.None, Duration = 0};

    public static IReadOnlyDictionary<EffectId, double> EffectDebuffValue = new Dictionary<EffectId, double>()
    {
      [EffectId.UnitFreezed] = 2, // speed slow
      [EffectId.UnitPoisoned] = 0.05, // percents of health
    };
  }
}

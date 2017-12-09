using Newtonsoft.Json;

namespace GameServer.Common.Models.Effects
{
  public class SpecialEffect
  {
    public const int FreezedSlowCoeff = 2;
    
    [JsonProperty("Effect")] public EffectId Effect { set; get; }
    [JsonProperty("Duration")] public int Duration { set; get; }

    public static SpecialEffect Empty { get; } = new SpecialEffect{Effect = EffectId.None, Duration = 0};
  }
}

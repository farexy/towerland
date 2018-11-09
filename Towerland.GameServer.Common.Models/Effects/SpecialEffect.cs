using Newtonsoft.Json;

namespace Towerland.GameServer.Common.Models.Effects
{
  public class SpecialEffect
  {
    [JsonProperty("i")] public EffectId Id { set; get; }
    [JsonProperty("d")] public int Duration { set; get; }

    public static SpecialEffect Empty { get; } = new SpecialEffect{Id = EffectId.None, Duration = 0};
  }
}

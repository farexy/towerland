using Newtonsoft.Json;

namespace GameServer.Common.Models.Effects
{
  public struct SpecialEffect
  {
    [JsonProperty("i")] public EffectId Effect { set; get; }
    [JsonProperty("d")] public int Duration { set; get; } 


  }
}

using GameServer.Common.Models.Effects;
using GameServer.Common.Models.GameObjects;
using Newtonsoft.Json;

namespace GameServer.Common.Models.Stats
{
  public struct TowerStats : IStats
  {
    [JsonProperty("Type")] public GameObjectType Type { set; get; }
    [JsonProperty("Damage")] public int Damage { set; get; }
    [JsonProperty("Range")] public int Range { set; get; }
    [JsonProperty("AttackSpeed")] public int AttackSpeed { set; get; }
    [JsonProperty("Attack")] public AttackType Attack { set; get; }
    [JsonProperty("SpecialEffects")] public SpecialEffect[] SpecialEffects { set; get; }
    [JsonProperty("Cost")] public int Cost { set; get; }

    public enum AttackType
    {
      Usual,
      Magic,
      Burst
    }
  }
}

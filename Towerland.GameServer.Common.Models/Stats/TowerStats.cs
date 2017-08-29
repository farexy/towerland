using GameServer.Common.Models.Effects;
using GameServer.Common.Models.GameObjects;
using Newtonsoft.Json;

namespace GameServer.Common.Models.Stats
{
  public struct TowerStats
  {
    [JsonProperty("t")] public GameObjectType Type { set; get; }
    [JsonProperty("d")] public int Damage { set; get; }
    [JsonProperty("r")] public int Range { set; get; }
    [JsonProperty("s")] public int AttackSpeed { set; get; }
    [JsonProperty("a")] public AttackType Attack { set; get; }
    [JsonProperty("e")] public SpecialEffect[] SpecialEffects { set; get; }

    public enum AttackType
    {
      Usual,
      Magic,
      Freezing,
      Burst
    }
  }
}

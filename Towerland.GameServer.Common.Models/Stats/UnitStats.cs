using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Models.Stats
{
  public struct UnitStats : IStats
  {
    [JsonProperty("t")] public GameObjectType Type { set; get; }
    [JsonProperty("h")] public int Health { set; get; }
    [JsonProperty("d")] public int Damage { set; get; }
    [JsonProperty("s")] public int Speed { set; get; } // ticks per cell
    [JsonProperty("m")] public MovementPriorityType MovementPriority { set; get; }
    [JsonProperty("a")] public bool IsAir { set; get; }
    [JsonProperty("e")] public SpecialEffect[] SpecialEffects { set; get; }
    [JsonProperty("c")] public int Cost { set; get; }
    [JsonProperty("f")] public DefenceType Defence { set; get; }
    [JsonProperty("p")] public StrokePriority Priority { set; get; }

    public enum MovementPriorityType
    {
      Fastest,
      Optimal,
      Random,
    }

    public enum DefenceType
    {
      Undefended,
      LightArmor,
      HeavyArmor
    }
  }
}

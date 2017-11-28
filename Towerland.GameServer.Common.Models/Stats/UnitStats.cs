using GameServer.Common.Models.Effects;
using GameServer.Common.Models.GameObjects;
using Newtonsoft.Json;

namespace GameServer.Common.Models.Stats
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

    public enum MovementPriorityType
    {
      Fastest,
      Optimal,
      Random,
    }
  }
}

using GameServer.Common.Models.Effects;
using GameServer.Common.Models.GameObjects;
using Newtonsoft.Json;

namespace GameServer.Common.Models.Stats
{
  public struct UnitStats : IStats
  {
    [JsonProperty("Type")] public GameObjectType Type { set; get; }
    [JsonProperty("Health")] public int Health { set; get; }
    [JsonProperty("Damage")] public int Damage { set; get; }
    [JsonProperty("Speed")] public int Speed { set; get; } // ticks per cell
    [JsonProperty("MovementPriority")] public MovementPriorityType MovementPriority { set; get; }
    [JsonProperty("IsAir")] public bool IsAir { set; get; }
    [JsonProperty("SpecialEffects")] public SpecialEffect[] SpecialEffects { set; get; }
    [JsonProperty("Cost")] public int Cost { set; get; }
    [JsonProperty("Defence")] public DefenceType Defence { set; get; }

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

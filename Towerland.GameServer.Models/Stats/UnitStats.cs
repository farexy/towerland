using Newtonsoft.Json;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Models.Stats
{
  public struct UnitStats : IStats
  {
    [JsonProperty("t")] public GameObjectType Type { set; get; }
    [JsonProperty("h")] public int Health { set; get; }
    [JsonProperty("d")] public int Damage { set; get; }
    [JsonProperty("s")] public int Speed { set; get; } // ticks per cell
    [JsonProperty("m")] public MovementPriorityType MovementPriority { set; get; }
    [JsonProperty("e")] public SkillId Skill { set; get; }
    [JsonProperty("c")] public int Cost { set; get; }
    [JsonProperty("f")] public DefenceType Defence { set; get; }
    [JsonProperty("i")] public bool Hidden { get; set; }

    [JsonIgnore] public bool IsAir => Skill == SkillId.AirUnit;

    public enum MovementPriorityType
    {
      Fastest,
      Optimal,
      Random,
      ByUser,
    }

    public enum DefenceType
    {
      Undefended,
      LightArmor,
      HeavyArmor
    }
  }
}

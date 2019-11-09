using System;
using Newtonsoft.Json;

namespace Towerland.GameServer.Models.Effects
{
  public class SpecialEffect : ICloneable
  {
    [JsonProperty("i")] public EffectId Id { set; get; }
    [JsonProperty("d")] public int Duration { set; get; }
    [JsonProperty("e")] public double EffectValue { get; set; }

    public static SpecialEffect Empty { get; } = new SpecialEffect{Id = EffectId.None, Duration = 0};

    [JsonIgnore] public EffectType Type => GetType(Id);

    public bool IsEmpty() => Id is EffectId.None;

    public static EffectType GetType(EffectId effectId)
    {
      switch (effectId)
      {
        case EffectId.None:
          return EffectType.None;
        case EffectId.SkillsDisabled:
          return EffectType.SkillDebuff;
        case EffectId.UnitFreezed:
          return EffectType.SpeedDebuff;
        case EffectId.UnitPoisoned:
          return EffectType.ConstantDamageDebuff;
        default:
          throw new ArgumentOutOfRangeException(nameof(effectId), effectId, null);
      }
    }
    
    public enum EffectType
    {
      None,
      SpeedBuff,
      SpeedDebuff,
      ConstantHealBuff,
      ConstantDamageDebuff,
      SkillDebuff
    }
    
    public object Clone()
    {
      return new SpecialEffect
      {
        Id = Id,
        Duration = Duration,
        EffectValue = EffectValue
      };
    }
  }
}

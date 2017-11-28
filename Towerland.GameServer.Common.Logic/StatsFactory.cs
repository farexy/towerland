using GameServer.Common.Models.Effects;
using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic
{
  public class StatsFactory
  {
    public UnitStats[] Units =
    {
      new UnitStats
      {
        Type = GameObjectType.Unit_Skeleton,
        Damage = 20,
        Health = 100,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Random,
        Speed = 4,
        Cost = 50
      }
    };

    public TowerStats[] Towers =
    {
      new TowerStats
      {
        Type = GameObjectType.Tower_Usual,
        Attack = TowerStats.AttackType.Usual,
        AttackSpeed = 8,
        Damage = 10,
        Range = 3,
        Cost = 50
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_Frost,
        Attack = TowerStats.AttackType.Magic,
        AttackSpeed = 3,
        Damage = 4,
        Range = 4,
        Cost = 120,
        SpecialEffects = new []{new SpecialEffect{Effect = EffectId.UnitFreezed, Duration = 10}}
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_Cannon,
        Attack = TowerStats.AttackType.Burst,
        AttackSpeed = 15,
        Damage = 20,
        Range = 5,
        Cost = 200
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_FortressWatchtower,
        Attack = TowerStats.AttackType.Usual,
        
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_Magic,
        Attack = TowerStats.AttackType.Magic,
        AttackSpeed = 6,
        Damage = 20,
        Range = 6,
        Cost = 300,
        SpecialEffects = new []{new SpecialEffect{Effect = EffectId.Unit10xDamage_10PercentProbability}}
      }
    };
  }
}
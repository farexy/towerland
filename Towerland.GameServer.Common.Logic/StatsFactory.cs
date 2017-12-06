using GameServer.Common.Models.Effects;
using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic
{
  public class StatsFactory
  {
    public readonly UnitStats[] Units =
    {
      new UnitStats
      {
        Type = GameObjectType.Unit_Skeleton,
        Damage = 20,
        Health = 100,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Random,
        Speed = 4,
        Cost = 50,
        Defence = UnitStats.DefenceType.LightArmor
      },
      new UnitStats
      {
        Type = GameObjectType.Unit_Orc,
        Damage = 30,
        Health = 100,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Random,
        Speed = 3,
        Cost = 120,
        Defence = UnitStats.DefenceType.HeavyArmor
      },
    };

    public readonly TowerStats[] Towers =
    {
      new TowerStats
      {
        Type = GameObjectType.Tower_Usual,
        Attack = TowerStats.AttackType.Usual,
        AttackSpeed = 8,
        Damage = 10,
        Range = 2,
        Cost = 50
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_Frost,
        Attack = TowerStats.AttackType.Magic,
        AttackSpeed = 3,
        Damage = 4,
        Range = 3,
        Cost = 120,
        SpecialEffects = new []{new SpecialEffect{Effect = EffectId.UnitFreezed, Duration = 16}}
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_Cannon,
        Attack = TowerStats.AttackType.Burst,
        AttackSpeed = 15,
        Damage = 20,
        Range = 4,
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
        Cost = 400,
        SpecialEffects = new []{new SpecialEffect{Effect = EffectId.Unit10xDamage_10PercentProbability}}
      }
    };

    public readonly DefenceCoeff[] DefenceCoeffs =
    {
      new DefenceCoeff
      {
        Defence = UnitStats.DefenceType.Undefended,
        Attack = TowerStats.AttackType.Usual,
        Coeff = 0.9
      },
      new DefenceCoeff
      {
        Defence = UnitStats.DefenceType.Undefended,
        Attack = TowerStats.AttackType.Burst,
        Coeff = 1
      },
      new DefenceCoeff
      {
        Defence = UnitStats.DefenceType.Undefended,
        Attack = TowerStats.AttackType.Magic,
        Coeff = 0.8
      },
      new DefenceCoeff
      {
        Defence = UnitStats.DefenceType.LightArmor,
        Attack = TowerStats.AttackType.Usual,
        Coeff = 0.7
      },
      new DefenceCoeff
      {
        Defence = UnitStats.DefenceType.LightArmor,
        Attack = TowerStats.AttackType.Burst,
        Coeff = 0.9
      },
      new DefenceCoeff
      {
        Defence = UnitStats.DefenceType.LightArmor,
        Attack = TowerStats.AttackType.Magic,
        Coeff = 0.5
      },
      new DefenceCoeff
      {
        Defence = UnitStats.DefenceType.HeavyArmor,
        Attack = TowerStats.AttackType.Usual,
        Coeff = 0.7
      },
      new DefenceCoeff
      {
        Defence = UnitStats.DefenceType.HeavyArmor,
        Attack = TowerStats.AttackType.Burst,
        Coeff = 0.5
      },
      new DefenceCoeff
      {
        Defence = UnitStats.DefenceType.HeavyArmor,
        Attack = TowerStats.AttackType.Magic,
        Coeff = 0.2
      },
    };
  }
}
﻿using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Factories
{
  public class StatsFactory : IStatsProvider
  {
    private UnitStats[] _units =
    {
      new UnitStats
      {
        Type = GameObjectType.Unit_Skeleton,
        Damage = 5,
        Health = 100,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Fastest,
        Speed = 2,
        Cost = 50,
        Defence = UnitStats.DefenceType.LightArmor,
        Priority = StrokePriority.Usual
      },
      new UnitStats
      {
        Type = GameObjectType.Unit_Impling,
        Damage = 10,
        Health = 200,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Random,
        Speed = 4,
        Cost = 120,
        Defence = UnitStats.DefenceType.LightArmor,
        Priority = StrokePriority.Usual
      },
      new UnitStats
      {
        Type = GameObjectType.Unit_Orc,
        Damage = 15,
        Health = 330,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Random,
        Speed = 6,
        Cost = 200,
        Defence = UnitStats.DefenceType.HeavyArmor,
        Priority = StrokePriority.Usual
      },
      new UnitStats
      {
        Type = GameObjectType.Unit_Goblin,
        Damage = 15,
        Health = 350,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Optimal,
        Speed = 3,
        Cost = 250,
        Defence = UnitStats.DefenceType.LightArmor,
        Priority = StrokePriority.Usual
      },
      new UnitStats
      {
        Type = GameObjectType.Unit_Dragon,
        Damage = 25,
        Health = 600,
        IsAir = true,
        MovementPriority = UnitStats.MovementPriorityType.Optimal,
        Speed = 3,
        Cost = 500,
        Defence = UnitStats.DefenceType.Undefended,
        Priority = StrokePriority.Usual
      },
      new UnitStats
      {
        Type = GameObjectType.Unit_Golem,
        Damage = 30,
        Health = 700,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Random,
        Speed = 5,
        Cost = 750,
        Defence = UnitStats.DefenceType.HeavyArmor,
        Priority = StrokePriority.Usual
      },
      new UnitStats
      {
        Type = GameObjectType.Unit_Necromancer,
        Damage = 10,
        Health = 400,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Random,
        Speed = 4,
        Cost = 500,
        Defence = UnitStats.DefenceType.Undefended,
        SpecialEffect = new SpecialEffect {Id = EffectId.ReviveDeadUnitsAtPreviousTick},
        Priority = StrokePriority.TheLowest
      }
    };

    private TowerStats[] _towers =
    {
      new TowerStats
      {
        Type = GameObjectType.Tower_Usual,
        TargetPriority = TowerStats.AttackPriority.Random,
        Attack = TowerStats.AttackType.Usual,
        AttackSpeed = 6,
        Damage = 60,
        Range = 3,
        Cost = 50,
        Priority = StrokePriority.Usual
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_Frost,
        TargetPriority = TowerStats.AttackPriority.Random,
        Attack = TowerStats.AttackType.Magic,
        AttackSpeed = 3,
        Damage = 35,
        Range = 4,
        Cost = 120,
        SpecialEffect = new SpecialEffect {Id = EffectId.UnitFreezed, Duration = 16},
        Priority = StrokePriority.Usual
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_Cannon,
        TargetPriority = TowerStats.AttackPriority.Random,
        Attack = TowerStats.AttackType.Burst,
        AttackSpeed = 12,
        Damage = 70,
        Range = 4,
        Cost = 200,
        Priority = StrokePriority.Usual
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_FortressWatchtower,
        TargetPriority = TowerStats.AttackPriority.Optimal,
        Attack = TowerStats.AttackType.Usual,
        AttackSpeed = 8,
        Damage = 100,
        Range = 5,
        Cost = 400,
        Priority = StrokePriority.Usual
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_Magic,
        TargetPriority = TowerStats.AttackPriority.Optimal,
        Attack = TowerStats.AttackType.Magic,
        AttackSpeed = 5,
        Damage = 70,
        Range = 6,
        Cost = 600,
        SpecialEffect = new SpecialEffect {Id = EffectId.Unit10xDamage_10PercentProbability},
        Priority = StrokePriority.Usual
      }
    };

    private DefenceCoeff[] _defenceCoeffs =
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
        Coeff = 0.2
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
        Coeff = 0.6
      },
    };

    public UnitStats[] GetUnitStats() => _units;

    public TowerStats[] GetTowerStats() => _towers;

    public DefenceCoeff[] GetDefenceCoeffs() => _defenceCoeffs;
  }
}
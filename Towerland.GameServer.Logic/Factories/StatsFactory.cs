using System;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Logic.Factories
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
        Defence = UnitStats.DefenceType.LightArmor
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
        Defence = UnitStats.DefenceType.LightArmor
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
        Defence = UnitStats.DefenceType.HeavyArmor
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
        Skill = SkillId.StealsTowerMoney
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
        Defence = UnitStats.DefenceType.Undefended
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
        Defence = UnitStats.DefenceType.HeavyArmor
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
        Skill = SkillId.RevivesDeadUnit
      },
      new UnitStats
      {
        Type = GameObjectType.Unit_BarbarianMage,
        Damage = 15,
        Health = 550,
        IsAir = false,
        MovementPriority = UnitStats.MovementPriorityType.Random,
        Speed = 4,
        Cost = 750,
        Defence = UnitStats.DefenceType.LightArmor,
        Skill= SkillId.DestroysTowerOnDeath
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
        SpawnType = TowerStats.TowerSpawnType.Ground
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
        Skill = SkillId.FreezesUnit,
        SpawnType = TowerStats.TowerSpawnType.Ground
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_Cannon,
        TargetPriority = TowerStats.AttackPriority.UnitsAtPosition,
        Attack = TowerStats.AttackType.Burst,
        AttackSpeed = 12,
        Damage = 70,
        Range = 4,
        Cost = 200,
        SpawnType = TowerStats.TowerSpawnType.Ground
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
        SpawnType = TowerStats.TowerSpawnType.Ground
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
        Skill = SkillId.ExtraDamageUnit,
        SpawnType = TowerStats.TowerSpawnType.Ground
      },
      new TowerStats
      {
        Type = GameObjectType.Tower_Poisoning,
        TargetPriority = TowerStats.AttackPriority.Random,
        Attack = TowerStats.AttackType.Magic,
        AttackSpeed = 5,
        Damage = 40,
        Range = 3,
        Cost = 500,
        Skill = SkillId.PoisonsUnit,
        SpawnType = TowerStats.TowerSpawnType.Ground
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
      }
    };

    private Skill[] _skills =
    {
      new Skill
      {
        Id = SkillId.FreezesUnit,
        GameObjectType = GameObjectType.Tower_Frost,
        EffectId = EffectId.UnitFreezed,
        Duration = 16,
        DebuffValue = 2
      },
      new Skill
      {
        Id = SkillId.PoisonsUnit,
        GameObjectType = GameObjectType.Tower_Poisoning,
        EffectId = EffectId.UnitPoisoned,
        Duration = 20,
        DebuffValue = 0.05,
      },
      new Skill
      {
        Id = SkillId.ExtraDamageUnit,
        GameObjectType = GameObjectType.Tower_Magic,
        ProbabilityPercent = 10,
        BuffValue = 10
      },
      new Skill
      {
        Id = SkillId.RevivesDeadUnit,
        GameObjectType = GameObjectType.Unit_Necromancer,
        WaitTicks = 1
      },
      new Skill
      {
        Id = SkillId.DestroysTowerOnDeath,
        GameObjectType = GameObjectType.Unit_BarbarianMage,
      },
      new Skill
      {
        Id = SkillId.StealsTowerMoney,
        GameObjectType = GameObjectType.Unit_Goblin,
        ProbabilityPercent = 25,
        BuffValue = 10,
        DebuffValue = 10,
        WaitTicks = 3,
      },
      new Skill
      {
        Id = SkillId.BlocksUnitSkills,
        GameObjectType = GameObjectType.Tower_Magic, // TODO
        EffectId = EffectId.SkillsDisabled,
        Duration = 6,
      },
      new Skill
      {
        Id = SkillId.BlocksTowerSkillsInRange,
        GameObjectType = GameObjectType.Unit_Golem, // TODO
        EffectId = EffectId.SkillsDisabled,
        Duration = 6,
        Range = 4
      },
    };

    public UnitStats[] GetUnitStats() => _units;

    public TowerStats[] GetTowerStats() => _towers;

    public DefenceCoeff[] GetDefenceCoeffs() => _defenceCoeffs;

    public Skill[] GetSkills() => _skills;
  }
}
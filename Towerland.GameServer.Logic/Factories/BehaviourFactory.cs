using Towerland.GameServer.Logic.Behaviour;
using Towerland.GameServer.Logic.Behaviour.Towers;
using Towerland.GameServer.Logic.Behaviour.Units;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Logic.Factories
{
  public class BehaviourFactory
  {
    private readonly BattleContext _battleContext;
    private readonly IStatsLibrary _statsLibrary;

    public BehaviourFactory(BattleContext battleContext, IStatsLibrary statsLibrary)
    {
      _battleContext = battleContext;
      _statsLibrary = statsLibrary;
    }

    public IBehaviour CreateUnitBehaviour(Unit unit)
    {
      var skill = unit.Effect.Id == EffectId.SkillsDisabled
        ? SkillId.None
        : _statsLibrary.GetUnitStats(unit.Type).Skill;
      switch (skill)
      {
        case SkillId.None:
          return new BaseUnitBehaviour(unit, _battleContext, _statsLibrary);
        case SkillId.RevivesDeadUnit:
          return new NecromancerBehaviour(unit, _battleContext, _statsLibrary);
        case SkillId.DestroysTowerOnDeath:
          return new BarbarianBehaviour(unit, _battleContext, _statsLibrary);
        case SkillId.StealsTowerMoney:
          return new GoblinBehaviour(unit, _battleContext, _statsLibrary);
        case SkillId.BlocksTowerSkillsInRange:
          return new TowerSkillBlockingBehaviour(unit, _battleContext, _statsLibrary);
        default:
          return new BaseUnitBehaviour(unit, _battleContext, _statsLibrary);
      }
    }

    public IBehaviour CreateTowerBehaviour(Tower tower)
    {
      var stats = _statsLibrary.GetTowerStats(tower.Type);
      var skill = tower.Effect.Id == EffectId.SkillsDisabled ? SkillId.None : stats.Skill;
      switch (skill)
      {
        case SkillId.None:
          switch (stats.Attack)
          {
            case TowerStats.AttackType.Usual:
            case TowerStats.AttackType.Magic:
              return new BaseTowerBehaviour(tower, _battleContext, _statsLibrary);
            case TowerStats.AttackType.Burst:
              return new BurstTowerBehaviour(tower, _battleContext, _statsLibrary);
            default:
              return null;
          }
        case SkillId.FreezesUnit:
        case SkillId.PoisonsUnit:
        case SkillId.BlocksUnitSkills:
          return new DebuffTowerBehaviour(tower, _battleContext, _statsLibrary);
        case SkillId.ExtraDamageUnit:
          return new ExtraDamageTowerBehaviour(tower, _battleContext, _statsLibrary);
        default:
          return new BaseTowerBehaviour(tower, _battleContext, _statsLibrary);
      }
    }
  }
}
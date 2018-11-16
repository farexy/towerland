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
      var stats = _statsLibrary.GetUnitStats(unit.Type);
      switch (stats.Ability)
      {
        case AbilityId.None:
          return new BaseUnitBehaviour(unit, _battleContext, _statsLibrary);
        case AbilityId.Unit_RevivesDeadUnit:
          return new NecromancerBehaviour(unit, _battleContext, _statsLibrary);
        case AbilityId.Unit_DestroysTowerOnDeath:
          return new BarbarianBehaviour(unit, _battleContext, _statsLibrary);
        default:
          return new BaseUnitBehaviour(unit, _battleContext, _statsLibrary);
      }
    }

    public IBehaviour CreateTowerBehaviour(Tower tower)
    {
      var stats = _statsLibrary.GetTowerStats(tower.Type);
      switch (stats.Ability)
      {
        case AbilityId.None:
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
        case AbilityId.Tower_FreezesUnit:
          return new FreezingTowerBehaviour(tower, _battleContext, _statsLibrary);
        case AbilityId.Tower_PoisonsUnit:
          return new PoisoningTowerBehaviour(tower, _battleContext, _statsLibrary);
        case AbilityId.Tower_10xDamage_10PercentProbability:
          return new ExtraDamageTowerBehaviour(tower, _battleContext, _statsLibrary);
        default:
          return new BaseTowerBehaviour(tower, _battleContext, _statsLibrary);
      }
    }
  }
}
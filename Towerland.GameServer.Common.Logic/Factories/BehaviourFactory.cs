using System;
using Towerland.GameServer.Common.Logic.Behaviour;
using Towerland.GameServer.Common.Logic.Behaviour.Towers;
using Towerland.GameServer.Common.Logic.Behaviour.Units;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Factories
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
      var effectId = unit.Effect == null || unit.Effect.Id == EffectId.None
        ? stats.SpecialEffect?.Id ?? EffectId.None
        : unit.Effect.Id;

      switch (effectId)
      {
        case EffectId.None:
          return new BaseUnitBehaviour(unit, _battleContext, _statsLibrary);
        case EffectId.UnitFreezed:
          return new FreezedUnitBehaviour(unit, _battleContext, _statsLibrary);
        case EffectId.ReviveDeadUnitsAtPreviousTick:
          return new NecromancerBehaviour(unit, _battleContext, _statsLibrary);
        default:
          return new BaseUnitBehaviour(unit, _battleContext, _statsLibrary);
      }
    }

    public IBehaviour CreateTowerBehaviour(Tower tower)
    {
      var stats = _statsLibrary.GetTowerStats(tower.Type);
      switch (stats.SpecialEffect?.Id ?? EffectId.None)
      {
        case EffectId.None:
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
        case EffectId.UnitFreezed:
          return new FreezingTowerBehaviour(tower, _battleContext, _statsLibrary);
        case EffectId.Unit10xDamage_10PercentProbability:
          return new ExtraDamageTowerBehaviour(tower, _battleContext, _statsLibrary);
        default:
          return new BaseTowerBehaviour(tower, _battleContext, _statsLibrary);
      }
    }
  }
}
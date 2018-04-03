using System;
using System.Collections.Generic;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.Exceptions;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Factories
{
  public class SpecialEffectLogic
  {
    public Func<Unit, List<GameAction>, bool> AffectAppliedUnitEffect { get; set; }
    public Action<TowerStats, Unit, List<GameAction>, int?> ApplyTowerAttackEffect { get; set; }
  }
  
  public class SpecialEffectLogicFactory
  {
    private static readonly Dictionary<EffectId, SpecialEffectLogic> Effects = new Dictionary<EffectId, SpecialEffectLogic>
    {
      [EffectId.None]= new SpecialEffectLogic
      {
        ApplyTowerAttackEffect = (towerStats, unit, actions, duration) => {}
      },
      [EffectId.UnitFreezed] = new SpecialEffectLogic
      {
        AffectAppliedUnitEffect = (unit, actions) => { return false; },
        ApplyTowerAttackEffect = (towerStats, unit, actions, duration) =>
        {
          unit.WaitTicks *= SpecialEffect.FreezedSlowCoeff;
          unit.Effect = new SpecialEffect{Effect = EffectId.UnitFreezed, Duration = duration ?? 0};
          actions.Add(new GameAction{ActionId = ActionId.UnitFreezes, UnitId = unit.GameId, WaitTicks = duration ?? 0});
        }
      },
      [EffectId.Unit10xDamage_10PercentProbability] = new SpecialEffectLogic
      {
        ApplyTowerAttackEffect = (towerStats, unit, actions, duration) =>
        {
          if (GameMath.CalcProbableEvent(10))
          {
            unit.Health -= towerStats.Damage * 9;
            actions.Add(new GameAction
            {
              ActionId = ActionId.UnitRecievesDamage,
              UnitId = unit.GameId,
              Damage = towerStats.Damage * 9
            });
          }
        }
      }
    };

    public SpecialEffectLogic GetEffectLogic(EffectId effectId)
    {
      if (!Effects.ContainsKey(effectId))
      {
        throw new LogicException("No effect with such id found");
      }
      return Effects[effectId];
    } 
  }
}
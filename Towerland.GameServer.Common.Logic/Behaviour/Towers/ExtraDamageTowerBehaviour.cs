using System;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Behaviour.Towers
{
  public class ExtraDamageTowerBehaviour : BaseTowerBehaviour
  {
    public ExtraDamageTowerBehaviour(Tower tower, BattleContext battleContext, IStatsLibrary statsLibrary) : base(tower, battleContext, statsLibrary)
    {
    }

    protected override int CalculateDamage(Unit unit)
    {
      bool isApplicable;
      int damageMultiplier;
      switch (Stats.SpecialEffect.Id)
      {
        case EffectId.Unit10xDamage_10PercentProbability:
          isApplicable = GameMath.CalcProbableEvent(10);
          damageMultiplier = 10;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      var damage = base.CalculateDamage(unit);

      return isApplicable
        ? damage * damageMultiplier
        : damage;
    }
  }
}
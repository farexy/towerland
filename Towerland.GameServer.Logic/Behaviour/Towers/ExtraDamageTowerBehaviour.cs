using System;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Towers
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
      switch (Stats.Ability)
      {
        case AbilityId.Tower_10xDamage_10PercentProbability:
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
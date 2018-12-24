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
      bool isApplicable = false;
      var skill = StatsLibrary.GetSkill(Stats.Skill, Stats.Type);

      if (skill.ProbabilityPercent > 0)
      {
        isApplicable = GameMath.CalcProbableEvent(skill.ProbabilityPercent);
      }

      var damage = base.CalculateDamage(unit);

      return isApplicable
        ? (int)(damage * skill.BuffValue)
        : damage;
    }
  }
}
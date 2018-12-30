using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Units
{
  public class GoblinBehaviour : BaseUnitBehaviour
  {
    public GoblinBehaviour(Unit unit, BattleContext battleContext, IStatsLibrary statsLibrary) : base(unit, battleContext, statsLibrary)
    {
    }

    public override bool ApplyPreActionEffect()
    {
      base.ApplyPreActionEffect();

      var skill = StatsLibrary.GetSkill(Stats.Skill, Stats.Type);
      if (GameMath.CalcProbableEvent(skill.ProbabilityPercent))
      {
        BattleContext.AddAction(new GameAction
        {
          ActionId = ActionId.TowerPlayerLosesMoney,
          Money = (int)skill.DebuffValue
        });
        BattleContext.AddAction(new GameAction
        {
          ActionId = ActionId.MonsterPlayerReceivesMoney,
          Money = (int)skill.BuffValue
        });
        BattleContext.AddAction(new GameAction
        {
          ActionId = ActionId.UnitAppliesSkill,
          UnitId = Unit.GameId,
          WaitTicks = skill.WaitTicks,
          SkillId = skill.Id
        });

        return false;
      }

      return true;
    }
  }
}
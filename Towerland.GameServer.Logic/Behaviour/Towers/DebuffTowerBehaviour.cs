using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Towers
{
  public class DebuffTowerBehaviour : BaseTowerBehaviour
  {
    public DebuffTowerBehaviour(Tower tower, BattleContext battleContext, IStatsLibrary statsLibrary) : base(tower, battleContext, statsLibrary)
    {
    }

    protected override void ApplyEffectOnAttack(Unit unit)
    {
      var skill = StatsLibrary.GetSkill(Stats.Skill, Stats.Type);
      BattleContext.AddAction(new GameAction
      {
        ActionId = ActionId.UnitGetsEffect,
        UnitId = unit.GameId,
        WaitTicks = skill.Duration,
        EffectId = skill.EffectId,
        EffectValue = skill.DebuffValue
      });
    }
  }
}
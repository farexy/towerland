using System.Linq;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Units
{
  public class DemonBehaviour : BaseUnitBehaviour
  {
    public DemonBehaviour(Unit unit, BattleContext battleContext, IStatsLibrary statsLibrary) : base(unit, battleContext, statsLibrary)
    {
    }

    public override void ApplyAura()
    {
      var skill = StatsLibrary.GetSkill(Stats.Skill, Stats.Type);
      var towersInRange = Field.State.Towers
        .Where(t => GameMath.Distance(Unit.Position, t.Position) < skill.Range && t.Effect.IsEmpty());
      foreach (var tower in towersInRange)
      {
        BattleContext.AddAction(new GameAction
        {
          ActionId = ActionId.TowerGetsEffect,
          TowerId = tower.GameId,
          EffectId = skill.EffectId,
          WaitTicks = skill.Duration
        });
      }
    }
  }
}
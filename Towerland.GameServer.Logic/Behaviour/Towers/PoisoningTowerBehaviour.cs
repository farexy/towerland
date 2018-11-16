using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Towers
{
  public class PoisoningTowerBehaviour : BaseTowerBehaviour
  {
    private const int Duration = 20;

    public PoisoningTowerBehaviour(Tower tower, BattleContext battleContext, IStatsLibrary statsLibrary) : base(tower, battleContext, statsLibrary)
    {
    }

    protected override void ApplyEffectOnAttack(Unit unit)
    {
      unit.Effect = new SpecialEffect {Id = EffectId.UnitPoisoned, Duration = Duration};
      BattleContext.CurrentTick.Add(new GameAction
      {
        ActionId = ActionId.UnitPoisoned, UnitId = unit.GameId, WaitTicks = Duration
      });
    }
  }
}
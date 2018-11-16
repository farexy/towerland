using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Towers
{
  public class FreezingTowerBehaviour : BaseTowerBehaviour
  {
    private const int Duration = 16;

    public FreezingTowerBehaviour(Tower tower, BattleContext battleContext, IStatsLibrary statsLibrary) : base(tower, battleContext, statsLibrary)
    {
    }

    protected override void ApplyEffectOnAttack(Unit unit)
    {
      unit.Effect = new SpecialEffect {Id = EffectId.UnitFreezed, Duration = Duration};
      BattleContext.CurrentTick.Add(new GameAction
      {
        ActionId = ActionId.UnitFreezes, UnitId = unit.GameId, WaitTicks = Duration
      });
    }
  }
}
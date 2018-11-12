using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Behaviour.Towers
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
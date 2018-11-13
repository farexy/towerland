using System.Linq;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Behaviour.Units
{
  public class BarbarianBehaviour : BaseUnitBehaviour
  {
    public BarbarianBehaviour(Unit unit, BattleContext battleContext, IStatsLibrary statsLibrary) : base(unit, battleContext, statsLibrary)
    {
    }

    public override void TickEndAction()
    {
      base.TickEndAction();

      var towerKillsAction = BattleContext.CurrentTick.FirstOrDefault(a => a.ActionId == ActionId.TowerKills && a.UnitId == Unit.GameId);
      if (towerKillsAction.ActionId != ActionId.Empty)
      {
        BattleContext.TowersToRemove.Add(towerKillsAction.TowerId);

        BattleContext.CurrentTick.Add(new GameAction
        {
          ActionId = ActionId.UnitDestroysTower,
          UnitId = Unit.GameId,
          TowerId = towerKillsAction.TowerId
        });
        BattleContext.CurrentTick.Add(new GameAction
        {
          ActionId = ActionId.UnitAppliesEffect_DarkMagic,
          UnitId = Unit.GameId,
        });
      }
    }
  }
}
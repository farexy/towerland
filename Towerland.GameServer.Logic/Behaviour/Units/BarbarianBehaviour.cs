using System.Linq;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Units
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
          ActionId = ActionId.UnitAppliesSkill,
          UnitId = Unit.GameId,
        });
      }
    }
  }
}
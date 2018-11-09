using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Logic.Behaviour.Units
{
  public class FreezedUnitBehaviour : BaseUnitBehaviour
  {
    private const int FreezeSlowCoeff = 2;

    public FreezedUnitBehaviour(Unit unit, BattleContext battleContext, IStatsLibrary statsLibrary) : base(unit, battleContext, statsLibrary)
    {
    }

    protected override void Move(Path path)
    {
      var speed = Stats.Speed * FreezeSlowCoeff;
      var nextPoint = path.GetNext(Unit.Position);
      Unit.Position = nextPoint;
      Unit.WaitTicks = speed;
      BattleContext.CurrentTick.Add(new GameAction
      {
        ActionId = ActionId.UnitMoves,
        Position = nextPoint,
        UnitId = Unit.GameId,
        WaitTicks = speed
      });
    }
  }
}
using System.Collections.Generic;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Common.Logic
{
  public class BattleContext
  {
    public BattleContext(Field field)
    {
      Field = field;
      Ticks = new List<List<GameAction>>(40);
      CurrentTick = new List<GameAction>();
      UnitsToRemove = new List<int>();
    }

    public Field Field { get; }
    public List<List<GameAction>> Ticks { get; }
    public List<GameAction> CurrentTick { get; }
    public List<int> UnitsToRemove { get; }
  }
}
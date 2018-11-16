using System.Collections.Generic;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic
{
  public class BattleContext
  {
    public BattleContext(Field field)
    {
      Field = field;
      Ticks = new List<List<GameAction>>(40);
      CurrentTick = new List<GameAction>();
      UnitsToRemove = new HashSet<int>();
      UnitsToAdd = new List<Unit>();
      TowersToRemove = new HashSet<int>();
      TowersToAdd = new List<Tower>();
      RevivedUnits = (new HashSet<int>(), new HashSet<int>());
    }

    public Field Field { get; }
    public List<List<GameAction>> Ticks { get; }
    public List<GameAction> CurrentTick { get; }

    #region Help

    public HashSet<int> UnitsToRemove { get; }
    public List<Unit> UnitsToAdd { get; }
    public HashSet<int> TowersToRemove { get; }
    public List<Tower> TowersToAdd { get; }
    public (HashSet<int> OldIds, HashSet<int> NewIds) RevivedUnits { get; }

    #endregion
  }
}
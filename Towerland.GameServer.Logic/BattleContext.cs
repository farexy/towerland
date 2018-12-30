using System.Collections.Generic;
using Towerland.GameServer.Logic.ActionResolver;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic
{
  public class BattleContext
  {
    private readonly IActionResolver _actionResolver;
    
    public BattleContext(Field field)
    {
      _actionResolver = new FieldStateActionResolver(field);
      Field = field;
      Ticks = new List<List<GameAction>>(40);
      CurrentTick = new List<GameAction>();
      UnitsToRemove = new HashSet<int>();
      UnitsToAdd = new List<Unit>();
      TowersToRemove = new HashSet<int>();
      TowersToAdd = new List<Tower>();
    }

    public Field Field { get; }
    public List<List<GameAction>> Ticks { get; }
    public List<GameAction> CurrentTick { get; }

    #region Help

    public HashSet<int> UnitsToRemove { get; }
    public List<Unit> UnitsToAdd { get; }
    public HashSet<int> TowersToRemove { get; }
    public List<Tower> TowersToAdd { get; }

    #endregion

    #region Actions

    public void AddAction(GameAction action)
    {
      _actionResolver.Resolve(action);
      CurrentTick.Add(action);
    }

    #endregion
  }
}
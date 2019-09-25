using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.Logic.Selectors
{
  public class ComputerEnemyAI
  {
    private const int MinRequiredResources = 50;
    private readonly PlayerSide _side;

    public ComputerEnemyAI(PlayerSide side)
    {
      _side = side;
    }

    public IEnumerable<StateChangeCommand> AnalizePossibleAction(Field field)
    {
      var resources = _side == PlayerSide.Monsters ? field.State.MonsterMoney : field.State.TowerMoney;
      if (resources < MinRequiredResources)
      {
        return EmptyCommand;
      }

      return EmptyCommand;
    }

    public static IEnumerable<StateChangeCommand> EmptyCommand => Enumerable.Empty<StateChangeCommand>();
  }
}
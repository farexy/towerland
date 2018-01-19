using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.State;

namespace Towerland.GameServer.Common.Logic.SpecialAI
{
  public class ComputerEnemyAI
  {
    private const int MinRequiredRecources = 50;
    private readonly PlayerSide _side;

    public ComputerEnemyAI(PlayerSide side)
    {
      _side = side;
    }

    public IEnumerable<StateChangeCommand> AnalizePossibleAction(Field field)
    {
      var resources = _side == PlayerSide.Monsters ? field.State.MonsterMoney : field.State.TowerMoney;
      if (resources < MinRequiredRecources)
      {
        return EmptyCommand;
      }

      return EmptyCommand;
    }
    
    public static IEnumerable<StateChangeCommand> EmptyCommand => Enumerable.Empty<StateChangeCommand>();
  }
}
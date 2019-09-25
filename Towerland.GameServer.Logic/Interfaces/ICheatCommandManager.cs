using System.Collections.Generic;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface ICheatCommandManager
  {
    List<GameAction> ResolveCommand(string command, Field field);
  }
}
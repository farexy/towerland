using Towerland.GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface ICheatCommandManager
  {
    void ResolveCommand(string command, Field field);
  }
}
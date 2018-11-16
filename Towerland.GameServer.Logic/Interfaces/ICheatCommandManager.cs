using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface ICheatCommandManager
  {
    void ResolveCommand(string command, Field field);
  }
}
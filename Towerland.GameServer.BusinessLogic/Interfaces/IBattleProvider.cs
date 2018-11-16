using System;
using System.Threading.Tasks;

namespace Towerland.GameServer.BusinessLogic.Interfaces
{
  public interface IBattleInitializationService
  {
    Task<Guid> InitNewBattleAsync(Guid monstersPlayer, Guid towersPlayer);
  }
}

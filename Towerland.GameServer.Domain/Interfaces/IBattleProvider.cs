using System;
using System.Threading.Tasks;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IBattleInitializationService
  {
    Task<Guid> InitNewBattleAsync(Guid monstersPlayer, Guid towersPlayer);
  }
}

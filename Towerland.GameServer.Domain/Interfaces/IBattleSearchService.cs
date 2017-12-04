using System;
using System.Threading.Tasks;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IBattleSearchService
  {
    Task AddToQueueAsync(Guid sessionId);
    bool TryGetBattle(Guid sessionId, out Guid battleId);
  }
}

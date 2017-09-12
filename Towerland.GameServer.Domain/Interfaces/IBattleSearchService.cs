using System;
using System.Threading.Tasks;

namespace Towerland.GameServer.Domain.Interfaces
{
  interface IBattleSearchService
  {
    Task AddToQueueAsync(string sessionId);
    bool TryGetBattle(string sessionId, out Guid battleId);
  }
}

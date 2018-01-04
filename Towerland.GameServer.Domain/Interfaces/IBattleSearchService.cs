using System;
using System.Threading.Tasks;
using Towerland.GameServer.Common.Models.State;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IBattleSearchService
  {
    Task AddToQueueAsync(Guid sessionId);
    bool TryGetBattle(Guid sessionId, out Guid battleId, out PlayerSide side);
  }
}

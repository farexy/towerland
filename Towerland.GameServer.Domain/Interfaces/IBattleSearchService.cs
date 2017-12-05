using System;
using System.Threading.Tasks;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.State;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IBattleSearchService
  {
    Task AddToQueueAsync(Guid sessionId);
    bool TryGetBattle(Guid sessionId, out Guid battleId, out PlayerSide side);
  }
}

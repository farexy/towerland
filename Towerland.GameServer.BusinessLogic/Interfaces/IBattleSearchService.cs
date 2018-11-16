using System;
using System.Threading.Tasks;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.BusinessLogic.Interfaces
{
  public interface IBattleSearchService
  {
    Task AddToQueueAsync(Guid sessionId);
    bool TryGetBattle(Guid sessionId, out Guid battleId, out PlayerSide side);
  }
}

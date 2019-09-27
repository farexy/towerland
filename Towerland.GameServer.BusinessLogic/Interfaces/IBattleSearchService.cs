using System;
using System.Threading.Tasks;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.BusinessLogic.Interfaces
{
  public interface IBattleSearchService
  {
    Task BeginSinglePlayAsync(Guid sessionId);
    Task AddToQueueAsync(Guid sessionId);
    Task AddToMultiBattleQueueAsync(Guid sessionId);
    bool TryGetBattle(Guid sessionId, out (Guid battleId, PlayerSide side) playerSetting);
  }
}

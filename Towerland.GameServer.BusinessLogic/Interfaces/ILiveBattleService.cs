using System;
using System.Threading.Tasks;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.BusinessLogic.Interfaces
{
  public interface ILiveBattleService
  {
    bool CheckChanged(Guid battleId, int version);
    Field GetField(Guid battleId);
    LiveBattleModel GetActualBattleState(Guid battleId, out int revision);
    Task RecalculateAsync(StateChangeCommand command);
    Task TryEndBattleAsync(Guid battleId, Guid userId);
    void ResolveActions(LiveBattleModel battle);
  }
}

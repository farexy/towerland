using System;
using System.Threading.Tasks;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.State;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface ILiveBattleService
  {
    bool CheckChanged(Guid battleId, int version);
    Field GetField(Guid battleId);
    LiveBattleModel GetActualBattleState(Guid battleId, out int revision);
    Task RecalculateAsync(StateChangeCommand command);
    Task TryEndBattleAsync(Guid battleId, Guid userId);
  }
}

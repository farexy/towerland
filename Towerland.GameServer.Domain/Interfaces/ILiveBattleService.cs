using System;
using System.Threading.Tasks;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.State;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface ILiveBattleService
  {
    bool CheckChanged(Guid battleId, int version);
    Field GetFieldState(Guid battleId);
    Task RecalculateAsync(StateChangeCommand command, FieldState fieldState);
  }
}

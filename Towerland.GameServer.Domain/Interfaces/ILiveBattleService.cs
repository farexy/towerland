﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.State;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface ILiveBattleService
  {
    bool CheckChanged(Guid battleId, int version);
    int GetRevision(Guid battleId);
    FieldState GetFieldState(Guid battleId);
    IEnumerable<GameTick> GetCalculatedActionsByTicks(Guid battleId);
    Task RecalculateAsync(StateChangeCommand command, int curTick);
    Task TryEndBattleAsync(Guid battleId, Guid userId);
  }
}

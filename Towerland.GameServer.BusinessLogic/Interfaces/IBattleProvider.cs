﻿using System;
using System.Threading.Tasks;
using Towerland.GameServer.Data.Entities;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.BusinessLogic.Interfaces
{
  public interface IBattleInitializationService
  {
    Task<Guid> InitNewBattleAsync(Guid monstersPlayer, Guid towersPlayer, GameMode mode);
    Task<Guid> InitNewMultiBattleAsync(Guid monstersPlayer, Guid towersPlayer);

    PlayerSide AddToMultiBattle(Guid battleId, Guid player, out bool isAcceptNewPlayers);
    bool CheckMultiBattleAcceptNewPlayers(Guid battleId);
  }
}

﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.BusinessLogic.Infrastructure
{
  public class BattleSearchService : IBattleSearchService
  {
    private static readonly Random Rnd = new Random();
    private readonly ConcurrentQueue<BattleSearchSession> _sessionQueue;

    private readonly ConcurrentDictionary<Guid, (Guid, PlayerSide)> _sessionBattles;

    private readonly IBattleInitializationService _battleInitService;

    private readonly ConcurrentQueue<BattleSearchSession> _sessionQueueMultiBattle;
    private Guid? _availableMultiBattle;

    public BattleSearchService(IBattleInitializationService battleInitService)
    {
      _battleInitService = battleInitService;

      _sessionQueue = new ConcurrentQueue<BattleSearchSession>();
      _sessionBattles = new ConcurrentDictionary<Guid, (Guid, PlayerSide)>();

      _sessionQueueMultiBattle = new ConcurrentQueue<BattleSearchSession>();
    }

    public async Task AddToQueueAsync(Guid sessionId)
    {
      if (_sessionQueue.Any(s => s.SessionId == sessionId))
      {
        return;
      }

      var session = new BattleSearchSession
      {
        SessionId = sessionId,
        SearchBegin = DateTime.UtcNow
      };
      if (_sessionQueue.Any())
      {
        if (_sessionQueue.TryDequeue(out var enemySession))
        {
          Guid monstersUser, towersUser;
          if (Rnd.Next() % 2 == 0)
          {
            monstersUser = sessionId;
            towersUser = enemySession.SessionId;
          }
          else
          {
            monstersUser = enemySession.SessionId;
            towersUser = sessionId;
          }

          var battleId = await _battleInitService.InitNewBattleAsync(monstersUser, towersUser);
          _sessionBattles.TryAdd(monstersUser, (battleId, PlayerSide.Monsters));
          _sessionBattles.TryAdd(towersUser, (battleId, PlayerSide.Towers));

          return;
        }
      }

      _sessionQueue.Enqueue(session);
    }

    public bool TryGetBattle(Guid sessionId, out (Guid battleId, PlayerSide side) playerSetting)
    {
      if (_sessionBattles.ContainsKey(sessionId))
      {
        _sessionBattles.TryRemove(sessionId, out playerSetting);
        return true;
      }

      playerSetting = default;
      return false;
    }

    private struct BattleSearchSession
    {
      public Guid SessionId;
      public DateTime SearchBegin;
    }

    #region Multibattles

    public async Task AddToMultiBattleQueueAsync(Guid sessionId)
    {
      if (_sessionQueueMultiBattle.Any(s => s.SessionId == sessionId))
      {
        return;
      }

      if (_availableMultiBattle.HasValue)
      {
        var side = _battleInitService.AddToMultiBattle(_availableMultiBattle.Value, sessionId, out var isAcceptNewPlayers);
        _sessionBattles.TryAdd(sessionId, (_availableMultiBattle.Value, side));

        if (!isAcceptNewPlayers)
        {
          _availableMultiBattle = null;
        }
        return;
      }

      var session = new BattleSearchSession
      {
        SessionId = sessionId,
        SearchBegin = DateTime.UtcNow
      };

      if (_sessionQueueMultiBattle.Any())
      {
        if (_sessionQueueMultiBattle.TryDequeue(out var enemySession))
        {
          Guid monstersUser, towersUser;
          if (Rnd.Next() % 2 == 0)
          {
            monstersUser = sessionId;
            towersUser = enemySession.SessionId;
          }
          else
          {
            monstersUser = enemySession.SessionId;
            towersUser = sessionId;
          }

          var battleId = await _battleInitService.InitNewMultiBattleAsync(monstersUser, towersUser);

          _availableMultiBattle = battleId;
          _sessionBattles.TryAdd(monstersUser, (battleId, PlayerSide.Monsters));
          _sessionBattles.TryAdd(towersUser, (battleId, PlayerSide.Towers));

          return;
        }
      }

      _sessionQueueMultiBattle.Enqueue(session);
    }

    #endregion
  }
}

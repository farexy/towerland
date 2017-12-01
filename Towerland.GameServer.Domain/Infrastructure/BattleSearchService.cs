using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Towerland.GameServer.Domain.Interfaces;

namespace Towerland.GameServer.Domain.Infrastructure
{
  class BattleSearchService : IBattleSearchService
  {
    private readonly ConcurrentQueue<string> _sessionQueue;
    private readonly ConcurrentDictionary<string, Guid> _sessionBattles;
    private readonly IBattleService _battleProvider;

    public BattleSearchService(IBattleService battleProvider)
    {
      _sessionQueue = new ConcurrentQueue<string>();
      _sessionBattles = new ConcurrentDictionary<string, Guid>();
      _battleProvider = battleProvider;
    }

    public async Task AddToQueueAsync(string sessionId)
    {
      await Task.Run(() =>
        {
          if (_sessionQueue.Any())
          {
            if (_sessionQueue.TryDequeue(out var enemySession))
            {
              var battleId = _battleProvider.InitNewBattle();
              _sessionBattles.TryAdd(sessionId, battleId);
              _sessionBattles.TryAdd(enemySession, battleId);
              return;
            }
          }
          _sessionQueue.Enqueue(sessionId);
        }
      );
    }

    public bool TryGetBattle(string sessionId, out Guid battleId)
    {
      if (_sessionBattles.ContainsKey(sessionId))
      {
        _sessionBattles.TryRemove(sessionId, out battleId);
        return true;
      }
      battleId = Guid.Empty;
      return false;
    }
  }
}

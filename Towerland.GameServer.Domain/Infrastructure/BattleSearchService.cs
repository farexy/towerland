using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Towerland.GameServer.Domain.Interfaces;

namespace Towerland.GameServer.Domain.Infrastructure
{
  public class BattleSearchService : IBattleSearchService
  {
    private static ConcurrentQueue<Guid> _sessionQueue;
    private static ConcurrentDictionary<Guid, Guid> _sessionBattles;
    private readonly IBattleService _battleProvider;

    public BattleSearchService(IBattleService battleProvider)
    {
      _battleProvider = battleProvider;
    }

    public async Task AddToQueueAsync(Guid sessionId)
    {
      if (_sessionQueue == null)
      {
        _sessionQueue = new ConcurrentQueue<Guid>();
      }
      if (_sessionBattles == null)
      {
        _sessionBattles = new ConcurrentDictionary<Guid, Guid>();
      }
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

    public bool TryGetBattle(Guid sessionId, out Guid battleId)
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

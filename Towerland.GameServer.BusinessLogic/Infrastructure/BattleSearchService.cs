using System;
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
    private readonly ConcurrentDictionary<Guid, Guid> _sessionBattles;
    private readonly ConcurrentDictionary<Guid, PlayerSide> _battlePlayerSides;

    private readonly IBattleInitializationService _battleProvider;

    public BattleSearchService(IBattleInitializationService battleProvider)
    {
      _battleProvider = battleProvider;

      _sessionQueue = new ConcurrentQueue<BattleSearchSession>();
      _sessionBattles = new ConcurrentDictionary<Guid, Guid>();
      _battlePlayerSides = new ConcurrentDictionary<Guid, PlayerSide>();
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

          _battlePlayerSides.TryAdd(monstersUser, PlayerSide.Monsters);
          _battlePlayerSides.TryAdd(towersUser, PlayerSide.Towers);

          var battleId = await _battleProvider.InitNewBattleAsync(monstersUser, towersUser);
          _sessionBattles.TryAdd(sessionId, battleId);
          _sessionBattles.TryAdd(enemySession.SessionId, battleId);

          return;
        }
      }

      _sessionQueue.Enqueue(session);
    }

    public bool TryGetBattle(Guid sessionId, out Guid battleId, out PlayerSide side)
    {
      if (_sessionBattles.ContainsKey(sessionId))
      {
        _sessionBattles.TryRemove(sessionId, out battleId);
        _battlePlayerSides.TryRemove(sessionId, out side);
        return true;
      }
      side = PlayerSide.Undefined;
      battleId = Guid.Empty;
      return false;
    }

    private struct BattleSearchSession
    {
      public Guid SessionId;
      public DateTime SearchBegin;
    }
  }
}

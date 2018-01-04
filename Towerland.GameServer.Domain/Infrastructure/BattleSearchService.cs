using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Towerland.GameServer.Common.Models.State;
using Towerland.GameServer.Domain.Interfaces;

namespace Towerland.GameServer.Domain.Infrastructure
{
  public class BattleSearchService : IBattleSearchService
  {
    private static readonly Random Rnd = new Random();
    private static ConcurrentQueue<Guid> _sessionQueue;
    private static ConcurrentDictionary<Guid, Guid> _sessionBattles;
    private static ConcurrentDictionary<Guid, PlayerSide> _battlePlayerSides;
      
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
        _battlePlayerSides = new ConcurrentDictionary<Guid, PlayerSide>();
      }
      await Task.Run(() =>
        {
          if (_sessionQueue.Any())
          {
            if (_sessionQueue.TryDequeue(out var enemySession))
            {
              Guid monstersUser, towersUser;
              if (Rnd.Next() % 2 == 0)
              {
                monstersUser = sessionId;
                towersUser = enemySession;
              }
              else
              {
                monstersUser = enemySession;
                towersUser = sessionId;
              }
              _battlePlayerSides.TryAdd(monstersUser, PlayerSide.Monsters);
              _battlePlayerSides.TryAdd(towersUser, PlayerSide.Towers);
              
              var battleId = _battleProvider.InitNewBattle(monstersUser, towersUser);
              _sessionBattles.TryAdd(sessionId, battleId);
              _sessionBattles.TryAdd(enemySession, battleId);
              
              return;
            }
          }
          _sessionQueue.Enqueue(sessionId);
        }
      );
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
  }
}

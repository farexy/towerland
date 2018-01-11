using System;
using System.Collections.Concurrent;
using Towerland.GameServer.Common.Models.Exceptions;
using Towerland.GameServer.Domain.Interfaces;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Infrastructure
{
  public class BattleInMemoryProvider : IProvider<LiveBattleModel>
  {
    private static readonly ConcurrentDictionary<Guid, LiveBattleModel> _battles;

    static BattleInMemoryProvider()
    {
      _battles = new ConcurrentDictionary<Guid, LiveBattleModel>();
    }

    public LiveBattleModel Find(Guid id)
    {
      if (!_battles.ContainsKey(id))
      {
        throw new LogicException("Not found");
      }
      return _battles[id];
    }

    public Guid Create(LiveBattleModel obj)
    {
      _battles.TryAdd(obj.Id, obj);
      return obj.Id;
    }

    public void Update(LiveBattleModel obj)
    {
      if (!_battles.ContainsKey(obj.Id))
      {
        throw new LogicException("Not found");
      }
      _battles[obj.Id] = obj;
    }

    public void Delete(Guid id)
    {
      if (!_battles.ContainsKey(id))
      {
        throw new LogicException("Not found");
      }
      _battles.TryRemove(id, out var _);
    }

    public void Clear()
    {
      _battles.Clear();
    }
  }
}
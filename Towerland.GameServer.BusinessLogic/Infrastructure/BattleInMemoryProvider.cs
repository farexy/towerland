using System;
using System.Collections.Concurrent;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Models.Exceptions;

namespace Towerland.GameServer.BusinessLogic.Infrastructure
{
  public class BattleInMemoryProvider : IProvider<LiveBattleModel>
  {
    private readonly ConcurrentDictionary<Guid, LiveBattleModel> _battles;

    public BattleInMemoryProvider()
    {
      _battles = new ConcurrentDictionary<Guid, LiveBattleModel>();
    }

    public LiveBattleModel Find(Guid id)
    {
      if (!_battles.ContainsKey(id))
      {
        return null;
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
      _battles.TryRemove(id, out _);
    }

    public void Clear()
    {
      _battles.Clear();
    }
  }
}
using System;
using System.Collections.Generic;
using Towerland.GameServer.Common.Models.Exceptions;
using Towerland.GameServer.Core.DataAccess;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Infrastructure
{
  public class BattleInMemoryProvider : IProvider<LiveBattleModel>
  {
    private static readonly Dictionary<Guid, LiveBattleModel> _battles;

    static BattleInMemoryProvider()
    {
      _battles = new Dictionary<Guid, LiveBattleModel>();
    }

    public LiveBattleModel Get(Guid id)
    {
      if (!_battles.ContainsKey(id))
      {
        throw new LogicException("Not found");
      }
      return _battles[id];
    }

    public Guid Add(LiveBattleModel obj)
    {
      _battles.Add(obj.Id, obj);
      return obj.Id;
    }

    public void Update(Guid id, LiveBattleModel obj)
    {
      if (!_battles.ContainsKey(id))
      {
        throw new LogicException("Not found");
      }
      _battles[id] = obj;
    }

    public void Delete(Guid id)
    {
      if (!_battles.ContainsKey(id))
      {
        throw new LogicException("Not found");
      }
      _battles.Remove(id);
    }

    public void Clear()
    {
      _battles.Clear();
    }
  }
}
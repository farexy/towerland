using System;

namespace Towerland.GameServer.Domain.Lockers
{
  public class BattleLocker : IDisposable
  {
    private static readonly KeyLocker<Guid> Locker = new KeyLocker<Guid>();
    private readonly Guid _battleId;

    public BattleLocker(Guid battleId)
    {
      _battleId = battleId;
      Locker.Lock(battleId);
    }
    
    public void Dispose()
    {
      Locker.Release(_battleId);
    }
  }
}
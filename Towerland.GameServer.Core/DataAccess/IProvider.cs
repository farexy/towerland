using System;

namespace Towerland.GameServer.Core.DataAccess
{
  public interface IProvider<T>
  {
    T Get(Guid id);

    Guid Add(T obj);
    void Update(Guid id, T obj);
    void Delete(Guid id);
    void Clear();
  }
}
using System;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Core.Interfaces;

namespace Towerland.GameServer.Core.DataAccess
{
  public interface ICrudRepository<T> : IProvider<T> where T : DataEntity
  {
    T Get(int id);
    T[] Get(object[] ids);
    T[] Get();

    int Create(IIdentityEntity entity);
    Guid Create(IGuidEntity entity);

    int Update(IIdentityEntity entity);
    Guid Update(IGuidEntity entity);

    int Update(int id, IIdentityEntity entity);
    Guid Update(Guid id, IGuidEntity entity);

    bool Delete(int id);

    void SaveStateAsync();
  }
}

using System;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Core.Interfaces;

namespace Towerland.GameServer.Core.DataAccess
{
  public class MySqlCrudRepository<T> : ICrudRepository<T> where T : DataEntity
  {
    public T Get(Guid id)
    {
      throw new NotImplementedException();
    }

    public Guid Add(T obj)
    {
      throw new NotImplementedException();
    }

    public void Update(Guid id, T obj)
    {
      throw new NotImplementedException();
    }

    public void Delete(Guid id)
    {
      throw new NotImplementedException();
    }

    public T Get(int id)
    {
      throw new NotImplementedException();
    }

    public T Get(object[] ids)
    {
      throw new NotImplementedException();
    }

    public T Get()
    {
      throw new NotImplementedException();
    }

    public int Create(IIdentityEntity entity)
    {
      throw new NotImplementedException();
    }

    public Guid Create(IGuidEntity entity)
    {
      throw new NotImplementedException();
    }

    public int Update(IIdentityEntity entity)
    {
      throw new NotImplementedException();
    }

    public Guid Update(IGuidEntity entity)
    {
      throw new NotImplementedException();
    }

    public int Update(int id, IIdentityEntity entity)
    {
      throw new NotImplementedException();
    }

    public Guid Update(Guid id, IGuidEntity entity)
    {
      throw new NotImplementedException();
    }

    public bool Delete(int id)
    {
      throw new NotImplementedException();
    }

    public void SaveStateAsync()
    {
      throw new NotImplementedException();
    }
  }
}
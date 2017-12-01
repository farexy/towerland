using System;
using System.Data.Entity;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Core.Interfaces;

namespace Towerland.GameServer.Core.DataAccess
{
  class CrudRepository<T> : ICrudRepository<T> where T : DataEntity
  {
    protected readonly DbContext _db;

    public CrudRepository(DbContext db)
    {
      _db = db;
    }

    public T Get(int id)
    {
      return _db.Set<T>().Find(id);
    }

    public T Get(Guid id)
    {
      return _db.Set<T>().Find(id);
    }

    public Guid Add(T obj)
    {
      return Create((IGuidEntity) obj);
    }

    public void Update(Guid id, T obj)
    {
      Update(id, (IGuidEntity) obj);
    }

    void IProvider<T>.Delete(Guid id)
    {
      Delete(id);
    }

    public T Get(object[] ids)
    {
      return _db.Set<T>().Find(ids);
    }

    public T Get()
    {
      return _db.Set<T>().Find(); ;
    }

    //ReSharper disable All
    public int Create(IIdentityEntity entity)
    {
      var res = _db.Set<T>().Add(entity as T);
      return (res as IIdentityEntity).Id;
    }

    public Guid Create(IGuidEntity entity)
    {
      var res = _db.Set<T>().Add(entity as T);
      return (res as IGuidEntity).Id;
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
      T current = _db.Set<T>().Find(id);
      current = entity as T;
      _db.SaveChanges();
      _db.Entry(current).State = EntityState.Modified;
      return id;
    }

    public Guid Update(Guid id, IGuidEntity entity)
    {
      T current = _db.Set<T>().Find(id);
      current = entity as T;
      _db.SaveChanges();
      _db.Entry(current).State = EntityState.Modified;
      return id;
    }
    //ReSharper enable All

    public bool Delete(int id)
    {
      T item = _db.Set<T>().Find(id);

      if (item != null)
      {
        _db.Set<T>().Remove(item);
        return true;
      }

      return false;
    }

    public bool Delete(Guid id)
    {
      T item = _db.Set<T>().Find(id);

      if (item != null)
      {
        _db.Set<T>().Remove(item);
        return true;
      }

      return false;
    }

    public async void SaveStateAsync()
    {
      _db.SaveChangesAsync();
    }
  }
}

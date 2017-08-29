using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Common.Models.GameObjects;

namespace GameServer.Common.Models.GameField
{
  public class Field : ICloneable
  {
    private Dictionary<int, GameObject> _objects;

    protected Field()
    {
    }

    public Field(FieldCell[,] cells)
    {
      _objects = new Dictionary<int, GameObject>();
      Cells = cells;
      Start = cells.Cast<FieldCell>().First(c => c.Object == FieldObject.Entrance).Position;
      Finish = cells.Cast<FieldCell>().First(c => c.Object == FieldObject.Castle).Position;
      Units = new List<Unit>();
      Towers = new List<Tower>();
    }

    public int AddGameObject(GameObject gameObj)
    {
      var type = gameObj.ResolveType();
      var id = _objects.Count + gameObj.GetHashCode() - _objects.LastOrDefault().Key;
      gameObj.GameId = id;

      if (GameObjectType.Castle == type)
      {
        if(Castle != null)
          throw new ArgumentException("There can be only one castle");
        Castle = (Castle) gameObj;
      }
      if (type == GameObjectType.Unit)
      {
        Units.Add((Unit)gameObj);
      }
      if (type == GameObjectType.Tower)
      {
        Towers.Add((Tower)gameObj);
      }

      _objects.Add(id, gameObj);

      return id;
    }

    public IEnumerable<int> AddMany(IEnumerable<GameObject> objects)
    {
      foreach (var o in objects)
      {
        yield return AddGameObject(o);
      }
    }

    public void RemoveGameObject(int gameId)
    {
      if(!_objects.ContainsKey(gameId))
        throw new ArgumentException("There is no object with spisified GameId on the field");

      var gameObj = _objects[gameId];
      var type = gameObj.ResolveType();

      if (GameObjectType.Castle == type)
      {
        throw new ArgumentException("There can be only one castle");
      }
      if (type == GameObjectType.Unit)
      {
        Units.Remove((Unit)gameObj);
      }
      if (type == GameObjectType.Tower)
      {
        Towers.Remove((Tower)gameObj);
      }

      _objects.Remove(gameId);
    }

    public void RemoveMany(IEnumerable<int> gameIds)
    {
      foreach (var id in gameIds)
      {
        RemoveGameObject(id);
      }
    }

    public GameObject this[int gameId]
    {
      get { return _objects[gameId]; }
    }

    public FieldCell[,] Cells { private set; get; }
    public Castle Castle { set; get; }

    public List<Tower> Towers { private set; get; }
    public List<Unit> Units { private set; get; }

    public Path[] Path { set; get; }

    public int Width
    {
      get { return Cells.GetLength(1); }
    }

    public int Height
    {
      get { return Cells.GetLength(0); }
    }

    public Point Start { get; private set; }
    public Point Finish { get; private set; }

    public IEnumerable<GameObject> FindGameObjects(Predicate<GameObject> predicate)
    {
      return _objects.Values.Where(obj => predicate(obj));
    }

    public object Clone()
    {
      return new Field
      {
        Cells = Cells,
        Castle = new Castle
        {
          Health = Castle.Health,
          Position = Castle.Position
        },
        _objects = _objects.ToDictionary(item => item.Key, item => item.Value),
        Towers = Towers.ToList(),
        Units = Units.ToList(),
        Path = Path
      };
    }
  }
}

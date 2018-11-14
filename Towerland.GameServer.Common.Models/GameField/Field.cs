using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Models.GameField
{
    public class Field : ICloneable
    {
        public GameObject this[int gameId] => !_objects.ContainsKey(gameId) ? null : _objects[gameId];

        [JsonProperty("sd")]
        public FieldStaticData StaticData { private set; get; }

        [JsonProperty("state")]
        public FieldState State { private set; get; }

        private Dictionary<int, GameObject> _objects;

        public Field()
        {
            _objects = new Dictionary<int, GameObject>();
            State = new FieldState();
        }

        public Field(FieldStaticData staticData) : this()
        {
            StaticData = staticData;
        }

        public Field(FieldCell[,] cells) : this()
        {
            StaticData = new FieldStaticData(cells,
                cells.Cast<FieldCell>().First(c => c.Object == FieldObject.Entrance).Position,
                cells.Cast<FieldCell>().First(c => c.Object == FieldObject.Castle).Position
            );
        }

        public int AddGameObject(GameObject gameObj)
        {
            var id = gameObj.GameId == default(int)
                ? GenerateId()
                : gameObj.GameId;
            return AddGameObject(id, gameObj);
        }

        private int AddGameObject(int gameId, GameObject gameObj)
        {
            var type = gameObj.ResolveType();
            gameObj.GameId = gameId;

            if (this[gameId]?.Type == GameObjectType.GeneratedId)
            {
                _objects[gameId] = gameObj;
            }
            _objects.Add(gameId, gameObj);

            switch (type)
            {
                case GameObjectType.Castle:
                    if (State.Castle != null)
                        throw new ArgumentException("There can be only one castle");
                    State.Castle = (Castle)gameObj;
                    break;
                case GameObjectType.Unit:
                    State.Units.Add((Unit)gameObj);
                    break;
                case GameObjectType.Tower:
                    State.Towers.Add((Tower)gameObj);
                    break;
            }

            return gameId;
        }

        public IEnumerable<int> AddMany(IEnumerable<GameObject> objects)
        {
            return objects.Select(AddGameObject);
        }

        public int GenerateGameObjectId()
        {
            var newId = GenerateId();
            AddGameObject(new GeneratedId(newId));
            return newId;
        }

        private int GenerateId()
        {
            unchecked
            {
                var id = _objects.Count * DateTime.Now.Millisecond - DateTime.Now.Second;
                while (_objects.ContainsKey(id))
                {
                    id++;
                }

                return id;
            }
        }

        public void RemoveGameObject(int gameId)
        {
            if (!_objects.ContainsKey(gameId))
                throw new ArgumentException("There is no object with specified GameId on the field");

            var gameObj = _objects[gameId];
            var type = gameObj.ResolveType();

            switch (type)
            {
                case GameObjectType.Castle:
                    throw new ArgumentException("There can be only one castle");
                case GameObjectType.Unit:
                    State.Units.Remove((Unit)gameObj);
                    break;
                case GameObjectType.Tower:
                    State.Towers.Remove((Tower)gameObj);
                    break;
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

        public IEnumerable<GameObject> FindGameObjects(Predicate<GameObject> predicate)
        {
            return _objects.Values.Where(obj => predicate(obj));
        }

        public void SetState(FieldState state)
        {
            _objects = state.Objects.ToDictionary(item => item.Key, item => (GameObject) item.Value.Clone());
            this.State = new FieldState(_objects, state.Castle)
            {
                MonsterMoney = state.MonsterMoney,
                TowerMoney = state.TowerMoney
            };
        }

        public object Clone()
        {
            var clonedObjects = _objects.ToDictionary(item => item.Key, item => (GameObject) item.Value.Clone());
            return new Field
            {
                StaticData = new FieldStaticData(StaticData.Cells, StaticData.Start, StaticData.Finish)
                {
                    Path = StaticData.Path,
                    EndTimeUtc = StaticData.EndTimeUtc
                },
                _objects = clonedObjects,
                State = new FieldState(clonedObjects, State.Castle)
                {
                    MonsterMoney = State.MonsterMoney,
                    TowerMoney = State.TowerMoney
                },
            };
        }
    }
}

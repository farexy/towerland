using System;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Calculators
{
  public class PredictableRandom
  {
    private readonly Random _rand;

    public PredictableRandom(Field field) : this(field, new GameObject())
    {
    }

    public PredictableRandom(Field field, GameObject obj)
    {
      var state = field.State;
      var seed = state.Castle.Health * state.MonsterMoney / state.TowerMoney
                 + state.Towers.Count * state.Units.Count
                 - obj.Position.X * obj.Position.Y
                 + obj.GameId;
      _rand = new Random(seed);
    }

    public int Next()
    {
      return _rand.Next();
    }

    public int Next(int max)
    {
      return _rand.Next(max);
    }

    public int Next(int min, int max)
    {
      return _rand.Next(min, max);
    }

    public bool CalcProbableEvent(int probabilityPercent)
    {
      return _rand.Next(0, 100) <= probabilityPercent;
    }
  }
}
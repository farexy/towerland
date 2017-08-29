using System;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.Stats;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic.SpecialAI
{
  public class StateChangeRecalculator
  {
    private readonly IPathOptimiser _pathOptimiser;
    private readonly IStatsLibrary _statsLib;
    private readonly Random _random;
    private readonly IGameObjectFactory<Unit> _unitsFactory;
    private readonly IGameObjectFactory<Tower> _towersFactory;

    public StateChangeRecalculator(IPathOptimiser pathOptimiser, IStatsLibrary stats, IGameObjectFactory<Unit> unitFactory, IGameObjectFactory<Tower> towerFactory)
    {
      _pathOptimiser = pathOptimiser;
      _statsLib = stats;
      _random = new Random();
      _unitsFactory = unitFactory;
      _towersFactory = towerFactory;
    }

    public void AddNewUnit(Field field, GameObjectType type, CreationOptions? opt = null)
    {
      var unit = _unitsFactory.Create(type, opt);
      field.AddGameObject(unit);
      RecalcUnitPath(field, unit);
      unit.Position = field.Start;
    }

    public void AddNewTower(Field field, GameObjectType type, CreationOptions? opt = null)
    {
      var tower = _towersFactory.Create(type, opt);
      field.AddGameObject(tower);
      foreach (Unit u in field.Units)
      {
        RecalcUnitPath(field, u);
      }
    }

    private  void RecalcUnitPath(Field field, Unit unit)
    {
      var stats = _statsLib.GetUnitStats(unit.Type);
      if (!unit.PathId.HasValue || stats.MovementPriority != UnitStats.MovementPriorityType.Random)
      {
        unit.PathId = stats.MovementPriority == UnitStats.MovementPriorityType.Optimal ? _pathOptimiser.GetOptimalPath(field.Path, unit)
          : stats.MovementPriority == UnitStats.MovementPriorityType.Fastest ? _pathOptimiser.GetFastestPath(field.Path, unit)
            : _random.Next(field.Path.Length);
      }
    }


  }
}

using System;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Exceptions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.State;
using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Logic.Calculators
{
  public class StateChangeRecalculator : IStateChangeRecalculator
  {
    private readonly IPathChooser _pathChooser;
    private readonly IStatsLibrary _statsLib;
    private readonly IGameObjectFactory<Unit> _unitsFactory;
    private readonly IGameObjectFactory<Tower> _towersFactory;

    public StateChangeRecalculator(IPathChooser pathChooser, IStatsLibrary stats, IGameObjectFactory<Unit> unitFactory, IGameObjectFactory<Tower> towerFactory)
    {
      _pathChooser = pathChooser;
      _statsLib = stats;
      _unitsFactory = unitFactory;
      _towersFactory = towerFactory;
    }

    public void AddMoney(Field field, int money, PlayerSide side)
    {
      switch (side)
      {
        case PlayerSide.Monsters:
          field.State.MonsterMoney += money;
          break;
        case PlayerSide.Towers:
          field.State.TowerMoney += money;
          break;
        default:
          field.State.MonsterMoney += money;
          field.State.TowerMoney += money;
          break;
      }
    }

    public void AddNewUnit(Field field, GameObjectType type, CreationOptions? opt = null)
    {
      var cost = _statsLib.GetUnitStats(type).Cost;
      if (field.State.MonsterMoney < cost)
      {
        throw new LogicException("Not enough money");
      }

      var unit = _unitsFactory.Create(type, opt);
      unit.Position = field.StaticData.Start;

      TryAddGameObject(field, unit);
      CalculateUnitPath(field, unit, opt);

      field.State.MonsterMoney -= cost;
    }

    public void AddNewTower(Field field, GameObjectType type, CreationOptions? opt = null)
    {
      var cost = _statsLib.GetTowerStats(type).Cost;
      if (field.State.TowerMoney < cost)
      {
        throw new LogicException("Not enough money");
      }
      if (opt == null)
      {
        throw new LogicException("Creation options for tower must have position");
      }
      PlaceTowerOnField(field, type, opt.Value);

      field.State.TowerMoney -= cost;
    }

    private void TryAddGameObject(Field field, GameObject go, int retries = 0)
    {
      try
      {
        field.AddGameObject(go);
      }
      catch (ArgumentException)
      {
        if (retries < 10)
        {
          retries++;
          TryAddGameObject(field, go, retries);
        }
      }
    }

    private void PlaceTowerOnField(Field field, GameObjectType type, CreationOptions opt)
    {
      var cellObject = field.StaticData.Cells[opt.Position.X, opt.Position.Y].Object;
      switch (_statsLib.GetTowerStats(type).SpawnType)
      {
        case TowerStats.TowerSpawnType.Ground when cellObject != FieldObject.Ground:
          throw new LogicException("Tower must be placed on free place on ground");
        case TowerStats.TowerSpawnType.Tree when cellObject != FieldObject.Tree:
          throw new LogicException("Tower must be placed on tree");
      }
      if (field.FindTowerAt(opt.Position) != null)
      {
        throw new LogicException("Cell is already busy by tower");
      }
      var tower = _towersFactory.Create(type, opt);
      TryAddGameObject(field, tower);
    }

    private void CalculateUnitPath(Field field, Unit unit, CreationOptions? opt)
    {
      var stats = _statsLib.GetUnitStats(unit.Type);
      if (!unit.PathId.HasValue)
      {
        unit.PathId = stats.MovementPriority == UnitStats.MovementPriorityType.Optimal ? _pathChooser.GetOptimalPath(field, unit)
          : stats.MovementPriority == UnitStats.MovementPriorityType.Fastest ? _pathChooser.GetFastestPath(field.StaticData.Path, unit)
          : stats.MovementPriority == UnitStats.MovementPriorityType.ByUser && opt?.PathId != null ? opt.Value.PathId
          : GameMath.Rand.Next(field.StaticData.Path.Length);
      }
    }
  }
}

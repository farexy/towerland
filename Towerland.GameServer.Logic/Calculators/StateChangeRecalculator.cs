using System;
using System.Collections.Generic;
using Towerland.GameServer.Logic.ActionResolver;
using Towerland.GameServer.Logic.Extensions;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Exceptions;
using Towerland.GameServer.Models.GameActions;
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

    public StateChangeRecalculator(IPathChooser pathChooser, IStatsLibrary stats,
      IGameObjectFactory<Unit> unitFactory, IGameObjectFactory<Tower> towerFactory)
    {
      _pathChooser = pathChooser;
      _statsLib = stats;
      _unitsFactory = unitFactory;
      _towersFactory = towerFactory;
    }

    public List<GameAction> AddMoney(Field field, int money, PlayerSide side)
    {
      var actions = new List<GameAction>();
      switch (side)
      {
        case PlayerSide.Monsters:
          actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerReceivesMoney, Money = money});
          break;
        case PlayerSide.Towers:
          actions.Add(new GameAction{ActionId = ActionId.TowerPlayerReceivesMoney, Money = money});
          break;
        default:
          actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerReceivesMoney, Money = money});
          actions.Add(new GameAction{ActionId = ActionId.TowerPlayerReceivesMoney, Money = money});
          break;
      }

      return actions;
    }

    public List<GameAction> AddNewUnit(Field field, GameObjectType type, UnitCreationOption opt = null)
    {
      var actions = new List<GameAction>();
      var cost = _statsLib.GetUnitStats(type).Cost;
      if (field.State.MonsterMoney < cost)
      {
        throw new LogicException("Not enough money");
      }

      var options = new CreationOptions
      {
        GameId = field.GenerateGameObjectId(),
        PathId = opt?.PathId ?? CalculateUnitPath(field, type),
        Position = field.StaticData.Start
      };
      var unit = _unitsFactory.Create(type, options);

      actions.Add(new GameAction{ActionId = ActionId.UnitAppears, GoUnit = unit});
      actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerLosesMoney, Money = cost});

      return actions;
    }

    public List<GameAction> AddNewTower(Field field, GameObjectType type, TowerCreationOption opt)
    {
      var actions = new List<GameAction>();

      var cost = _statsLib.GetTowerStats(type).Cost;
      if (field.State.TowerMoney < cost)
      {
        throw new LogicException("Not enough money");
      }
      if (opt == null)
      {
        throw new LogicException("Creation options for tower must have position");
      }
      var options = new CreationOptions
      {
        GameId = field.GenerateGameObjectId(),
        Position = opt.Position
      };
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
      var tower = _towersFactory.Create(type, options);

      actions.Add(new GameAction{ActionId = ActionId.TowerAppears, GoTower = tower});
      actions.Add(new GameAction{ActionId = ActionId.TowerPlayerLosesMoney, Money = cost});

      return actions;
    }

    private int? CalculateUnitPath(Field field, GameObjectType type)
    {
      return _pathChooser.GetOptimalPath(field, type);

      var stats = _statsLib.GetUnitStats(type);
      return stats.MovementPriority == UnitStats.MovementPriorityType.Optimal
        ? _pathChooser.GetOptimalPath(field, type)
        : stats.MovementPriority == UnitStats.MovementPriorityType.Fastest
          ? _pathChooser.GetFastestPath(field.StaticData.Path, field.StaticData.Start)
          : GameMath.Rand.Next(field.StaticData.Path.Length);
    }
  }
}

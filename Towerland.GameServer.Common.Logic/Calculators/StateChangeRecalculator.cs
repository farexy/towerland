using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.Exceptions;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.State;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Calculators
{
  public class StateChangeRecalculator : IStateChangeRecalculator
  {
    private readonly IPathOptimiser _pathOptimiser;
    private readonly IStatsLibrary _statsLib;
    private readonly IGameObjectFactory<Unit> _unitsFactory;
    private readonly IGameObjectFactory<Tower> _towersFactory;

    public StateChangeRecalculator(IPathOptimiser pathOptimiser, IStatsLibrary stats, IGameObjectFactory<Unit> unitFactory, IGameObjectFactory<Tower> towerFactory)
    {
      _pathOptimiser = pathOptimiser;
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

      field.AddGameObject(unit);
      RecalcUnitPath(field, unit);

      field.State.MonsterMoney -= cost;
    }

    public void AddNewTower(Field field, GameObjectType type, CreationOptions? opt = null)
    {
      var cost = _statsLib.GetTowerStats(type).Cost;
      if (field.State.TowerMoney < cost)
      {
        throw new LogicException("Not enough money");
      }
      if (opt != null && field.StaticData.Cells[opt.Value.Position.X, opt.Value.Position.Y].Object != FieldObject.Ground)
      {
        throw new LogicException("Tower can't be placed on the path");
      }

      var tower = _towersFactory.Create(type, opt);
      field.AddGameObject(tower);
      foreach (Unit u in field.State.Units)
      {
        RecalcUnitPath(field, u);
      }

      field.State.TowerMoney -= cost;
    }

    private void RecalcUnitPath(Field field, Unit unit)
    {
      var stats = _statsLib.GetUnitStats(unit.Type);
      if (!unit.PathId.HasValue)
      {
        unit.PathId = stats.MovementPriority == UnitStats.MovementPriorityType.Optimal ? _pathOptimiser.GetOptimalPath(field, unit)
          : stats.MovementPriority == UnitStats.MovementPriorityType.Fastest ? _pathOptimiser.GetFastestPath(field.StaticData.Path, unit)
            : GameMath.Rand.Next(field.StaticData.Path.Length);
      }
    }
  }
}

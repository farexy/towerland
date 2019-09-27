using System.Linq;
using Towerland.GameServer.Logic.ActionResolver;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Extensions;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.Logic.Selectors
{
  public class TowerSelector : ITowerSelector
  {
    private readonly IStatsProvider _statsProvider;
    private readonly IStatsLibrary _statsLibrary;
    private readonly IStateChangeRecalculator _stateChangeRecalculator;

    public TowerSelector(IStatsProvider statsProvider, IStatsLibrary statsLibrary, IStateChangeRecalculator stateChangeRecalculator)
    {
      _statsProvider = statsProvider;
      _stateChangeRecalculator = stateChangeRecalculator;
      _statsLibrary = statsLibrary;
    }

    public (GameObjectType type, Point position)? GetNewTower(Field field)
    {
      var selector = new IntelligentGameObjectSelector<Point>(
        _statsProvider.GetTowerStats().Where(t => t.Cost <= field.State.TowerMoney).Select(t => t.Type),
        field.StaticData.Cells.Cast<FieldCell>()
          .Where(c => c.Object is FieldObject.Ground && field.FindTowerAt(c.Position) is null).Select(c => c.Position)
      );
      return selector.GetOptimalVariant((type, pos) =>
      {
        var fieldClone = (Field) field.Clone();
        var stateChangeActions = _stateChangeRecalculator.AddNewTower(fieldClone, type, new TowerCreationOption{Type = type, Position = pos});
        var stats = _statsLibrary.GetTowerStats(type);
        var stateCalc = new StateCalculator(_statsLibrary, fieldClone, stateChangeActions);
        var actions = stateCalc.CalculateActionsByTicks().SelectMany(tick => tick.Actions).ToArray();
        var totalDamage = actions.Where(a => a.ActionId is ActionId.UnitReceivesDamage).Sum(a => a.Damage);
        var totalKills = actions.Count(a => a.ActionId is ActionId.TowerKills);
        var potentialDamage = stats.Damage * field.GetNeighbourPoints(pos, stats.Range, FieldObject.Road).Count();
        return (double) (totalDamage + 1) * (totalKills + 1) * potentialDamage / stats.Cost;
      });
    }
  }
}
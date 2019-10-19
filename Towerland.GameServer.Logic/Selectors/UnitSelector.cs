using System.Linq;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.Logic.Selectors
{
  public class UnitSelector : IUnitSelector
  {
    private readonly IStatsProvider _statsProvider;
    private readonly IStatsLibrary _statsLibrary;
    private readonly IStateChangeRecalculator _stateChangeRecalculator;

    public UnitSelector(IStatsProvider statsProvider, IStatsLibrary statsLibrary, IStateChangeRecalculator stateChangeRecalculator)
    {
      _statsProvider = statsProvider;
      _stateChangeRecalculator = stateChangeRecalculator;
      _statsLibrary = statsLibrary;
    }

    public (GameObjectType type, int pathId)? GetNewUnit(Field field)
    {
      var selector = new IntelligentGameObjectSelector<int>(
        _statsProvider.GetUnitStats().Where(u => u.Cost <= field.State.MonsterMoney).Select(u => u.Type),
        field.StaticData.Path.Select((p, idx) => idx)
      );
      return selector.GetOptimalVariant((type, pathId) =>
      {
        var stateChangeActions = _stateChangeRecalculator.AddNewUnit(field, type, new UnitCreationOption{PathId = pathId});
        var stateCalc = new StateCalculator(_statsLibrary, field, stateChangeActions);
        stateCalc.CalculateActionsByTicks();
        return (double) (field.State.Castle.Health - stateCalc.Field.State.Castle.Health) /*/ _statsLibrary.GetStats(type).Cost*/;
      });
    }
  }
}
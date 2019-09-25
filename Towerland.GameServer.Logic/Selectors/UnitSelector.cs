using System.Linq;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

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
      var fieldClone = (Field) field.Clone();
      var selector = new IntelligentGameObjectSelector<int>(
        _statsProvider.GetUnitStats().Where(u => u.Cost <= fieldClone.State.MonsterMoney).Select(u => u.Type),
        fieldClone.StaticData.Path.Select((p, idx) => idx)
      );
      return selector.GetOptimalVariant((type, pathId) =>
      {
        _stateChangeRecalculator.AddNewUnit(fieldClone, type, new CreationOptions{PathId = pathId});
        var stateCalc = new StateCalculator(_statsLibrary, fieldClone);
        stateCalc.CalculateActionsByTicks();
        return (double) (fieldClone.State.Castle.Health - stateCalc.Field.State.Castle.Health) / _statsLibrary.GetStats(type).Cost;
      });
    }
  }
}
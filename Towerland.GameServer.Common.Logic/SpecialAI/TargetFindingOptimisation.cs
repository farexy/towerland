using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.SpecialAI
{
  public class TargetFindingOptimisation : ITargetFindingOptimizer
  {
    private readonly IStatsLibrary _statsLib;
    private readonly AdditiveConvolutionCalculator _additiveConvolutionCalculator;
    private readonly GameCalculator _gameCalculator;

    private static readonly double[] OptimalTargetCriteriaWeights =
    {
      0.6, // first criteria - chance to optimally kill unit (the more resulted negative health of unit close to 0, the better, if health is positive - ignore)
      0.2, // second criteria - ??
      0.2, // third criteria - optimality of units defence type for tower (the more result damage close to stats tower damage, the better)
    };

    public TargetFindingOptimisation(IStatsLibrary statsLibrary)
    {
      _additiveConvolutionCalculator = new AdditiveConvolutionCalculator(OptimalTargetCriteriaWeights);
      _statsLib = statsLibrary;
      _gameCalculator = new GameCalculator(statsLibrary);
    }


    public int GetOptimalTarget(Field field, Tower tower)
    {
      var possibleTargets = field.FindePossibleTargetsForTower(tower, _statsLib);
      var tableToAnalize = new double[possibleTargets.Length, _additiveConvolutionCalculator.NumberOfCriterias];
      var towerStats = _statsLib.GetTowerStats(tower.Type);

      for (int i = 0; i < possibleTargets.Length; i++)
      {
        var unit = (Unit)field[possibleTargets[i]];

        tableToAnalize[i, 0] = 1 - GetKilledUnitHealthDeltaWithZero(towerStats, unit);
        tableToAnalize[i, 1] = 0;
        tableToAnalize[i, 2] = _gameCalculator.CalculateDamage(unit.Type, towerStats) / (double)towerStats.Damage;
      }
      return possibleTargets[_additiveConvolutionCalculator.FindOptimalVariantIndex(tableToAnalize)];
    }

    private double GetKilledUnitHealthDeltaWithZero(TowerStats stats, Unit unit)
    {
      var damage = _gameCalculator.CalculateDamage(unit.Type, stats);
      var resultedHealth = unit.Health - damage;
      return resultedHealth > 0 ? 1 : 0 - (double)resultedHealth / damage;
    }
  }
}
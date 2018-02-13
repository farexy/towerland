using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Common.Logic.SpecialAI
{
  public class TargetFindingOptimisation : ITargetFindingOptimizer
  {
    private readonly IStatsLibrary _statsLib;
    private readonly AdditiveConvolutionCalculator _additiveConvolutionCalculator;
    
    private static readonly double[] OptimalTargetCriteriaWeights =
    {
      0.1, // first criteria - number of towers on the path
      0.5, // second criteria - total damage of towers on the path for the monster
      0.3, // third criteria - remoteness of towers from beginning (length of the possible path)
      0.1, // fourth criteria - ratio of towers with special coef
    };
    
    public TargetFindingOptimisation(IStatsLibrary statsLibrary)
    {
      _additiveConvolutionCalculator = new AdditiveConvolutionCalculator(OptimalTargetCriteriaWeights);
      _statsLib = statsLibrary;
    }


  }
}
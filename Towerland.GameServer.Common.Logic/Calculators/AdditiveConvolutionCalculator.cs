using Towerland.GameServer.Common.Models.Exceptions;

namespace Towerland.GameServer.Common.Logic.Calculators
{
  public class AdditiveConvolutionCalculator
  {
    private readonly double[] _criteriaCoeffs;
    
    public AdditiveConvolutionCalculator(double[] criteriaCoeffs)
    {
      NumberOfCriterias = criteriaCoeffs.Length;
      _criteriaCoeffs = criteriaCoeffs;
    }

    public int NumberOfCriterias { get; }

    public int FindOptimalVariantIndex(double[,] criteriasByVariants)
    {
      if (NumberOfCriterias != criteriasByVariants.GetLength(1))
      {
        throw new LogicException("Number of criterias to analize is invalid");
      }

      var numOfVariants = criteriasByVariants.GetLength(0);

      var maxWeight = double.MinValue;
      var maxWeightIndex = 0;

      for (int i = 0; i < numOfVariants; i++)
      {
        for (int j = 0; j < NumberOfCriterias; j++)
        {
          var weight = criteriasByVariants[i, j] * _criteriaCoeffs[j];
          if (weight > maxWeight)
          {
            maxWeight = weight;
            maxWeightIndex = i;
          }
        }
      }

      return maxWeightIndex;
    }
  }
}
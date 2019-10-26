using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Selectors
{
  public class IntelligentGameObjectSelector<TDest> where TDest : struct
  {
    private static readonly ILog LoggerAI = LogManager.GetLogger(Assembly.GetEntryAssembly(), "AI");

    private readonly IEnumerable<GameObjectType> _availableGameObjects;
    private readonly IEnumerable<TDest> _availableDestinations;

    public IntelligentGameObjectSelector(
      IEnumerable<GameObjectType> availableGameObjects, IEnumerable<TDest> availableDestinations)
    {
      _availableGameObjects = availableGameObjects;
      _availableDestinations = availableDestinations;
    }

    public (GameObjectType type, TDest destination)? GetOptimalVariant(Func<GameObjectType, TDest, double> func)
    {
      if (!_availableGameObjects.Any() || !_availableDestinations.Any())
      {
        return default;
      }
      var maxWeight = 0.0;
      var variants = new List<(GameObjectType type, TDest dest)>();
      foreach (var gameObject in _availableGameObjects)
      {
        LoggerAI.Info($"Processing {gameObject}");
        _availableDestinations.AsParallel().ForAll(dest =>
        {
          var weight = func(gameObject, dest);
          if (Math.Abs(weight - maxWeight) < double.Epsilon)
          {
            variants.Add((gameObject, dest));
          }

          if (weight > maxWeight)
          {
            variants.Clear();
            variants.Add((gameObject, dest));
            maxWeight = weight;
          }
        });
      }

//      if (Math.Abs(maxWeight) < double.Epsilon)
//      {
//        return GameMath.Rand.Next(5) == 0
//          ? variants[GameMath.Rand.Next(variants.Count)]
//          : default((GameObjectType type, TDest destination)?);
//      }

      return GameMath.Rand.Next(2) == 0
        ? variants[GameMath.Rand.Next(variants.Count)]
        : default((GameObjectType type, TDest destination)?);
    }
  }
}
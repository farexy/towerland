using System;
using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.Exceptions;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Factories
{
  public class SpecialEffectLogic
  {
    public Func<Unit, List<GameAction>, bool> AffectAppliedUnitEffect { get; set; }
    public Func<Unit, SpecialEffectLogicFactory.EffectLogicNeededData, List<GameAction>, bool> ApplyUnitMoveEffect { get; set; }
    public Action<TowerStats, Unit, List<GameAction>, int?> ApplyTowerAttackEffect { get; set; }
  }
  
  public class SpecialEffectLogicFactory
  {
    private static readonly Dictionary<EffectId, SpecialEffectLogic> Effects = new Dictionary<EffectId, SpecialEffectLogic>
    {
      [EffectId.None]= new SpecialEffectLogic
      {
        ApplyTowerAttackEffect = (towerStats, unit, actions, duration) => {}
      },

      //Tower Effects
      [EffectId.UnitFreezed] = new SpecialEffectLogic
      {
        AffectAppliedUnitEffect = (unit, actions) => { return false; },
        ApplyTowerAttackEffect = (towerStats, unit, actions, duration) =>
        {
          unit.WaitTicks *= SpecialEffect.FreezedSlowCoeff;
          unit.Effect = new SpecialEffect{Effect = EffectId.UnitFreezed, Duration = duration ?? 0};
          actions.Add(new GameAction{ActionId = ActionId.UnitFreezes, UnitId = unit.GameId, WaitTicks = duration ?? 0});
        }
      },
      [EffectId.Unit10xDamage_10PercentProbability] = new SpecialEffectLogic
      {
        ApplyTowerAttackEffect = (towerStats, unit, actions, duration) =>
        {
          if (GameMath.CalcProbableEvent(10))
          {
            unit.Health -= towerStats.Damage * 9;
            actions.Add(new GameAction
            {
              ActionId = ActionId.UnitRecievesDamage,
              UnitId = unit.GameId,
              Damage = towerStats.Damage * 9
            });
          }
        }
      },

      //Unit Effects
      [EffectId.ReviveDeadUnitsAtPreviousTick] = new SpecialEffectLogic
      {
        ApplyUnitMoveEffect = (unit, data, actions) =>
        {
          var diedUnitActions = default(IEnumerable<GameAction>);
          //var diedUnitActions = data.CalculatedTicks.LastOrDefault()?.Where(a => a.ActionId == ActionId.UnitDies);
          if (diedUnitActions == null || !diedUnitActions.Any())
          {
            return true;
          }

          var uFactory = new UnitFactory(data.StatsLibrary);
          foreach (var deadUnitAction in diedUnitActions)
          {
            var possiblePath = data.Field.GetPossiblePathIds(deadUnitAction.Position).ToArray();
            var pathId = possiblePath[GameMath.Rand.Next(possiblePath.Length)];
            var skeleton = uFactory.Create(GameObjectType.Unit_Skeleton, 
              new CreationOptions {Position = deadUnitAction.Position, PathId = pathId});
            var newUnitId = data.Field.AddGameObject(skeleton);
            actions.Add(new GameAction
            {
              ActionId = ActionId.UnitAppears,
              UnitId = newUnitId,
              Position = deadUnitAction.Position,
              GoUnit = (Unit) skeleton.Clone()
            });
          }

          actions.Add(new GameAction
          {
            ActionId = ActionId.UnitAppliesEffect_DarkMagic,
            UnitId = unit.GameId,
          });
          return false;
        }
      }
    };

    public SpecialEffectLogic GetEffectLogic(EffectId effectId)
    {
      if (!Effects.ContainsKey(effectId))
      {
        throw new LogicException("No effect with such id found");
      }
      return Effects[effectId];
    }

    public class EffectLogicNeededData
    {
      public EffectLogicNeededData(IStatsLibrary statsLibrary, Field field, List<List<GameAction>> calculatedTicks)
      {
        StatsLibrary = statsLibrary;
        Field = field;
        CalculatedTicks = calculatedTicks;
      }

      internal List<List<GameAction>> CalculatedTicks;
      internal IStatsLibrary StatsLibrary;
      internal Field Field;
    }
  }
}
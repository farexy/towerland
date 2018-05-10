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
    public Action<TowerStats, Unit, List<GameAction>, int?> ApplyTowerAttackEffect { get; set; }
    public Func<Unit, SpecialEffectLogicFactory.EffectLogicNeededData, List<GameAction>, int?, bool> ApplyUnitEffectOnMove { get; set; }
  }

  public class SpecialEffectLogicFactory
  {
    private static readonly Dictionary<EffectId, SpecialEffectLogic> Effects = new Dictionary<EffectId, SpecialEffectLogic>
    {
      [EffectId.None]= new SpecialEffectLogic
      {
        ApplyTowerAttackEffect = (towerStats, unit, actions, duration) => {}
      },
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
      [EffectId.ReviveUnitsAtNextPoint] = new SpecialEffectLogic
      {
        ApplyUnitEffectOnMove = (unit, data, actions, duration) =>
        {
          var path = data.Field.StaticData.Path[unit.PathId.Value];
          if (!path.TryGetNext(unit.Position, out var nextPoint))
          {
            return true;
          }
          var deadUnits = data.Field.FindUnitsAt(nextPoint, true);
          var uFactory = new UnitFactory(data.StatsLibrary);
          if (!deadUnits.Any())
          {
            return true;
          }
          foreach (var deadUnit in deadUnits)
          {
            var skeleton = uFactory.Create(GameObjectType.Unit_Skeleton, new CreationOptions {Position = deadUnit.Position});
            actions.Add(new GameAction{ActionId = ActionId.DeadUnitDisapears, UnitId = deadUnit.GameId});
            data.Field.RemoveGameObject(deadUnit.GameId);
            data.Field.AddGameObject(skeleton);
          }

          actions.Add(new GameAction
          {
            ActionId = ActionId.UnitAppliesEffect_DarkMagic,
            UnitId = unit.GameId,
            WaitTicks = 1
          });
          unit.WaitTicks = 1;
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
      internal IStatsLibrary StatsLibrary;
      internal Field Field;
    }
  }
}
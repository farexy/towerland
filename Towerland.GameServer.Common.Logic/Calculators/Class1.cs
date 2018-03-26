using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Common.Logic.Factories;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Calculators
{
    public class StateCalculator
    {
      private readonly IStatsLibrary _statsLib;
      private readonly SpecialEffectLogicFactory _effectLogicFactory;
      private readonly Field _field;
      private readonly MoneyProvider _moneyProvider;
      private readonly GameCalculator _gameCalculator;

      private const int NotFound = -1;
      private static readonly Point NotFoundPoint = new Point(-1, -1);

      public Field Field
      {
        get { return _field; }
      }

      public StateCalculator(IStatsLibrary statsLibrary, Field fieldState)
      {
        _statsLib = statsLibrary;
        _field = (Field)fieldState.Clone();

        _effectLogicFactory = new SpecialEffectLogicFactory();
        _moneyProvider = new MoneyProvider(statsLibrary);
        _gameCalculator = new GameCalculator(statsLibrary);
      }

      public void SetState(FieldState fieldState)
      {
        _field.SetState(fieldState);
      }

      public GameTick[] CalculateActionsByTicks()
      {
        var ticks = new List<List<GameAction>>(40);
        while (_field.State.Castle.Health > 0
          && _field.State.Units.Any())
        {
          var actions = new List<GameAction>();

          actions.AddRange(CheckDeadUnits());
          actions.AddRange(GetUnitActions());
          actions.AddRange(GetTowerActions());

          ticks.Add(actions);
        }

        var result = new GameTick[ticks.Count];
        for (int i = 0; i < ticks.Count; i++)
        {
          result[i] = new GameTick
          {
            RelativeTime = i,
            Actions = ticks[i]
          };
        }
        return result;
      }

      private List<GameAction> GetUnitActions()
      {
        var actions = new List<GameAction>();
        var deadUnits = new List<Unit>();

        foreach (var unit in _field.State.Units)
        {
          if (unit.Health <= 0)
          {
            deadUnits.Add(unit);
            continue;
          }
          if (unit.WaitTicks != 0)
          {
            unit.WaitTicks--;
            continue;
          }
          if (unit.Effect != null && unit.Effect.Effect != EffectId.None)
          {
            unit.Effect.Duration -= 1;
            if (unit.Effect.Duration == 0)
            {
              unit.Effect = SpecialEffect.Empty;
              actions.Add(new GameAction {ActionId = ActionId.UnitEffectCanseled, UnitId = unit.GameId});
            }
          }

          var stats = _statsLib.GetUnitStats(unit.Type);

          if (!ApplyUnitEffect(stats, unit, actions))
          {
            continue;
          }
          var path = _field.StaticData.Path[unit.PathId.Value];
          if (path.End == unit.Position)
          {
            var attackAction = new GameAction
            {
              ActionId = ActionId.UnitAttacksCastle,
              UnitId = unit.GameId,
              Damage = stats.Damage
            };
            actions.Add(attackAction);
            _field.State.Castle.Health -= stats.Damage;
            deadUnits.Add(unit);

            var unitReward = _moneyProvider.GetUnitReward(_field, attackAction);
            _field.State.MonsterMoney += unitReward;
            actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});
          }
          else
          {
            var contextSpeedCoeff = GetContextSpeedCoeff(unit);
            var nextPoint = path.GetNext(unit.Position);
            unit.Position = nextPoint;
            unit.WaitTicks = stats.Speed * contextSpeedCoeff;
            actions.Add(new GameAction
            {
              ActionId = ActionId.UnitMoves,
              Position = nextPoint,
              UnitId = unit.GameId,
              WaitTicks = stats.Speed * contextSpeedCoeff
            });
          }
        }

        _field.MoveUnitsToDead(deadUnits);

        return actions;
      }

      private List<GameAction> GetTowerActions()
      {
        var actions = new List<GameAction>();

        foreach (var tower in _field.State.Towers)
        {
          if (tower.WaitTicks != 0)
          {
            tower.WaitTicks -= 1;
            continue;
          }

          var stats = _statsLib.GetTowerStats(tower.Type);

          switch (stats.Attack)
          {
            case TowerStats.AttackType.Usual:
            case TowerStats.AttackType.Magic:
              var targetId = FindTarget(_field, tower, stats);
              if (targetId != NotFound)
              {
                var unit = (Unit) _field[targetId];
                var damage = _gameCalculator.CalculateDamage(unit.Type, stats);
                tower.WaitTicks = stats.AttackSpeed;

                actions.Add(new GameAction
                {
                  ActionId = ActionId.TowerAttacks,
                  TowerId = tower.GameId,
                  UnitId = targetId,
                  WaitTicks = stats.AttackSpeed
                });
                actions.Add(new GameAction
                {
                  ActionId = ActionId.UnitRecievesDamage,
                  UnitId = targetId,
                  Damage = damage
                });

                ApplyTowerEffects(stats, unit, actions);
                unit.Health -= damage;
                if (unit.Health <= 0)
                {
                  var unitTrue = _field.State.Units.First(u => u.GameId == targetId);
                  var dieAction = new GameAction {ActionId = ActionId.UnitDies, UnitId = targetId, TowerId = tower.GameId};
                  var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = tower.GameId, UnitId = targetId, Position = unitTrue.Position};

                  actions.Add(dieAction);
                  actions.Add(killAction);
                  
                  var towerReward = _moneyProvider.GetTowerReward(_field, dieAction);
                  var unitReward = _moneyProvider.GetUnitReward(_field, killAction);

                  _field.State.MonsterMoney += unitReward;
                  _field.State.TowerMoney += towerReward;
                  
                  actions.Add(new GameAction{ActionId = ActionId.TowerPlayerRecievesMoney, Money = towerReward});
                  actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});
                  
                  _field.MoveUnitToDead(unit);
                }
              }
              break;

            case TowerStats.AttackType.Burst:
              var targetPoint = FindBurstTarget(_field, tower, stats);
              if (targetPoint != NotFoundPoint)
              {
                tower.WaitTicks = stats.AttackSpeed;
                
                actions.Add(new GameAction
                {
                  ActionId = ActionId.TowerAttacksPosition,
                  TowerId = tower.GameId,
                  Position = targetPoint,
                  Damage = stats.Damage,
                  WaitTicks = stats.AttackSpeed
                });
                var units = _field.FindUnitsAt(targetPoint);
//                foreach (var point in _field.GetNeighbourPoints(targetPoint, 1, FieldObject.Road))
//                {
//                  units = units.Union(_field.FindUnitsAt(point));
//                }
                var deadUnits = new List<Unit>();
                foreach (var unit in units)
                {
                  ApplyTowerEffects(stats, unit, actions);
                  var damage =  _gameCalculator.CalculateDamage(unit.Type, stats);
                  
                  if(damage == 0)
                    continue;
                  
                  actions.Add(new GameAction
                  {
                    ActionId = ActionId.UnitRecievesDamage,
                    UnitId = unit.GameId,
                    Damage = damage
                  });
                  unit.Health -= damage;
                  if (unit.Health <= 0)
                  {
                    var dieAction = new GameAction {ActionId = ActionId.UnitDies, UnitId = unit.GameId, TowerId = tower.GameId};
                    var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = tower.GameId, UnitId = unit.GameId, Position = unit.Position};
                  
                    actions.Add(dieAction);
                    actions.Add(killAction);
                  
                    var towerReward = _moneyProvider.GetTowerReward(_field, dieAction);
                    var unitReward = _moneyProvider.GetUnitReward(_field, killAction);

                    _field.State.MonsterMoney += unitReward;
                    _field.State.TowerMoney += towerReward;

                    actions.Add(new GameAction{ActionId = ActionId.TowerPlayerRecievesMoney, Money = towerReward});
                    actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});

                    deadUnits.Add(unit);
                  }
                }
                _field.MoveUnitsToDead(deadUnits);
              }
              break;
          }
        }

        return actions;
      }

      private List<GameAction> CheckDeadUnits()
      {
        var actions = new List<GameAction>();
        var unitsToRemove = new List<int>();
        foreach (var unit in _field.State.DeadUnits)
        {
          unit.WaitTicks--;
          if (unit.WaitTicks == 0)
          {
            actions.Add(new GameAction
            {
              ActionId = ActionId.DeadUnitDisapears,
              UnitId = unit.GameId
            });
            unitsToRemove.Add(unit.GameId);
          }
        }
        _field.RemoveMany(unitsToRemove);

        return actions;
      }

      #region Logic

      private static int GetContextSpeedCoeff(Unit unit)
      {
        return unit.Effect.Effect == EffectId.UnitFreezed ? SpecialEffect.FreezedSlowCoeff : 1;
      }

      private bool ApplyUnitEffect(UnitStats stats, Unit unit, List<GameAction> actions)
      {
        if (stats.SpecialEffects == null)
        {
          return true;
        }

        bool continueAfter = true;
        foreach (var effect in stats.SpecialEffects)
        {
          var effectLogic = _effectLogicFactory.GetEffectLogic(effect.Effect);
          var neededData = new SpecialEffectLogicFactory.EffectLogicNeededData
          {
            StatsLibrary = _statsLib,
            Field = _field
          };
          if (!effectLogic.ApplyUnitEffectOnMove(unit, neededData, actions, effect.Duration))
          {
            continueAfter = false;
          }
        }

        return continueAfter;
      }
      
      private void ApplyTowerEffects(TowerStats tower, Unit unit, List<GameAction> actions)
      {
        if (tower.SpecialEffects == null)
        {
          return;
        } 
        foreach (var effect in tower.SpecialEffects)
        {
          var effectLogic = _effectLogicFactory.GetEffectLogic(effect.Effect);
          effectLogic.ApplyTowerAttackEffect(tower, unit, actions, effect.Duration);
        }
      }
      
      private static int FindTarget(Field field, GameObject tower, TowerStats stats)
      {
        var x = tower.Position.X;
        var y = tower.Position.Y;
        for (int range = 1; range <= stats.Range; range++)
        {
          var units = new List<Unit>(field.FindUnitsAt(new Point(x + range, y)));
          
          var p = new Point(x, y + range);
          units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

          p = new Point(x, y - range);
          units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());
          
          p = new Point(x + range, y - range);
          units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());
          
          p = new Point(x + range, y);
          units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());
          
          p = new Point(x + range, y + range);
          units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

          p = new Point(x - range, y + range);
          units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());
          
          p = new Point(x - range, y);
          units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());
          
          p = new Point(x - range, y - range);
          units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());
            
          if (units.Any())
          {
            return units[GameMath.Rand.Next(units.Count)].GameId;
          }
        }

        return NotFound;
      }
      
      private static Point FindBurstTarget(Field field, GameObject tower, TowerStats stats)
      {
        return FindPointWithManyTargets(field, tower.Position, stats.Range);
      }
      
      private static Point FindPointWithManyTargets(Field field, Point center, int radius)
      {
        var x = center.X;
        var y = center.Y;
        
        var maxPoint = NotFoundPoint;
        var maxCount = 0;
        
        for (int rng = 1; rng <= radius; rng++)
        {          
          var p = new Point(x, y + rng);
          UpdMaxUnitsPoint(field, p, ref maxPoint, ref maxCount);

          p = new Point(x + rng, y + rng);
          UpdMaxUnitsPoint(field, p, ref maxPoint, ref maxCount);

          p = new Point(x - rng, y);
          UpdMaxUnitsPoint(field, p, ref maxPoint, ref maxCount);
          
          p = new Point(x, y - rng);
          UpdMaxUnitsPoint(field, p, ref maxPoint, ref maxCount);

          p = new Point(x - rng, y - rng);
          UpdMaxUnitsPoint(field, p, ref maxPoint, ref maxCount);
        }

        return maxPoint;
      }

      private static void UpdMaxUnitsPoint(Field field, Point p, ref Point maxPoint, ref int maxCount)
      {
        if (p != field.StaticData.Finish)
        {
          var countUnits = field.FindUnitsAt(p).Count();
          if (countUnits > maxCount)
          {
            maxCount = countUnits;
            maxPoint = p;
          }
        }
      }

      #endregion
    }
}

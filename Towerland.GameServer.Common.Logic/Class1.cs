using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Common.Models.Effects;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.Stats;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Logic.SpecialAI;

namespace Towerland.GameServer.Common.Logic
{
    public class StateCalculator
    {
      private readonly IStatsLibrary _statsLib;
      private readonly Field _field;

      private readonly MoneyProvider _moneyProvider;
      
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
        
        _moneyProvider = new MoneyProvider(_field, statsLibrary);
      }

      public void SetState(FieldState fieldState)
      {
        _field.SetState(fieldState);
      }

      public GameTick[] CalculateActionsByTicks()
      {
        var ticks = new List<List<GameAction>>(100);
        while (_field.State.Castle.Health > 0
          && _field.State.Units.Any())
        {
          var actions = new List<GameAction>();

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
        var unitsToRemove = new List<int>();

        foreach (var unit in _field.State.Units)
        {
          if (unit.WaitTicks != 0)
          {
            unit.WaitTicks -= 1;
            continue;
          }
          if (unit.Effect != null && unit.Effect.Effect != EffectId.None)
          {
            unit.Effect.Duration -= 1;
            if (unit.Effect.Duration == 0)
            {
              unit.Effect = SpecialEffect.Empty;
              actions.Add(new GameAction{ActionId = ActionId.UnitEffectCanseled, UnitId = unit.GameId});
            }
          }

          var path = _field.StaticData.Path[unit.PathId.Value];
          var stats = _statsLib.GetUnitStats(unit.Type);
          if (path.End == unit.Position)
          {
            actions.Add(new GameAction { ActionId = ActionId.UnitAttacksCastle, UnitId = unit.GameId, Damage = stats.Damage });
            _field.State.Castle.Health -= stats.Damage;
            unitsToRemove.Add(unit.GameId);
          }
          else
          {
            var effectSpeedCoeff = unit.Effect != null && unit.Effect.Effect == EffectId.UnitFreezed ? SpecialEffect.FreezedSlowCoeff : 1;
            var nextPoint = path.GetNext(unit.Position);
            unit.Position = nextPoint;
            unit.WaitTicks = stats.Speed * effectSpeedCoeff;
            actions.Add(new GameAction
            {
              ActionId = ActionId.UnitMoves,
              Position = nextPoint,
              UnitId = unit.GameId,
              WaitTicks = stats.Speed * effectSpeedCoeff
            });
          }        
        }

        _field.RemoveMany(unitsToRemove);

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
                var damage = CalculateDamage(_statsLib.GetUnitStats(unit.Type), stats);
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
                  var dieAction = new GameAction {ActionId = ActionId.UnitDies, UnitId = targetId, TowerId = tower.GameId};
                  var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = tower.GameId, UnitId = targetId};
                  
                  actions.Add(dieAction);
                  actions.Add(killAction);
                  
                  var towerReward = _moneyProvider.GetTowerReward(dieAction);
                  var unitReward = _moneyProvider.GetUnitReward(killAction);

                  _field.State.MonsterMoney += unitReward;
                  _field.State.TowerMoney += towerReward;
                  
                  actions.Add(new GameAction{ActionId = ActionId.TowerPlayerRecievesMoney, Money = towerReward});
                  actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});
                  
                  _field.RemoveGameObject(targetId);
                }
              }
              else
              {
                actions.Add(new GameAction {ActionId = ActionId.TowerSearches, TowerId = tower.GameId});
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
                foreach (var unit in _field.FindUnitsAt(targetPoint))
                {
                  ApplyTowerEffects(stats, unit, actions);
                  var damage = CalculateDamage(_statsLib.GetUnitStats(unit.Type), stats);
                  
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
                    var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = tower.GameId, UnitId = unit.GameId};
                  
                    actions.Add(dieAction);
                    actions.Add(killAction);
                  
                    var towerReward = _moneyProvider.GetTowerReward(dieAction);
                    var unitReward = _moneyProvider.GetUnitReward(killAction);

                    _field.State.MonsterMoney += unitReward;
                    _field.State.TowerMoney += towerReward;
                  
                    actions.Add(new GameAction{ActionId = ActionId.TowerPlayerRecievesMoney, Money = towerReward});
                    actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});
                  
                    _field.RemoveGameObject(unit.GameId);
                  }
                }
              }
              else
              {
                actions.Add(new GameAction { ActionId = ActionId.TowerSearches, TowerId = tower.GameId });
              }
              break;
          }
        }

        return actions;
      }

      #region Logic
      
      private int CalculateDamage(UnitStats unit, TowerStats tower)
      {
        if(tower.Attack == TowerStats.AttackType.Burst && unit.IsAir)
          return 0;

        return GameMath.Round(tower.Damage * _statsLib.GetDefenceCoeff(unit.Defence, tower.Attack));
      }
      
      private static void ApplyTowerEffects(TowerStats tower, Unit unit, List<GameAction> actions)
      {
        if (tower.SpecialEffects == null)
        {
          return;
        } 
        foreach (var effect in tower.SpecialEffects)
        {
          switch (effect.Effect)
          {
              case EffectId.UnitFreezed:
                unit.Effect = new SpecialEffect{Effect = EffectId.UnitFreezed, Duration = effect.Duration};
                actions.Add(new GameAction{ActionId = ActionId.UnitFreezes, UnitId = unit.GameId, WaitTicks = effect.Duration});
                break;
              case EffectId.Unit10xDamage_10PercentProbability:
                if (GameMath.CalcProbableEvent(10))
                {
                  unit.Health -= tower.Damage * 9;
                  actions.Add(new GameAction
                  {
                    ActionId = ActionId.UnitRecievesDamage,
                    UnitId = unit.GameId,
                    Damage = tower.Damage * 9
                  });
                }
                break;
          }
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

          p = new Point(x + range, y + range);
          units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());

          p = new Point(x - range, y);
          units.AddRange(p != field.StaticData.Finish ? field.FindUnitsAt(p) : Enumerable.Empty<Unit>());
          
          p = new Point(x, y - range);
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
          var countUnits = field.FindUnitsAt(p).Length;
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

using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Common.Logic.Factories;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Logic.SpecialAI;
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
      private readonly TargetFinder _targetFinder;
      
      private List<List<GameAction>> Ticks { set; get; }
      
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
        _targetFinder = new TargetFinder(statsLibrary);
      }

      public void SetState(FieldState fieldState)
      {
        _field.SetState(fieldState);
      }

      public GameTick[] CalculateActionsByTicks()
      {
        Ticks = new List<List<GameAction>>(40);
        while (_field.State.Castle.Health > 0
          && _field.State.Units.Any())
        {
          var actions = new List<GameAction>();

          actions.AddRange(GetUnitActions());
          actions.AddRange(GetTowerActions());

          Ticks.Add(actions);
        }
        GetActionsAfterCalculation();

        var result = new GameTick[Ticks.Count];
        for (int i = 0; i < Ticks.Count; i++)
        {
          result[i] = new GameTick
          {
            RelativeTime = i,
            Actions = Ticks[i]
          };
        }
        return result;
      }

      private List<GameAction> GetUnitActions()
      {
        var actions = new List<GameAction>();
        var unitsToRemove = new List<int>();

        foreach (var unit in _field.State.Units.ToArray())
        {
          if (unit.Health <= 0)
          {
            unitsToRemove.Add(unit.GameId);
            continue;
          }
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
              actions.Add(new GameAction {ActionId = ActionId.UnitEffectCanseled, UnitId = unit.GameId});
            }
          }

          var path = _field.StaticData.Path[unit.PathId.Value];
          var stats = _statsLib.GetUnitStats(unit.Type);
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
            unitsToRemove.Add(unit.GameId);

            var unitReward = _moneyProvider.GetUnitReward(_field, attackAction);
            _field.State.MonsterMoney += unitReward;
            actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});
          }
          else
          {
            ApplyUnitEffects(unit, actions);
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
              var targetId = _targetFinder.FindTarget(_field, tower);
              if (targetId.HasValue)
              {
                var unit = (Unit) _field[targetId.Value];
                var damage = _gameCalculator.CalculateDamage(unit.Type, stats);
                tower.WaitTicks = stats.AttackSpeed;

                actions.Add(new GameAction
                {
                  ActionId = ActionId.TowerAttacks,
                  TowerId = tower.GameId,
                  UnitId = targetId.Value,
                  WaitTicks = stats.AttackSpeed
                });
                actions.Add(new GameAction
                {
                  ActionId = ActionId.UnitRecievesDamage,
                  UnitId = targetId.Value,
                  Damage = damage
                });

                ApplyTowerEffects(stats, unit, actions);
                unit.Health -= damage;
                if (unit.Health <= 0)
                {
                  var unitTrue = _field.State.Units.First(u => u.GameId == targetId);
                  var dieAction = new GameAction {ActionId = ActionId.UnitDies, UnitId = targetId.Value, TowerId = tower.GameId, Position = unitTrue.Position};
                  var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = tower.GameId, UnitId = targetId.Value, Position = unitTrue.Position};

                  actions.Add(dieAction);
                  actions.Add(killAction);

                  var towerReward = _moneyProvider.GetTowerReward(_field, dieAction);
                  var unitReward = _moneyProvider.GetUnitReward(_field, killAction);

                  _field.State.MonsterMoney += unitReward;
                  _field.State.TowerMoney += towerReward;

                  actions.Add(new GameAction{ActionId = ActionId.TowerPlayerRecievesMoney, Money = towerReward});
                  actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});

                  _field.RemoveGameObject(targetId.Value);
                }
              }
              break;

            case TowerStats.AttackType.Burst:
              var targetPoint = _targetFinder.FindBurstTarget(_field, tower);
              if (targetPoint.HasValue)
              {
                tower.WaitTicks = stats.AttackSpeed;

                actions.Add(new GameAction
                {
                  ActionId = ActionId.TowerAttacksPosition,
                  TowerId = tower.GameId,
                  Position = targetPoint.Value,
                  Damage = stats.Damage,
                  WaitTicks = stats.AttackSpeed
                });
                var units = _field.FindUnitsAt(targetPoint.Value);
//                foreach (var point in _field.GetNeighbourPoints(targetPoint, 1, FieldObject.Road))
//                {
//                  units = units.Union(_field.FindUnitsAt(point));
//                }
                var deadUnits = new List<int>();
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
                    var dieAction = new GameAction {ActionId = ActionId.UnitDies, UnitId = unit.GameId, TowerId = tower.GameId, Position = unit.Position};
                    var killAction = new GameAction {ActionId = ActionId.TowerKills, TowerId = tower.GameId, UnitId = unit.GameId, Position = unit.Position};

                    actions.Add(dieAction);
                    actions.Add(killAction);

                    var towerReward = _moneyProvider.GetTowerReward(_field, dieAction);
                    var unitReward = _moneyProvider.GetUnitReward(_field, killAction);

                    _field.State.MonsterMoney += unitReward;
                    _field.State.TowerMoney += towerReward;

                    actions.Add(new GameAction{ActionId = ActionId.TowerPlayerRecievesMoney, Money = towerReward});
                    actions.Add(new GameAction{ActionId = ActionId.MonsterPlayerRecievesMoney, Money = unitReward});

                    deadUnits.Add(unit.GameId);
                  }
                }
                _field.RemoveMany(deadUnits);
              }
              break;
          }
        }

        return actions;
      }

      private void GetActionsAfterCalculation()
      {
        if (_field.State.Castle.Health <= 0)
        {
          Ticks.Add(new List<GameAction>
          {
            new GameAction{ActionId = ActionId.MonsterPlayerWins}
          });
        }
      }

      #region Logic

      private static int GetContextSpeedCoeff(Unit unit)
      {
        return unit.Effect.Effect == EffectId.UnitFreezed ? SpecialEffect.FreezedSlowCoeff : 1;
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
      
      private void ApplyUnitEffects(Unit unit, List<GameAction> actions)
      {
        var stats = _statsLib.GetUnitStats(unit.Type);
        if (stats.SpecialEffects == null)
        {
          return;
        } 
        foreach (var effect in stats.SpecialEffects)
        {
          var effectLogic = _effectLogicFactory.GetEffectLogic(effect.Effect);
          effectLogic.ApplyUnitMoveEffect(unit, new SpecialEffectLogicFactory.EffectLogicNeededData(_statsLib, _field, Ticks), actions);
        }
      }
      

      
      

      #endregion
    }
}

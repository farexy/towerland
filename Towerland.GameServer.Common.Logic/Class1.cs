using System;
using System.Collections.Generic;
using System.Linq;
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
      private static readonly Random Random = new Random();

      private const int NotFound = -1;
      private static readonly Point NotFoundPoint = new Point(-1, -1);

      public StateCalculator(IStatsLibrary statsLibrary)
      {
        _statsLib = statsLibrary;
      }

      public GameTick[] CalculateActionsByTicks(Field fieldState)
      {
        var field = (Field)fieldState.Clone();
        var ticks = new List<List<GameAction>>(100);
        while (field.Castle.Health > 0
          && field.Units.Any())
        {
          var actions = new List<GameAction>();

          actions.AddRange(GetUnitActions(field));
          actions.AddRange(GetTowerActions(field));

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

      private List<GameAction> GetUnitActions(Field field)
      {
        var actions = new List<GameAction>();
        var unitsToRemove = new List<int>();

        foreach (var unit in field.Units)
        {
          if (unit.WaitTicks != 0)
          {
            unit.WaitTicks -= 1;
            continue;
          }

          var path = field.Path[unit.PathId.Value];
          var stats = _statsLib.GetUnitStats(unit.Type);

          if (path.End == unit.Position)
          {
            actions.Add(new GameAction { ActionId = ActionId.UnitAttacksCastle, UnitId = unit.GameId, Damage = stats.Damage });
            field.Castle.Health -= stats.Damage;
            unitsToRemove.Add(unit.GameId);
          }
          else
          {
            var nextPoint = path.GetNext(unit.Position);
            unit.Position = nextPoint;
            unit.WaitTicks = stats.Speed;
            actions.Add(new GameAction { ActionId = ActionId.UnitMoves, Position = nextPoint, UnitId = unit.GameId, WaitTicks = stats.Speed });
          }
        }

        field.RemoveMany(unitsToRemove);

        return actions;
      }

      private List<GameAction> GetTowerActions(Field field)
      {
        var actions = new List<GameAction>();

        foreach (var tower in field.Towers)
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
              var targetId = FindTarget(field, tower, stats);
              if (targetId != NotFound)
              {
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
                  Damage = stats.Damage
                });

                var unit = (Unit) field[targetId];
                unit.Health -= stats.Damage;
                if (unit.Health <= 0)
                {
                  actions.Add(new GameAction{ActionId = ActionId.UnitDies, UnitId = targetId});
                  field.RemoveGameObject(targetId);
                }
              }
              else
              {
                actions.Add(new GameAction {ActionId = ActionId.TowerSearches, TowerId = tower.GameId});
              }
              break;

            case TowerStats.AttackType.Burst:
              var targetPoint = FindBurstTarget(field, tower, stats);
              if (targetPoint != NotFoundPoint)
              {
                actions.Add(new GameAction
                {
                  ActionId = ActionId.TowerAttacks,
                  TowerId = tower.GameId,
                  Position = targetPoint,
                  Damage = stats.Damage,
                  WaitTicks = stats.AttackSpeed
                });
                foreach (var unit in field.FindUnitsAt(targetPoint))
                {
                  actions.Add(new GameAction
                  {
                    ActionId = ActionId.UnitRecievesDamage,
                    UnitId = unit.GameId,
                    Damage = stats.Damage
                  });
                  unit.Health -= stats.Damage;
                  if (unit.Health <= 0)
                  {
                    actions.Add(new GameAction { ActionId = ActionId.UnitDies, UnitId = unit.GameId });
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

      private static int FindTarget(Field field, Tower tower, TowerStats stats)
      {
        var x = tower.Position.X;
        var y = tower.Position.Y;
        for (int range = 1; range <= stats.Range; range++)
        {
          var units = new List<Unit>(field.FindUnitsAt(new Point(x + range, y)));
          units.AddRange(field.FindUnitsAt(new Point(x, y + range)));
          units.AddRange(field.FindUnitsAt(new Point(x + range, y + range)));
          units.AddRange(field.FindUnitsAt(new Point(x - range, y)));
          units.AddRange(field.FindUnitsAt(new Point(x, y - range)));
          units.AddRange(field.FindUnitsAt(new Point(x - range, y - range)));
          if (units.Any())
          {
            return units[Random.Next(units.Count)].GameId;
          }
        }

        return NotFound;
      }

      private static Point FindBurstTarget(Field field, Tower tower, TowerStats stats)
      {
        var unitId = FindTarget(field, tower, stats);
        return unitId != NotFound
          ? field[unitId].Position
          : NotFoundPoint;
      }

      #endregion  
    }
}

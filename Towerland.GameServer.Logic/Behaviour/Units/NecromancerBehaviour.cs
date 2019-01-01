using System.Linq;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Extensions;
using Towerland.GameServer.Logic.Factories;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Units
{
  public class NecromancerBehaviour : BaseUnitBehaviour
  {
    public NecromancerBehaviour(Unit unit, BattleContext battleContext, IStatsLibrary statsLibrary) : base(unit, battleContext, statsLibrary)
    {
    }

    public override bool ApplyPreActionEffect()
    {
      base.ApplyPreActionEffect();
      var deadUnitAction = FindDeadUnitAction();

      if (!deadUnitAction.HasValue)
      {
        return true;
      }
      var action = deadUnitAction.Value;
      var uFactory = new UnitFactory(StatsLibrary);
      var skill = StatsLibrary.GetSkill(Stats.Skill, Stats.Type);

      var possiblePath = Field.GetPossiblePathIds(action.Position).ToArray();
      var pathId = possiblePath[GameMath.Rand.Next(possiblePath.Length)];
      var newUnitId = Field.GenerateGameObjectId();
      var skeleton = uFactory.Create(GameObjectType.Unit_Skeleton,
        new CreationOptions {GameId = newUnitId, Position = action.Position, PathId = pathId});

      BattleContext.UnitsToAdd.Add(skeleton);

      BattleContext.AddAction(new GameAction
      {
        ActionId = ActionId.UnitRevives,
        UnitId = action.UnitId,
        UnitId2 = newUnitId,
        Position = action.Position
      });
      BattleContext.AddAction(new GameAction
      {
        ActionId = ActionId.UnitAppliesSkill,
        UnitId = Unit.GameId,
        WaitTicks = skill.WaitTicks,
        SkillId = skill.Id
      });

      return false;
    }

    private GameAction? FindDeadUnitAction()
    {
      var len = BattleContext.Ticks.Count;
      for (int i = len - 1; i >= 0 && i >= len - Stats.Speed; i--)
      {
        foreach (var action in BattleContext.Ticks[i])
        {
          if (action.ActionId == ActionId.TowerKills
              && !Field.State.RevivedUnits.OldIds.Contains(action.UnitId)
              && !Field.State.RevivedUnits.NewIds.Contains(action.UnitId))
          {
            return action;
          }
        }
      }

      return null;
    }
  }
}
using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Logic.Extensions;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Logic.Selectors;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Behaviour.Towers
{
    public class ShurikenAttackBehaviour : BaseTowerBehaviour
    {
        public ShurikenAttackBehaviour(Tower tower, BattleContext battleContext, IStatsLibrary statsLibrary) 
            : base(tower, battleContext, statsLibrary)
        {
        }

        public override void DoAction()
        {
            var target = new TargetFinder(StatsLibrary).FindTarget(BattleContext, Tower);

            using (var unitsEnumerator = FindAttackChain(target).GetEnumerator())
            {
                Unit prevUnit = null;
                while (unitsEnumerator.MoveNext())
                {
                    if (prevUnit == null)
                    {
                        BattleContext.AddAction(new GameAction
                        {
                            ActionId = ActionId.TowerAttacks,
                            TowerId = Tower.GameId,
                            UnitId = unitsEnumerator.Current.GameId,
                            WaitTicks = Stats.AttackSpeed
                        });
                    }
                    else
                    {
                        BattleContext.AddAction(new GameAction
                        {
                            ActionId = ActionId.ShurikenAttacks,
                            UnitId = unitsEnumerator.Current.GameId,
                            UnitId2 = prevUnit.GameId,
                        });
                    }
                    
                    DamageUnit(unitsEnumerator.Current);
                    prevUnit = unitsEnumerator.Current;
                }
            }
        }

        private IEnumerable<Unit> FindAttackChain(int? targetId)
        {
            if (!targetId.HasValue)
            {
                yield break;
            }

            var unit = Field[targetId.Value];
            var possiblePaths = Field.GetPossiblePathIds(unit.Position);
            Unit nextUnit = null;
            Path chainPath = null;
            bool moveBack = false;
            foreach (var pathId in possiblePaths)
            {
                var nextPoint = Field.GetPath(pathId).GetPrevious(unit.Position);
                nextUnit = Field.FindTargetsAt(nextPoint).FirstOrDefault();
                if (nextUnit != null)
                {
                    chainPath = Field.GetPath(pathId);
                    moveBack = true;
                    break;
                }
                nextPoint = Field.GetPath(pathId).GetNext(unit.Position);
                nextUnit = Field.FindTargetsAt(nextPoint).FirstOrDefault();
                if (nextUnit != null)
                {
                    chainPath = Field.GetPath(pathId);
                    break;
                }
            }

            while (nextUnit != null)
            {
                yield return nextUnit;
                nextUnit = moveBack
                    ? Field.FindTargetsAt(chainPath.GetPrevious(nextUnit.Position)).FirstOrDefault()
                    : Field.FindTargetsAt(chainPath.GetNext(nextUnit.Position)).FirstOrDefault();
            }
        }
    }
}
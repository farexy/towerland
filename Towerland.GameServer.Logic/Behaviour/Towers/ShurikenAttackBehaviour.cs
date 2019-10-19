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

            var chainIds = new HashSet<int>();
            var nextUnit = (Unit) Field[targetId.Value];

            while (nextUnit != null)
            {
                yield return nextUnit;
//                chainIds.Add(nextUnit.GameId);
//                
//                nextUnit = moveBack
//                    ? Field.FindTargetsAt(chainPath.GetPrevious(nextUnit.Position)).FirstOrDefault()
//                    : Field.FindTargetsAt(chainPath.GetNext(nextUnit.Position)).FirstOrDefault();
            }
        }
    }
}
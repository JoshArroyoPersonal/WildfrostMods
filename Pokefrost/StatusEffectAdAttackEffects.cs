using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectAddAttackEffects : StatusEffectData
    {
        public StatusEffectData effectToApply;
        public TargetConstraint[] attackerConstraints = new TargetConstraint[0];
        public override bool RunHitEvent(Hit hit)
        {
            if (hit.attacker == null || !Battle.IsOnBoard(hit.attacker) || hit.target == null || !hit.Offensive || !hit.BasicHit) { return false; }

            foreach(TargetConstraint constraint in targetConstraints)
            {
                if (!constraint.Check(hit.target))
                {
                    return false;
                }
            }
            foreach(TargetConstraint constraint in attackerConstraints)
            {
                if (!constraint.Check(hit.attacker))
                {
                    return false;
                }
            }

            hit.AddStatusEffect(new CardData.StatusEffectStacks(effectToApply, GetAmount()));

            return false;
        }
    }
}

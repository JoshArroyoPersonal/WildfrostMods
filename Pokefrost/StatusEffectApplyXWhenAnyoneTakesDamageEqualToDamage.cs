using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectApplyXWhenAnyoneTakesDamageEqualToDamage : StatusEffectApplyXWhenAnyoneTakesDamage
    {

        public int damageAmount;

        public override bool RunPostHitEvent(Hit hit)
        {
            if (target.enabled && target.alive && hit.Offensive && hit.damageType == targetDamageType)
            {
                damageAmount = hit.damageDealt;
                return Battle.IsOnBoard(target);
            }

            return false;
        }

        public override int GetAmount(Entity entity, bool equalAmount = false, int equalTo = 0)
        {
            return damageAmount;
        }

    }
}

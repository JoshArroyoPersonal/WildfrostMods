using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectImmuneToDamage : StatusEffectData
    {

        public List<string> immuneTypes;

        public override void Init()
        {
            base.OnHit += Check;
        }

        public override bool RunHitEvent(Hit hit)
        {
            if (hit.target == target && hit.Offensive && hit.canBeNullified && immuneTypes.Contains(hit.damageType))
            {
                return hit.damage > 0;
            }

            return false;
        }

        private IEnumerator Check(Hit hit)
        {
            SfxSystem.OneShot("event:/sfx/status_icon/block_decrease");
            hit.damageBlocked = hit.damage;
            hit.damage = 0;

            target.PromptUpdate();
            yield break;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public class StatusEffectJolted : StatusEffectData
    {
        public override void Init()
        {
            base.PostAttack += Check;
        }

        public override bool RunPostAttackEvent(Hit hit)
        {
            if (hit.attacker == target && hit.Offensive && hit.BasicHit)
            {
                return true;
            }

            return false;
        }

        public IEnumerator Check(Hit hit)
        {

            Hit hit2 = new Hit(target, hit.attacker, count)
            {
                canRetaliate = false,
                damageType = "jolt"
            };

            Pokefrost.VFX.TryPlayEffect("jolted", target.transform.position, 0.5f*target.transform.lossyScale);
            yield return hit2.Process();
        }
    }
}

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
            base.OnCardPlayed += Check;
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            if (entity == target)
            {
                return true;
            }

            return false;
        }

        public IEnumerator Check(Entity entity, Entity[] targets)
        {

            Hit hit2 = new Hit(entity, entity, count)
            {
                canRetaliate = false,
                damageType = "jolt"
            };

            Pokefrost.VFX.TryPlayEffect("jolt", target.transform.position, 0.5f*target.transform.lossyScale);
            Pokefrost.SFX.TryPlaySound("jolt");
            target.curveAnimator.Ping();
            yield return new WaitForSeconds(0.25f);
            yield return hit2.Process();
        }
    }
}

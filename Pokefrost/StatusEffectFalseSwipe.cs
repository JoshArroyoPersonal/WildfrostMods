using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectFalseSwipe : StatusEffectData
    {
        public override void Init()
        {
            base.OnHit += Check;
        }

        public override bool RunHitEvent(Hit hit)
        {

            if (hit.attacker == target && !target.silenced)
            {
                UnityEngine.Debug.Log("attacking");
                return hit.damage >= hit.target.hp.current;
            }

            return false;
        }

        public IEnumerator Check(Hit hit)
        {
            while (hit.damage >= hit.target.hp.current && hit.target.hp.current > 0)
            {
                hit.damage--;
                hit.damageBlocked++;
            }

            target.PromptUpdate();

            yield return true;
        }
    }

    internal class StatusEffectResist : StatusEffectData
    {
        public override void Init()
        {
            base.OnHit += Check;
        }

        public override bool RunHitEvent(Hit hit)
        {
            if (hit.target == target)
            {
                return hit.damage > 0;
            }

            return false;
        }

        public IEnumerator Check(Hit hit)
        {
            int countflag = 0;
            while (hit.damage > 0 && countflag < GetAmount())
            {
                hit.damage--;
                hit.damageBlocked++;
                countflag++;
            }

            target.PromptUpdate();

            yield return true;
        }
    }


}

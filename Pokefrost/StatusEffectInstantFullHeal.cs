using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public class StatusEffectInstantFullHeal : StatusEffectInstant
    {
        [SerializeField]
        public bool doPing = true;

        public override IEnumerator Process()
        {
            if (target.alive)
            {
                if (doPing)
                {
                    target.curveAnimator?.Ping();
                }

                Hit hit = new Hit(applier, target, -target.hp.max+target.hp.current);
                yield return hit.Process();
            }

            yield return base.Process();
        }
    }

    public class StatusEffectInstantRemoveCounter : StatusEffectInstant
    {

        public int cCount = 99;

        public override IEnumerator Process()
        {
            target.counter.current = cCount;
            target.counter.max = cCount;

            for (int j = target.statusEffects.Count - 1; j >= 0; j--)
            {

                if (target.statusEffects[j].GetType() == typeof(StatusEffectWhileActiveX))
                {
                    StatusEffectWhileActiveX activeEff = target.statusEffects[j] as StatusEffectWhileActiveX;
                    if (activeEff.active == true)
                    {
                        yield return activeEff.Deactivate();
                    }
                }

                yield return target.statusEffects[j].Remove();

            }

            target.PromptUpdate();
            yield return base.Process();
        }
    }
}

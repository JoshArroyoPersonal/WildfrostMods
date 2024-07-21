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
        public override IEnumerator Process()
        {
            target.counter.current = 99;
            target.counter.max = 0;
            target.silenceCount += 5000;
            target.PromptUpdate();
            yield return base.Process();
        }
    }
}

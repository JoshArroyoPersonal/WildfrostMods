using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectDuplicateEffect : StatusEffectApplyX
    {
        private int chain = 0;
        private int maxChain = 3;

        public override void Init()
        {
            base.PostApplyStatus += Copy;   
        }

        private IEnumerator Copy(StatusEffectApply apply)
        {
            chain++;
            if (chain == maxChain) { yield break; }

            effectToApply = apply.effectData;
            yield return Run(new List<Entity> { target }, apply.count);

            chain = 0;
        }

        public override bool RunPostApplyStatusEvent(StatusEffectApply apply)
        {
            if (!apply.applier || apply.applier == target || !apply.target || !apply.effectData || apply.count - apply.effectData.temporary <= 0  || target.silenced) { return false; }

            List<Entity> candidates = GetTargets();
            if (!candidates.Contains(apply.target)) { return false; }

            return true;
        }


    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rewired.Controller;

namespace Pokefrost
{
    internal class StatusEffectApplyXWhileStatused : StatusEffectData
    {

        [SerializeField]
        public StatusEffectData effectToGain;

        public int currentAmount;

        public bool active;

        public override bool HasEnableRoutine => true;

        public override bool HasDisableRoutine => true;

        public override bool HasPostHitRoutine => true;

        public override bool HasTurnEndRoutine => true;

        public override bool RunEnableEvent(Entity entity)
        {
            return entity == target;
        }

        public override IEnumerator EnableRoutine(Entity entity)
        {
            return Check();
        }

        public override bool RunDisableEvent(Entity entity)
        {
            if (entity == target)
            {
                return currentAmount != 0;
            }

            return false;
        }

        public override IEnumerator DisableRoutine(Entity entity)
        {
            return Deactivate();
        }

        public override bool RunPostHitEvent(Hit hit)
        {
            return hit.target == target;
        }

        public override IEnumerator PostHitRoutine(Hit hit)
        {
            return Check();
        }

        public override IEnumerator TurnEndRoutine(Entity entity)
        {
            return Check();
        }

        public IEnumerator Check()
        {
            if (!target.alive)
            {
                yield break;
            }

            bool flag = false;
            for (int num = target.statusEffects.Count - 1; num >= 0; num--)
            {
                StatusEffectData statusEffectData = target.statusEffects[num];
                if (statusEffectData.isStatus && statusEffectData.offensive && statusEffectData.visible)
                {
                    flag = true;
                    if (!active)
                    {
                        yield return Activate();
                    }
                    break;
                }
            }

            if (active && !flag)
            {
                yield return Deactivate();
            }
        }

        public IEnumerator Activate()
        {
            currentAmount = GetAmount();
            yield return StatusEffectSystem.Apply(target, target, effectToGain, currentAmount, temporary: true);
            active = true;
        }

        public IEnumerator Deactivate()
        {
            for (int num = target.statusEffects.Count - 1; num >= 0; num--)
            {
                StatusEffectData statusEffectData = target.statusEffects[num];
                if ((bool)statusEffectData && statusEffectData.name == effectToGain.name)
                {
                    yield return statusEffectData.RemoveStacks(currentAmount, removeTemporary: true);
                    break;
                }
            }

            currentAmount = 0;
            active = false;
        }

        public override bool RunEffectBonusChangedEvent()
        {
            if (target.enabled && active)
            {
                ActionQueue.Add(new ActionSequence(ReAffect()));
            }

            return false;
        }

        public IEnumerator ReAffect()
        {
            yield return Deactivate();
            yield return Activate();
        }

    }
}

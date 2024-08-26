using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    public class StatusEffectSpicune : StatusEffectData
    {
        public bool cardPlayed;

        public int current;

        public int amountToClear;

        public override void Init()
        {
            base.OnActionPerformed += ActionPerformed;
        }

        public override bool RunStackEvent(int stacks)
        {
            current += stacks;
            target.effectBonus += stacks;
            return false;
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            if (!cardPlayed && entity == target && count > 0)
            {
                cardPlayed = true;
                amountToClear = current;
            }

            return false;
        }

        public override bool RunActionPerformedEvent(PlayAction action)
        {
            if (cardPlayed)
            {
                return ActionQueue.Empty;
            }

            return false;
        }

        public IEnumerator ActionPerformed(PlayAction action)
        {
            cardPlayed = false;
            yield return Clear(amountToClear);
        }

        public IEnumerator Clear(int amount)
        {
            int amount2 = amount;
            Events.InvokeStatusEffectCountDown(this, ref amount2);
            if (amount2 != 0)
            {
                current -= amount2;
                target.effectBonus -= amount2;
                yield return CountDown(target, amount2);
            }
        }

        public override bool RunEndEvent()
        {
            target.effectBonus -= current;
            return false;
        }
    }
}

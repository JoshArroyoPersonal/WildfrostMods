using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Pokefrost
{
    public class StatusEffectSpicune : StatusEffectData
    {
        public bool cardPlayed;

        public int current;

        public int amountToClear;

        public static event UnityAction<Entity, int> OnJuiceCleared;

        public override void Init()
        {
            base.OnCardPlayed += CardPlayed;
            //base.OnActionPerformed += ActionPerformed;
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
                return true;
                
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
                if (count == 0)
                {
                    OnJuiceCleared?.Invoke(target, amount2);
                }
            }
        }

        public IEnumerator CardPlayed(Entity entity, Entity[] targets)
        {
            yield return Clear(amountToClear);
        }

        public override bool RunEndEvent()
        {
            target.effectBonus -= current;
            return false;
        }
    }
}

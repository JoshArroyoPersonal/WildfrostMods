using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectForetell : StatusEffectData
    {

        static StatusEffectData trigger = Pokefrost.instance.Get<StatusEffectData>("Trigger (High Prio)");

        public override void Init()
        {
            base.OnTurnEnd += Foresee;
            this.OnEnd += IKnewIt;
        }


        public IEnumerator Foresee(Entity entity)
        {

            int amount = 1;
            Events.InvokeStatusEffectCountDown(this, ref amount);
            if (amount != 0)
            {
                yield return CountDown(entity, amount);
            }
        }

        public IEnumerator IKnewIt()
        {

            if (target.IsAliveAndExists()) 
            {
                yield return StatusEffectSystem.Apply(target, target, trigger, 1);
            }

            yield break;
        }


    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    public class StatusEffectConvertEffects : StatusEffectInstant
    {
        //Converts status A => status B
        public string effectA = "";
        public string effectB = "";
        public bool swap = false;

        protected int stackA = 0;
        protected int stackB = 0;
        public override IEnumerator Process()
        {
            Routine.Clump clump = new Routine.Clump();
            for (int i = target.statusEffects.Count - 1; i >= 0; i--)
            {
                if (target.statusEffects[i]?.name == effectA)
                {
                    stackA = target.statusEffects[i].count;
                    clump.Add(target.statusEffects[i].Remove());

                }
                if (swap && target.statusEffects[i]?.name == effectB)
                {
                    stackB = target.statusEffects[i].count;
                    clump.Add(target.statusEffects[i].Remove());
                }
            }
            yield return clump.WaitForEnd();
            if (stackA > 0)
            {
                clump.Add(StatusEffectSystem.Apply(target, applier, Pokefrost.instance.Get<StatusEffectData>(effectB), stackA));
            }
            if (swap && stackB > 0)
            {
                clump.Add(StatusEffectSystem.Apply(target, applier, Pokefrost.instance.Get<StatusEffectData>(effectA), stackB));
            }
            yield return clump.WaitForEnd();
            yield return base.Process(); 
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public abstract class EntityCardScript : ScriptableObject
    {
        public abstract IEnumerator Run(Entity entity, int stack);
    }

    public class EntityCardScriptSwapTraits : EntityCardScript
    {
        protected TraitData traitA;

        protected TraitData traitB;

        public override IEnumerator Run(Entity target, int _)
        {
            int origStackA = 0;
            int origStackB = 0;
            foreach(Entity.TraitStacks stacks in target.traits)
            {
                if (stacks.data.name == traitA.name)
                {
                    origStackA = stacks.count - stacks.tempCount;
                    stacks.count -= origStackA;
                }
                if (stacks.data.name == traitB.name)
                {
                    origStackB = stacks.count - stacks.tempCount;
                    stacks.count -= origStackB;
                }
            }

            target.GainTrait(traitA, origStackB, temporary: false);
            target.GainTrait(traitB, origStackA, temporary: false);
            yield return target.UpdateTraits();

        }

        public static EntityCardScriptSwapTraits Create(TraitData traitA, TraitData traitB)
        {
            EntityCardScriptSwapTraits script = ScriptableObject.CreateInstance<EntityCardScriptSwapTraits>();
            script.traitA = traitA;
            script.traitB = traitB;
            return script;
        }
    }
}

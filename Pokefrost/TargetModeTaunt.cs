using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class TargetModeTaunt : TargetMode
    {
        public override Entity[] GetPotentialTargets(Entity entity, Entity target, CardContainer targetContainer)
        {
            HashSet<Entity> hashSet = new HashSet<Entity>();
            hashSet.AddRange(from e in entity.GetAllEnemies()
                             where (bool)e && e.enabled && e.alive && e.canBeHit && HasTaunt(e)
                             select e);
            if (hashSet.Count <= 0)
            {
                return null;
            }

            return hashSet.ToArray();
        }

        public override Entity[] GetSubsequentTargets(Entity entity, Entity target, CardContainer targetContainer)
        {
            return GetTargets(entity, target, targetContainer);
        }

        public bool HasTaunt(Entity entity)
        {
            foreach(CardData.TraitStacks t in entity.data.traits)
            {
                if(t.data.name == "Taunt")
                {
                    return true;
                }
            }
            return false;
        }

    }
}

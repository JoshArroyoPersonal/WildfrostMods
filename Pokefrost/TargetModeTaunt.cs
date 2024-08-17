using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class TargetModeTaunt : TargetMode
    {

        public string targetTrait = "Taunt";
        public bool missing = false;
        public bool failSafe = true;

        public override Entity[] GetPotentialTargets(Entity entity, Entity target, CardContainer targetContainer)
        {
            HashSet<Entity> hashSet = new HashSet<Entity>();
            hashSet.AddRange(from e in entity.GetAllEnemies()
                             where (bool)e && e.enabled && e.alive && e.canBeHit && HasTaunt(e)
                             select e);
            if (hashSet.Count <= 0 && failSafe)
            {
                TargetModeBasic targetModeBasic = ScriptableObject.CreateInstance<TargetModeBasic>();
                return targetModeBasic.GetPotentialTargets(entity, target, targetContainer);
            }

            return hashSet.ToArray();
        }

        public override Entity[] GetSubsequentTargets(Entity entity, Entity target, CardContainer targetContainer)
        {
            return GetTargets(entity, target, targetContainer);
        }

        public bool HasTaunt(Entity entity)
        {
            foreach(Entity.TraitStacks t in entity.traits)
            {
                if(t.data.name == targetTrait)
                {
                    if (missing)
                    {
                        return false;
                    }
                    return true;
                }
            }
            if (missing)
            {
                return true;
            }
            return false;
        }

    }


    internal class TargetModeStatus : TargetMode
    {

        public string targetType;
        public bool missing = false;
        public bool failSafe = false;

        public override Entity[] GetPotentialTargets(Entity entity, Entity target, CardContainer targetContainer)
        {
            HashSet<Entity> hashSet = new HashSet<Entity>();
            hashSet.AddRange(from e in entity.GetAllEnemies()
                             where (bool)e && e.enabled && e.alive && e.canBeHit && HasStatus(e)
                             select e);
            if (hashSet.Count <= 0 && failSafe)
            {
                TargetModeBasic targetModeBasic = ScriptableObject.CreateInstance<TargetModeBasic>();
                return targetModeBasic.GetPotentialTargets(entity, target, targetContainer);
            }

            return hashSet.ToArray();
        }

        public override Entity[] GetSubsequentTargets(Entity entity, Entity target, CardContainer targetContainer)
        {
            return GetTargets(entity, target, targetContainer);
        }

        public bool HasStatus(Entity entity)
        {
            foreach (StatusEffectData t in entity.statusEffects)
            {
                if (t.type == targetType)
                {
                    if (missing)
                    {
                        return false;
                    }
                    return true;
                }
            }
            if (missing)
            {
                return true;
            }
            return false;
        }

    }


}

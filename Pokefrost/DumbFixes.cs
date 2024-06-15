using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectWhileInHandXUpdate : StatusEffectWhileInHandX
    {

        public bool removeOnPlay = false;

        public override bool HasCardPlayedRoutine => removeOnPlay;

        public override IEnumerator BeginRoutine()
        {
            yield return base.BeginRoutine();
            target.display.promptUpdateDescription = true;
            target.PromptUpdate();
        }

        public override IEnumerator CardPlayedRoutine(Entity entity, Entity[] targets)
        {

            UnityEngine.Debug.Log("[Curse] Card Played");
            if(entity == target && removeOnPlay)
            {
                yield return CountDown(target, this.count);
                entity.statusEffects.Remove(this);
                target.display.promptUpdateDescription = true;
                target.PromptUpdate();
                UnityEngine.Debug.Log("[Curse] Removed... maybe");
            }
            yield break;
        }
    }
}

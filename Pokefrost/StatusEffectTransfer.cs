using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectTransfer : StatusEffectApplyXInstant
    {

        public override void Init()
        {
            base.OnBegin += Process;
            base.OnEnd += RemoveEffects;
        }

        public IEnumerator RemoveEffects()
        {
            if (this.effectToApply.GetType() == typeof(StatusEffectMultEffects))
            {
                StatusEffectMultEffects effs = this.effectToApply as StatusEffectMultEffects;
                for(int i = 0; i < effs.effects.Count; i++)
                {
                    for (int j = target.statusEffects.Count-1; j >= 0; j--) 
                    {
                        if (target.statusEffects[j].name == effs.effects[i].name)
                        {

                            if (target.statusEffects[j].GetType() == typeof(StatusEffectWhileActiveX))
                            {
                                StatusEffectWhileActiveX activeEff = target.statusEffects[j] as StatusEffectWhileActiveX;
                                if (activeEff.active == true)
                                {
                                    UnityEngine.Debug.Log("DEACTIVATING");
                                    yield return activeEff.Deactivate();
                                }
                            }

                            yield return target.statusEffects[j].RemoveStacks(GetAmount(), true);
                            break;
                        }

                    }
                }

                target.display.promptUpdateDescription = true;
                target.PromptUpdate();
            }
        }


    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectRevive : StatusEffectData
    {

        public float healthFactor = 0.5f;

        public float damageFactor = 0.5f;

        //public override bool preventDeath => true;

        public override void Init()
        {
            Events.OnEntityDisplayUpdated += EntityDisplayUpdated;
        }

        public void OnDestroy()
        {
            Events.OnEntityDisplayUpdated -= EntityDisplayUpdated;
        }
        public void EntityDisplayUpdated(Entity entity)
        {
            if (entity == target && target.hp.current <= 0 && !target.silenced)
            {
                target.PromptUpdate();
                ActionQueue.Stack(new ActionSequence(Animate()));
                target.damage.current = Mathf.CeilToInt((float)target.damage.max * damageFactor);
                target.hp.current = Mathf.CeilToInt((float)target.hp.max * healthFactor);
                CountDown();
                target.statusEffects.Remove(this);
                Events.OnEntityDisplayUpdated -= EntityDisplayUpdated;
                if (target.display is Card card)
                {
                    card.promptUpdateDescription = true;
                }
                target.PromptUpdate();
            }
            else if (entity == target && target.hp.current <= 0)
            {
                target.statusEffects.Remove(this);
                Events.OnEntityDisplayUpdated -= EntityDisplayUpdated;
                if (target.display is Card card)
                {
                    card.promptUpdateDescription = true;
                }
                target.PromptUpdate();
            }
        }

        public void CountDown()
        {
            foreach (CardData card in References.PlayerData.inventory.deck)
            {
                if (target.data.id == card.id)
                {
                    for (int i = 0; i < card.startWithEffects.Length; i++)
                    {
                        CardData.StatusEffectStacks stack = card.startWithEffects[i];
                        if (stack.data.name == name)
                        {
                            stack.count--;
                            if (stack.count == 0)
                            {
                                card.startWithEffects = card.startWithEffects.Where((item) => item != stack).ToArray();
                            }
                        }
                    }
                }
            }
        }

        public IEnumerator Animate()
        {

            ChangePhaseAnimationSystem animationSystem = UnityEngine.Object.FindObjectOfType<ChangePhaseAnimationSystem>();

            GameObject obj = new GameObject("focus");
            obj.transform.SetParent(animationSystem.container);
            obj.transform.position = target.transform.position;

            animationSystem.container = obj.transform;
            animationSystem?.Flash();
            yield return animationSystem.Focus(target);
            yield return Sequences.Wait(0.3f);
            ActionQueue.Stack(new ActionSequence(animationSystem.UnFocus())
            {
                note = "Unfocus boss",
                priority = 10
            }, fixedPosition: true);
            
            
        }

        IEnumerator blah()
        {
            target.containers[0].SetChildPosition(target);
            yield break;
        }

        public override bool RunBeginEvent()
        {
            target.display.promptUpdateDescription = true;
            target.PromptUpdate();
            return base.RunBeginEvent();
        }
    }
}

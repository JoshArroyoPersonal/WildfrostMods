using Deadpan.Enums.Engine.Components.Modding;
using Pokefrost;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public class StatusEffectBecomeOvershroom : StatusEffectData
    {
        public override bool HasApplyStatusRoutine => true;

        public override IEnumerator ApplyStatusRoutine(StatusEffectApply apply)
        {
            Debug.Log("[Test] ApplyStatusRoutine");
            if (apply != null && apply.applier?.owner == target.owner && apply.effectData?.offensive == true && (apply?.effectData.name == "Overload" || apply?.effectData.name == "Shroom"))
            {
                Debug.Log("[Test] found overload");
                apply.effectData = AddressableLoader.Get<StatusEffectData>("StatusEffectData", "Overshroom");
            }

            return null;
        }
    }

    public class StatusEffectOvershroom : StatusEffectData
    {
        [SerializeField]
        public CardAnimation buildupAnimation;

        public bool overloading;

        public bool subbed;

        public bool primed;

        public override bool HasPostApplyStatusRoutine => true;

        public StatusEffectData dummy1;

        public StatusEffectData dummy2;

        public StatusEffectData dummy3;

        public override void Init()
        {
            base.OnStack += Stack;
            Events.OnEntityDisplayUpdated += EntityDisplayUpdated;
            base.OnTurnEnd += DealDamage;
            Events.OnPostProcessUnits += Prime;
            subbed = true;
        }

        public void OnDestroy()
        {
            Events.OnEntityDisplayUpdated -= EntityDisplayUpdated;
            Unsub();
        }

        public void EntityDisplayUpdated(Entity entity)
        {
            if (entity == target && target.enabled)
            {
                Check();
            }
        }

        public IEnumerator Stack(int stacks)
        {

            bool flag = true;
            List<StatusEffectData> effectstoremove = new List<StatusEffectData>();
            foreach (StatusEffectData effect in target.statusEffects)
            {
                if ((effect.name == "Shroom" || effect.name == "Overload") && effect.offensive == true)
                {
                    count += effect.count;
                    effectstoremove.Add(effect);
                }
            }

            foreach (StatusEffectData effect in effectstoremove)
            {
                yield return effect.RemoveStacks(effect.count, false);
            }


            foreach (StatusEffectData effect in target.statusEffects)
            {
                if (effect.name == "Shroom" && effect.offensive == false)
                {
                    flag = false;
                    if (effect.count < count)
                    {
                        Debug.Log("[Overshroom 1] too little shroom");
                        yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy1, count - effect.count));
                        yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy2, count - effect.count));
                        break;
                    }
                }
            }
            if (flag)
            {
                Debug.Log("[Overshroom 2] start apply");
                yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy2, count), true);
                yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy1, count), true);
            }


            Check();
            yield return null;
        }

        public void Check()
        {
            if (count >= target.hp.current && !overloading)
            {
                ActionQueue.Stack(new ActionSequence(DealDamage(count))
                {
                    fixedPosition = true,
                    priority = eventPriority,
                    note = "Overload"
                });
                ActionQueue.Stack(new ActionSequence(Clear())
                {
                    fixedPosition = true,
                    priority = eventPriority,
                    note = "Clear Overload"
                });
                overloading = true;
            }
        }

        public override IEnumerator PostApplyStatusRoutine(StatusEffectApply apply)
        {
            foreach (StatusEffectData effect in target.statusEffects)
            {
                if (effect.name == "Shroom")
                {
                    if (effect.count > count)
                    {
                        Debug.Log("[Overshroom 2] too much shroom");
                        yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy1, effect.count - count));
                        count = effect.count;
                        break;
                    }
                }

                if (effect.name == "Overload")
                {
                    if (effect.count > count)
                    {
                        Debug.Log("[Overshroom 2] too much overload");
                        yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy2, effect.count - count));
                        count = effect.count;
                        break;
                    }
                }
            }
        }

        public IEnumerator DealDamage(int damage)
        {
            if (!this || !target || !target.alive)
            {
                yield break;
            }

            HashSet<Entity> targets = new HashSet<Entity>();
            CardContainer[] containers = target.containers;
            foreach (CardContainer collection in containers)
            {
                targets.AddRange(collection);
            }

            if ((bool)buildupAnimation)
            {
                yield return buildupAnimation.Routine(target);
            }

            Entity damager = GetDamager();
            Routine.Clump clump = new Routine.Clump();
            foreach (Entity item in targets)
            {
                Hit hit = new Hit(damager, item, damage)
                {
                    damageType = "overload"
                };
                clump.Add(hit.Process());
            }

            clump.Add(Sequences.Wait(0.5f));
            yield return clump.WaitForEnd();
        }

        public IEnumerator Clear()
        {
            if ((bool)this && (bool)target && target.alive)
            {
                yield return Remove();
                overloading = false;
            }
        }

        public void Unsub()
        {
            if (subbed)
            {
                Events.OnPostProcessUnits -= Prime;
                subbed = false;
            }
        }

        public void Prime(Character character)
        {
            primed = true;
            Unsub();
        }

        public override bool RunTurnEndEvent(Entity entity)
        {
            if (primed && target.enabled)
            {
                return entity == target;
            }

            return false;
        }

        public IEnumerator DealDamage(Entity entity)
        {
            int amount = 1;
            Events.InvokeStatusEffectCountDown(this, ref amount);
            if (amount != 0)
            {
                yield return CountDown(entity, amount);
            }
            Hit hit = new Hit(GetDamager(), target, count + 1)
            {
                screenShake = 0.25f,
                damageType = "shroom"
            };
            yield return hit.Process();
            yield return Sequences.Wait(0.2f);
        }

    }

    public class StatusEffectDummy : StatusEffectData
    {
        public bool dummy = true;
        public string truename = string.Empty;

        public override void Init()
        {
            temporary = 99;
            base.OnTurnEnd += Decrease;
        }

        public IEnumerator Decrease(Entity entity)
        {
            int amount = 1;
            Events.InvokeStatusEffectCountDown(this, ref amount);
            if (amount != 0)
            {
                yield return CountDown(entity, amount);
            }
        }


    }

    public class StatusEffectDreamDummy : StatusEffectData { }


    public class StatusEffectShiny : StatusEffectData
    {

        public override bool HasBeginRoutine => true;

        public override IEnumerator BeginRoutine()
        {
            Debug.Log(target?.name);
            if(target != null && target.data.name.Contains("websiteofsites.wildfrost.pokefrost"))
            {
                Debug.Log(target.data.name);
                Debug.Log(Pokefrost.instance.ToString());
                string[] splitName = target.data.name.Split('.');
                string trueName = splitName[3];
                string fileName = Pokefrost.instance.ImagePath("shiny_" + trueName + ".png");
                Debug.Log("shiny_" + trueName);
                if (!System.IO.File.Exists(fileName))
                {
                    Debug.Log("[Pokefrost] Oops, shiny file not found. Contact devs.");
                    yield break;
                }
                Sprite sprite = Pokefrost.instance.ImagePath("shiny_" + trueName + ".png").ToSprite();
                sprite.name = "shiny";
                target.data.mainSprite = sprite;
                target.GetComponent<Card>().mainImage.sprite = sprite;
            }
            yield break;
        }
    }

}
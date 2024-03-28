using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectSketch : StatusEffectInstant
    {
        public override IEnumerator Process()
        {
            yield return AddEffectCopies();
            if (applier.display is Card card)
            {
                card.promptUpdateDescription = true;
            }

            applier.PromptUpdate();
            yield return base.Process();
        }

        public IEnumerator AddEffectCopies()
        {
            CardData trueCopy = null;
            foreach(CardData card in References.PlayerData.inventory.deck)
            {
                if (card.id == applier.data.id)
                {
                    trueCopy = card;
                    break;
                }
            }
            List<StatusEffectData> list = target.statusEffects.Where((StatusEffectData e) => e.count > e.temporary && !e.isStatus && e != this && e.HasDescOrIsKeyword).ToList();
            foreach (Entity.TraitStacks trait in target.traits)
            {
                foreach (StatusEffectData passiveEffect in trait.passiveEffects)
                {
                    list.Remove(passiveEffect);
                }

                int num = trait.count - trait.tempCount;
                if (num > 0)
                {
                    applier.GainTrait(trait.data, num);
                    if (trueCopy  != null)
                    {
                        trueCopy.traits.Add(new CardData.TraitStacks(trait.data, num));
                    }
                }
            }

            foreach (StatusEffectData item in list)
            {
                if (trueCopy != null)
                {
                    trueCopy.startWithEffects = CardData.StatusEffectStacks.Stack(trueCopy.startWithEffects, new CardData.StatusEffectStacks[1] { new CardData.StatusEffectStacks(item, item.count - item.temporary) });
                }
                yield return StatusEffectSystem.Apply(applier, item.applier, item, item.count - item.temporary);
            }

            if (trueCopy != null)
            {
                trueCopy.attackEffects = CardData.StatusEffectStacks.Stack(trueCopy.attackEffects, (from a in target.attackEffects
                                                                                                    select a.Clone()).ToArray());
            }

            applier.attackEffects = (from a in CardData.StatusEffectStacks.Stack(applier.attackEffects, target.attackEffects)
                                     select a.Clone()).ToList();
            yield return applier.UpdateTraits();
        }
    }

    internal class StatusEffectSketchOnDeploy : StatusEffectApplyXWhenDeployed
    {
        public override bool RunEnableEvent(Entity entity)
        {
            if (base.RunEnableEvent(entity))
            {
                count--;
                if (count == 0)
                {
                    target.StartCoroutine("Remove");
                    
                }
                return true;
            }
            return false;
        }

        public override bool RunCardMoveEvent(Entity entity)
        {
            if (base.RunCardMoveEvent(entity))
            {
                count--;
                if (count == 0)
                {
                    target.StartCoroutine("Remove");

                }
                return true;
            }
            return false;
        }
    }
}

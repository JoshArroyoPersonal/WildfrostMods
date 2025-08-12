using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    public class StatusEffectInstantApplyXCardInDeck : StatusEffectInstantApplyEffect
    {
        public TargetConstraint[] constraints;
        public override IEnumerator Process()
        {
            CardDataList deck = References.PlayerData.inventory.deck;
            List<CardData> cards = deck.InRandomOrder().ToList();
            foreach (CardData card in cards)
            {
                //UnityEngine.Debug.Log($"[Pokefrost] {card.title}");
                if (!SatisfiesConstraints(card))
                {
                    continue;
                }
                UnityEngine.Debug.Log($"[Pokefrost] Upgrading {card.title}");
                card.startWithEffects = CardData.StatusEffectStacks.Stack(card.startWithEffects, new CardData.StatusEffectStacks[1] { new CardData.StatusEffectStacks(effectToApply, GetAmount()) });
                foreach (Entity entity in Battle.GetAllCards())
                {
                    if (entity.data.id == card.id)
                    {
                        yield return StatusEffectSystem.Apply(entity, target, effectToApply, GetAmount());  
                    }
                }
                break;
            }
            yield return Remove();
        }

        public bool SatisfiesConstraints(CardData data)
        {
            foreach(TargetConstraint constraint in constraints)
            {
                if (!constraint.Check(data))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

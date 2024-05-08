using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectChangeData : StatusEffectData
    {
        public CardData cardBase;
        public int keepIndex = 0;

        public override void Init()
        {
            base.OnTurnEnd += TurnStart;
        }

        private IEnumerator TurnStart(Entity entity)
        {
            if (entity == target)
            {
                ChangeCard();
                yield return UpdateData();
                target.PromptUpdate();
                yield return target.display.UpdateData(true);
            }
            yield break;
        }

        public IEnumerator RemoveData()
        {
            for (int i = target.traits.Count - 1; i >= 0; i--)
            {
                target.traits[i].count -= target.traits[i].tempCount;
                target.traits[i].tempCount = 0;
                yield return target.UpdateTraits(target.traits[i]);
            }
            for (int i = target.statusEffects.Count - 1; i >= 1; i--)
            {
                StatusEffectData status = target.statusEffects[i];
                yield return status.Remove();
            }
        }

        public void ChangeCard()
        {
            IEnumerable<CardData> cards = AddressableLoader.GetGroup<CardData>("CardData").InRandomOrder();
            foreach (CardData card in cards)
            {
                UnityEngine.Debug.Log($"[Test] {card.title}");
                if (card.cardType.name == "Item" && card.traits != null && !card.traits.Exists((CardData.TraitStacks b) => b.data.name == "Consume"))
                {
                    cardBase = card;
                    break;
                }
            }
        }

        public IEnumerator UpdateData()
        {
            for (int i = 0; i < 30; i++)
            {

                float r = Dead.PettyRandom.Range(0f, 1f);
            }
            yield return RemoveData();
            CardData trueData = target.data;
            target.damage.current = cardBase.damage;
            target.targetMode = cardBase.targetMode;
            trueData.mainSprite = cardBase.mainSprite;
            trueData.hasAttack = cardBase.hasAttack;
            //trueData.backgroundSprite = cardBase.backgroundSprite;
            trueData.canPlayOnBoard = cardBase.canPlayOnBoard;
            trueData.canPlayOnEnemy = cardBase.canPlayOnEnemy;
            trueData.canPlayOnHand = cardBase.canPlayOnHand;
            trueData.canPlayOnFriendly = cardBase.canPlayOnFriendly;
            trueData.damage = cardBase.damage;
            trueData.needsTarget = cardBase.needsTarget;
            trueData.playOnSlot = cardBase.playOnSlot;
            Card card = target.display as Card;
            card.SetName(cardBase.title);


            foreach (CardData.TraitStacks trait in cardBase.traits)
            {
                target.GainTrait(trait.data, trait.count, temporary: true);
            }
            yield return target.UpdateTraits();

            foreach (CardData.StatusEffectStacks item in cardBase.startWithEffects)
            {
                yield return StatusEffectSystem.Apply(target, null, item.data, item.count, fireEvents: false, temporary: true);
            }

            target.attackEffects = (from a in cardBase.attackEffects
                                    select a.Clone()).ToList();
        }
    }
}

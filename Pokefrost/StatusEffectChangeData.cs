using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectChangeData : StatusEffectData
    {
        public CardData cardBase;
        public Sprite sprite;
        public int keepHowMany = 5; //temp dream + trigger dreamer + destroy on discard + consume effect + dream morph

        public override void Init()
        {
            base.OnTurnEnd += TurnStart;
        }

        public override bool RunBeginEvent()
        {
            target.data.backgroundSprite = sprite;
            if (cardBase != null )
            {
                target.data.mainSprite = cardBase.mainSprite;
            }
            target.data.playType = Card.PlayType.Play;
            target.StartCoroutine(target.display.UpdateData(true));
            return false;
        }

        public override bool RunTurnEndEvent(Entity entity)
        {
            return (entity == target);
        }

        private IEnumerator TurnStart(Entity entity)
        {
            ChangeCard();
            yield return UpdateData();
            target.PromptUpdate();
            yield return target.display.UpdateData(true);
            Card card = target.display as Card;
            card.SetName(cardBase.title);
            yield break;
        }

        public IEnumerator RemoveData()
        {
            for (int i = target.traits.Count - 1; i >= 1; i--)
            {
                target.traits[i].count -= target.traits[i].tempCount;
                target.traits[i].tempCount = 0;
                yield return target.UpdateTraits(target.traits[i]);
            }
            for (int i = target.statusEffects.Count - 1; i >= keepHowMany; i--)
            {
                if (i >= target.statusEffects.Count) //In case while active removes an effect applied before while active (rare)
                {
                    continue;
                }

                StatusEffectData status = target.statusEffects[i];
                if (status is StatusEffectWhileActiveX whileActive)
                {
                    if (whileActive.active)
                    {
                        yield return whileActive.Deactivate(); //Could decrease count faster than expected
                    }
                }
                yield return status.Remove();
            }
        }

        public void ChangeCard()
        {
            IEnumerable<CardData> cards = InPettyRandomOrder(AddressableLoader.GetGroup<CardData>("CardData"));
            foreach (CardData card in cards)
            {
                Debug.Log($"[Test] {card.title}");
                //Must be an item, that is playable, that has a sprite, that does not have consume or recycle
                if (card.cardType.name == "Item" && card.playType != Card.PlayType.None && card.mainSprite?.name != "Nothing" && (card.traits == null  || !card.traits.Exists((CardData.TraitStacks b) => (b.data.name == "Consume" || b.data.name == "Recycle")) ))
                {
                    cardBase = card;
                    break;
                }
            }
        }

        public static IOrderedEnumerable<T> InPettyRandomOrder<T>(IEnumerable<T> source)
        {
            return source.OrderBy((T a) => Dead.PettyRandom.Range(0f, 1f));
        }

        public IEnumerator UpdateData()
        {
            yield return RemoveData();
            CardData trueData = target.data;
            target.damage.current = cardBase.damage;
            target.targetMode = cardBase.targetMode;
            trueData.mainSprite = cardBase.mainSprite;
            trueData.hasAttack = cardBase.hasAttack;
            //trueData.backgroundSprite = cardBase.backgroundSprite;
            trueData.targetConstraints = cardBase.targetConstraints;
            trueData.canPlayOnBoard = cardBase.canPlayOnBoard;
            trueData.canPlayOnEnemy = cardBase.canPlayOnEnemy;
            trueData.canPlayOnHand = cardBase.canPlayOnHand;
            trueData.canPlayOnFriendly = cardBase.canPlayOnFriendly;
            trueData.damage = cardBase.damage;
            trueData.needsTarget = cardBase.needsTarget;
            trueData.playOnSlot = cardBase.playOnSlot;
            trueData.titleFallback = cardBase.titleFallback;
            trueData.textKey = cardBase.textKey;
            trueData.textInsert = cardBase.textInsert;
            Card card = target.display as Card;
            card.SetName(cardBase.title);


            if (cardBase.traits != null)
            {
                foreach (CardData.TraitStacks trait in cardBase.traits)
                {
                    target.GainTrait(trait.data, trait.count, temporary: true);
                }
                yield return target.UpdateTraits();
            }

            foreach (CardData.StatusEffectStacks item in cardBase.startWithEffects)
            {
                yield return StatusEffectSystem.Apply(target, null, item.data, item.count, fireEvents: false, temporary: true);
            }

            target.attackEffects = (from a in cardBase.attackEffects
                                    select a.Clone()).ToList();
        }
    }

    internal class StatusEffectGiveDreamCard : StatusEffectApplyX
    {
        public override void Init()
        {
            Events.OnRedrawBellHit += GiveCard;
            base.OnCardMove += GiveCard2;
            base.Init();
        }

        public override bool RunCardMoveEvent(Entity entity)
        {
            return (entity == target && WasMovedOnToBoard(entity));
        }

        private IEnumerator GiveCard2(Entity entity)
        {
            return Run(GetTargets());
        }

        private void OnDestroy()
        {
            Events.OnRedrawBellHit -= GiveCard;
        }

        private void GiveCard(RedrawBellSystem arg0)
        {
            if (Battle.IsOnBoard(target))
            {
                ActionQueue.Add(new ActionSequence(Run(GetTargets())), fixedPosition: true);
            }
        }

        public static bool WasMovedOnToBoard(Entity entity)
        {
            if (Battle.IsOnBoard(entity))
            {
                return !Battle.IsOnBoard(entity.preContainers);
            }

            return false;
        }

    }
}

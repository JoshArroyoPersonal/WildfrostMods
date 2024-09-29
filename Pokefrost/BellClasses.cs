using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Xml.Linq;

namespace Pokefrost
{
    //This class contains scripts and modifier systems for bells
    public class BounceJuiceModifierSystem : GameSystem
    {
        public void OnEnable()
        {
            StatusEffectSpicune.OnJuiceCleared += BounceJuice;
        }

        public void OnDisable()
        {
            StatusEffectSpicune.OnJuiceCleared -= BounceJuice;
        }

        private void BounceJuice(Entity entity, int amount)
        {
            if (Battle.instance == null) { return; }

            List<Entity> list = Battle.GetCards(References.Player);
            TargetConstraintCanBeBoosted canBeBoosted = ScriptableObject.CreateInstance<TargetConstraintCanBeBoosted>();
            foreach(Entity e in list.InRandomOrder())
            {
                if (canBeBoosted.Check(e) && e != entity)
                {
                    ActionQueue.Stack(new ActionApplyStatus(e, null, Pokefrost.instance.Get<StatusEffectData>("Spicune"), 1));
                    break;
                }
            }
        }
    }

    public class AlwaysIgniteModifierSystem : GameSystem
    {

        StatusEffectData status = Pokefrost.instance.Get<StatusEffectData>("Burning");
        int count = 3;

        public void OnEnable()
        {
            Events.OnBattlePreTurnStart += ApplyIgnite;
        }

        public void OnDisable()
        {
            Events.OnBattlePreTurnStart -= ApplyIgnite;
        }

        private void ApplyIgnite(int turn)
        {
            if (Battle.instance == null) { return; }

            HashSet<Entity> enemies = Battle.GetAllUnits(Battle.instance.enemy);

            if (enemies.Count == 0) { return; }

            foreach (Entity entity in enemies)
            {
                if (entity.statusEffects.FirstOrDefault((s) => s.type == "burning") != null)
                {
                    return;
                }
            }

            Entity target = enemies.RandomItems(1)[0];

            ActionQueue.Stack(new ActionApplyStatus(target, null, status, count));


        }
    }


    public class GiveZoomlinModifierSystem : GameSystem
    {

        TargetConstraint[] targetConstraints = Pokefrost.instance.Get<StatusEffectData>("Temporary Zoomlin").targetConstraints;

        public void OnEnable()
        {
            Events.OnRedrawBellHit += Zoomin;
        }

        public void OnDisable()
        {
            Events.OnRedrawBellHit -= Zoomin;
        }

        private void Zoomin(RedrawBellSystem arg0)
        {
            StartCoroutine(DumbWait());
        }

        public IEnumerator DumbWait()
        {
            yield return new WaitUntil(() => ActionQueue.Empty);
            ActionQueue.Add(new ActionSequence(GiveZoomlin()));
        }

        public IEnumerator GiveZoomlin()
        {
            List<Entity> list = References.Player.handContainer.Where((e) =>
            {
                foreach (TargetConstraint targetConstraint in targetConstraints)
                {
                    if (!targetConstraint.Check(e))
                    {
                        return false;
                    }
                }

                return true;
                
            }).ToList();
            if (list.Count == 0) { yield break; }
            Entity rando = list.RandomItem();
            yield return StatusEffectSystem.Apply(rando, null, Pokefrost.instance.Get<StatusEffectData>("Temporary Zoomlin"), 1);
        }
    }


    public class DestoryCardSystem : GameSystem
    {

        Entity target;

        public void OnEnable()
        {
            Events.OnRedrawBellHit += DestoryCard;
        }

        public void OnDisable()
        {
            Events.OnRedrawBellHit -= DestoryCard;
        }

        private void DestoryCard(RedrawBellSystem arg0)
        {
            CardContainer handContainer = References.Player.handContainer;
            if ((object)handContainer != null && handContainer.Count > 0)
            {
                target = References.Player.handContainer[0];
                if (target != null)
                {
                    ActionQueue.Add(new ActionKill(target));
                }
                
            }
        }
    }

    public class RecallToTopModifierSystem : GameSystem
    {
        public enum Container
        {
            DrawPile,
            Hand,
            DiscardPile
        }

        public Container toContainer = Container.Hand;

        public void OnEnable()
        {
            Events.OnActionQueued += EntityDiscard;
        }

        public void OnDisable()
        {
            Events.OnActionQueued -= EntityDiscard;
        }

        public void EntityDiscard(PlayAction action)
        {
            if (action is ActionMove actionMove && actionMove.toContainers.Contains(References.Player.discardContainer) && Battle.IsOnBoard(actionMove.entity.containers) )
            {
                Debug.Log("[Pokefrost] "+actionMove.entity.containers.ToString());
                StartCoroutine(PutOnTop(actionMove.entity));
            }
        }

        private IEnumerator PutOnTop(Entity target)
        {
            Debug.Log("[Pokefrost] Here!");
            yield return new WaitUntil(() => ActionQueue.Empty);
            CardContainer cc = References.Player.drawContainer;
            int index = cc.Count;
            CardPocketSequence sequence = UnityEngine.Object.FindObjectOfType<CardPocketSequence>();
            CardPocketSequence.Card card = null;
            if (sequence != null)
            {
                int i = 0;
                while (sequence.cards.Count > 0)
                {
                    if (sequence.cards[i].entity == target)
                    {
                        card = sequence.cards[i];
                        target.transform.SetParent(MonoBehaviourSingleton<References>.instance.transform, worldPositionStays: true);
                        sequence.cards.RemoveAt(i);
                        break;
                    }

                    i++;
                }

                sequence.promptEnd = true;
                yield return new WaitUntil(() => !sequence.isActiveAndEnabled);
                card.Reset();
                card.Return();
                yield return new WaitForSeconds(0.25f);
            }

            if (cc.Contains(target))
            {
                index--;
            }


            yield return Sequences.CardMove(target, new CardContainer[1] { cc }, index);
            CardContainer[] preContainers = target.preContainers;
            foreach (CardContainer c in preContainers)
            {
                c.TweenChildPositions();
            }

            if (!target.preContainers.Contains(cc))
            {
                cc.TweenChildPositions();
            }

            yield break;
        }
    }


    public class ScriptRunScriptsOnDeckAlt : Script
    {
        [SerializeField]
        public CardScript[] scripts;

        [SerializeField]
        public TargetConstraint[] constraints;

        [SerializeField]
        public Vector2Int countRange;

        [SerializeField]
        public bool includeReserve;

        public override IEnumerator Run()
        {
            List<CardData> list = new List<CardData>();
            AddRangeIfConstraints(list, References.PlayerData.inventory.deck, constraints);
            if (includeReserve)
            {
                AddRangeIfConstraints(list, References.PlayerData.inventory.reserve, constraints);
            }

            if (list.Count > 0)
            {
                Affect(list);
            }

            yield break;
        }

        public static void AddRangeIfConstraints(ICollection<CardData> collection, CardDataList toAdd, TargetConstraint[] constraints)
        {
            foreach (CardData item in toAdd)
            {
                AddIfConstraints(collection, item, constraints);
            }
        }

        public static void AddIfConstraints(ICollection<CardData> collection, CardData item, TargetConstraint[] constraints)
        {
            if (!constraints.Any((TargetConstraint c) => !c.Check(item)))
            {
                collection.Add(item);
            }
        }

        public void Affect(IReadOnlyCollection<CardData> cards)
        {
            int num = countRange.Random();
            Debug.Log("[" + base.name + "] Affecting [" + string.Join(", ", cards) + "]");
            foreach (CardData item in cards.InRandomOrder())
            {
                foreach (CardScript item2 in scripts)
                {
                    item2.Run(item);
                }

                num--;
                if (num <= 0)
                {
                    break;
                }
            }
        }
    }
}

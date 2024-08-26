using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using static System.Net.WebRequestMethods;
using System.Reflection;

namespace Pokefrost
{
    public class CombineCardInBattleSystem : GameSystem
    {
        [Serializable]
        public struct Combo
        {
            public string[] cardNames;

            public string resultingCardName;

            public bool AllCardsInDeck(List<Entity> deck)
            {
                bool result = true;
                string[] array = cardNames;
                foreach (string cardName in array)
                {
                    if (!HasCard(cardName, deck))
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            }

            public List<Entity> FindCards(List<Entity> deck)
            {
                List<Entity> tooFuse = new List<Entity>();
                string[] array = cardNames;
                foreach (string cardName in array)
                {
                    foreach (Entity item in deck)
                    {
                        if (item.data.name == cardName)
                        {
                            tooFuse.Add(item);
                            break;
                        }
                    }
                }

                return tooFuse;
            }

            public bool HasCard(string cardName, List<Entity> deck)
            {
                foreach (Entity item in deck)
                {
                    if (item.data.name == cardName)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [SerializeField]
        public string combineSceneName;

        [SerializeField]
        public Combo[] combos;

        public void OnEnable()
        {
            Events.OnEntityCreated += EntityEnterHand;

            combineSceneName = "CardCombine";

            combos = new Combo[] {new Combo()
            {
                cardNames = new string[2] { "LuminSealant", "BrokenVase" },
                resultingCardName = "LuminVase"
            } };

        }

        public void OnDisable()
        {
            Events.OnEntityCreated -= EntityEnterHand;
        }

        public void EntityEnterHand(Entity entity/*, Entity summonedBy*/)
        {

            Combo[] array = combos;
            for (int i = 0; i < array.Length; i++)
            {
                Combo combo = array[i];
                List<Entity> fulldeck = new List<Entity>();
                fulldeck.Add(entity);
                fulldeck.AddRange(References.Player.handContainer.ToList());
                fulldeck.AddRange(References.Player.drawContainer.ToList());
                fulldeck.AddRange(References.Player.discardContainer.ToList());

                Debug.Log("[Pokefrost] " + fulldeck[0].name);

                if (combo.cardNames.Contains(entity.data.name) && combo.AllCardsInDeck(fulldeck))
                {

                    Debug.Log("[Pokefrost] Found Lumin Parts");

                    StopAllCoroutines();
                    //StartCoroutine(CombineSequence(combo, combo.FindCards(fulldeck)));
                    CombineAction action = new CombineAction();
                    action.combineSceneName = combineSceneName;
                    action.tooFuse = combo.FindCards(fulldeck);
                    action.combo = combo;
                    ActionQueue.Stack(action);
                    break;
                }
            }
        }


        public class CombineAction : PlayAction
        {

            [SerializeField]
            public string combineSceneName;

            public Combo combo;

            public List<Entity> tooFuse;

            public override IEnumerator Run()
            {
                return CombineSequence(combo, tooFuse);
            }

            public IEnumerator CombineSequence(Combo combo, List<Entity> tooFuse)
            {
                CombineCardSequence combineSequence = null;
                yield return SceneManager.Load(combineSceneName, SceneType.Temporary, delegate (Scene scene)
                {
                    combineSequence = scene.FindObjectOfType<CombineCardSequence>();
                });
                if ((bool)combineSequence)
                {
                    yield return combineSequence.Run2(tooFuse, combo.resultingCardName);
                }

                yield return SceneManager.Unload(combineSceneName);
            }

        }
        /*public IEnumerator CombineSequence(Combo combo, List<Entity> tooFuse)
        {
            CombineCardSequence combineSequence = null;
            yield return SceneManager.Load(combineSceneName, SceneType.Temporary, delegate (Scene scene)
            {
                combineSequence = scene.FindObjectOfType<CombineCardSequence>();
            });
            if ((bool)combineSequence)
            {
                yield return combineSequence.Run2(tooFuse, combo.resultingCardName);
            }

            yield return SceneManager.Unload(combineSceneName);
        }*/
    }

    public static class CombineCardSequenceExtension
    {
        public static IEnumerator Run2(this CombineCardSequence seq, List<Entity> cardsToCombine, string resultingCard)
        {
            CardData cardDataClone = AddressableLoader.GetCardDataClone(resultingCard);
            CardUpgradeData upgrade = Pokefrost.instance.Get<CardUpgradeData>("CardUpgradeHunger").Clone();
            upgrade.Assign(cardDataClone);

            yield return Run2(seq, cardsToCombine.ToArray(), cardDataClone);
        }

        public static IEnumerator Run2(this CombineCardSequence seq, Entity[] entities, CardData finalCard)
        {
            //CinemaBarSystem.State cinemaBarState = new CinemaBarSystem.State();

            PauseMenu.Block();
            //CinemaBarSystem.SetSortingLayer("UI2", 100);
            //CinemaBarSystem.In();
            Card card = CardManager.Get(finalCard, Battle.instance.playerCardController, References.Player, inPlay: false, isPlayerCard: true);
            card.transform.localScale = Vector3.one * 1f;
            card.transform.SetParent(seq.finalEntityParent);
            Entity finalEntity = card.entity;
            Routine.Clump clump = new Routine.Clump();
            Entity[] array = entities;
            foreach (Entity entity in array)
            {
                clump.Add(entity.display.UpdateData());
            }

            clump.Add(finalEntity.display.UpdateData());
            clump.Add(Sequences.Wait(0.5f));
            yield return clump.WaitForEnd();

            array = entities;
            foreach (Entity entity2 in array)
            {
                entity2.RemoveFromContainers();
            }

            array = entities;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].transform.localScale = Vector3.one * 0.8f;
            }

            seq.fader.In();
            Vector3 zero = Vector3.zero;
            array = entities;
            foreach (Entity entity3 in array)
            {
                zero += entity3.transform.position;
            }

            zero /= (float)entities.Length;

            seq.group.position = zero;
            array = entities;
            foreach (Entity entity4 in array)
            {
                Transform transform = UnityEngine.Object.Instantiate(seq.pointPrefab, entity4.transform.position, Quaternion.identity, seq.group);
                transform.gameObject.SetActive(value: true);
                entity4.transform.SetParent(transform);
                entity4.flipper.FlipUp();
                seq.points.Add(transform);
                LeanTween.alphaCanvas(((Card)entity4.display).canvasGroup, 1f, 0.4f).setEaseInQuad();
            }

            foreach (Transform point in seq.points)
            {
                LeanTween.moveLocal(to: point.localPosition.normalized, gameObject: point.gameObject, time: 0.4f).setEaseInQuart();
            }

            yield return new WaitForSeconds(0.4f);

            //seq.Flash(0.5f);
            Events.InvokeScreenShake(1f, 0f);
            array = entities;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].wobbler.WobbleRandom();
            }

            //seq.hitPs.Play();
            foreach (Transform point2 in seq.points)
            {
                LeanTween.moveLocal(to: point2.localPosition.normalized * 3f, gameObject: point2.gameObject, time: 1f).setEase(seq.bounceCurve);
            }

            LeanTween.moveLocal(seq.group.gameObject, new Vector3(0f, 0f, -2f), 1f).setEaseInOutQuad();
            LeanTween.rotateZ(seq.group.gameObject, Dead.PettyRandom.Range(160f, 180f), 1f).setOnUpdateVector3(delegate
            {
                foreach (Transform point3 in seq.points)
                {
                    point3.transform.eulerAngles = Vector3.zero;
                }
            }).setEaseInOutQuad();
            yield return new WaitForSeconds(1f);

            //seq.Flash();
            Events.InvokeScreenShake(1f, 0f);
            if ((bool)seq.ps)
            {
                seq.ps.Play();
            }

            seq.combinedFx.SetActive(value: true);

            finalEntity.transform.position = Vector3.zero;
            array = entities;
            for (int i = 0; i < array.Length; i++)
            {
                CardManager.ReturnToPool(array[i]);
            }

            seq.group.transform.localRotation = Quaternion.identity;
            finalEntity.curveAnimator.Ping();
            finalEntity.wobbler.WobbleRandom();

            yield return new WaitForSeconds(1f);
            //CinemaBarSystem.Top.SetScript(seq.titleKey.GetLocalizedString());
            //CinemaBarSystem.Bottom.SetPrompt(seq.continueKey.GetLocalizedString(), "Select");
            //while (!InputSystem.IsButtonPressed("Select"))
            //{
            //    yield return null;
            //}

            //cinemaBarState.Restore();
            //CinemaBarSystem.SetSortingLayer("CinemaBars");
            seq.fader.gameObject.Destroy();
            PauseMenu.Unblock();

            yield return Sequences.CardMove(finalEntity, new CardContainer[1] { References.Player.handContainer });
            References.Player.handContainer.TweenChildPositions();
            finalEntity.inPlay = true;
            
            ActionQueue.Stack(new ActionReveal(finalEntity));

            Debug.Log("[Pokefrost] Fused Lumin Vase");

        }

    }
}

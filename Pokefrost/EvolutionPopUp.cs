using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pokefrost
{
    internal static class EvolutionPopUp
    {
        public static List<CardData> evolvedPokemonLastBattle = new List<CardData>(3);

        public static List<CardData> pokemonEvolvedIntoLastBattle = new List<CardData>(3);

        public static List<GameObject> silhouettes = new List<GameObject>();

        private static WildfrostMod mod => Pokefrost.instance;
        private static CardFramesUnlockedSequence sequence;
        private static TextMeshProUGUI titleObject;
        private static Button continueButton;
        private static GameObject fader;

        private static int eventProgress = 0;
        private static bool startClosingSequence = false;

        private static LeanTweenType silTweenType = LeanTweenType.linear;
        private static float duration = 0.5f;
        private static float overlap = 0.2f;

        private static Vector3 translate = new Vector3(0f, -3f, 0f);
        private static Vector3 translate2 = new Vector3(0f, -0.5f, 0f);
        private static float fadeInDur = 0.75f;
        private static LeanTweenType fadeInType = LeanTweenType.easeInCubic;
        private static float hold = 0.5f;
        private static float fadeOutDur = 0.3f;
        private static LeanTweenType fadeOutType = LeanTweenType.easeOutCubic;
        private static float frequency = 0.05f;
        private static float endDelay = 0.3f;

        private static void AddCommands()
        {
            Console.commands.Add(new CommandFillEvolves());
            Console.commands.Add(new CommandFillEvolveDebug1());
        }

        public static IEnumerator Run()
        {
            eventProgress = 0;
            yield return SceneManager.WaitUntilUnloaded("CardFramesUnlocked");
            yield return SceneManager.Load("CardFramesUnlocked", SceneType.Temporary);
            sequence = GameObject.FindObjectOfType<CardFramesUnlockedSequence>();
            sequence.container1.transform.Translate(translate2);
            titleObject = sequence.GetComponentInChildren<TextMeshProUGUI>(true);
            GameObject obj = UICollector.PullPrefab("Button", "Continue Button", sequence.gameObject);
            obj.transform.position = translate;
            continueButton = obj.GetComponentInChildren<Button>();
            continueButton.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            continueButton.onClick.AddListener(ProgressEvent);
            fader = UICollector.PullPrefab("Box", "Fader", sequence.gameObject);
            fader.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            fader.SetActive(false);

            yield return Preevos();
            yield return new WaitUntil(() => eventProgress > 0);
            fader.SetActive(true);
            //SilhouetteFade(0f, 1f, duration, silTweenType);
            yield return new WaitForSeconds(duration - overlap);
            BackgroundFade(0f, 1f, fadeInDur, fadeInType);
            yield return new WaitForSeconds(fadeInDur);
            yield return Evos();
            SfxSystem.OneShot("event:/sfx/inventory/charm_assign");
            yield return new WaitForSeconds(hold);
            BackgroundFade(1f, 0f, fadeInDur, fadeOutType);
            yield return new WaitForSeconds(fadeOutDur - overlap/2);
            //SilhouetteFade(1f, 0f, duration, silTweenType);
            fader.SetActive(false);
            yield return new WaitForSeconds(duration);
            Pokefrost.SFX.TryPlaySound("evolution");
            continueButton.interactable = true;
            yield return new WaitUntil(() => eventProgress > 1);
            Routine.Clump clump = new Routine.Clump();
            for(int i = sequence.container1.Count - 1; i >= 0; i--)
            {
                clump.Add(new Routine(AssetLoader.Lookup<CardAnimation>("CardAnimations", "FlyToBackpack").Routine(sequence.container1[i])));
                yield return new WaitForSeconds(frequency);
            }
            yield return clump.WaitForEnd();
            yield return new WaitForSeconds(endDelay);
            End();
        }

        public static void End()
        {
            //RemoveSilhouettes();
            evolvedPokemonLastBattle.Clear();
            pokemonEvolvedIntoLastBattle.Clear();
            sequence.End();
        }

        public static void MoveCardToDeck(Entity entity)
        {
            Events.InvokeEntityEnterBackpack(entity);
            entity.transform.parent = References.Player.entity.display.transform;
            entity.display?.hover?.Disable();
            new Routine(AssetLoader.Lookup<CardAnimation>("CardAnimations", "FlyToBackpack").Routine(entity));
        }

        public static IEnumerator Preevos()
        {
            if (evolvedPokemonLastBattle.Count == 1)
            {
                string preEvo = evolvedPokemonLastBattle[0].title;
                string evo = pokemonEvolvedIntoLastBattle[0].title;
                titleObject.text = "<size=0.55>Huh? <#ff0>" + preEvo + "</color> is evolving? </size>";
            }
            else
            {
                titleObject.text = "<size=0.55>Huh? <#ff0>" + evolvedPokemonLastBattle.Count + "</color> Pokemon are evolving?";
            }
            yield return CreateCardsAlt(evolvedPokemonLastBattle);
            continueButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Observe");
            //AttachSilhouettes(0f);
        }

        public static IEnumerator Evos()
        {
            if (evolvedPokemonLastBattle.Count == 1)
            {
                string preEvo = evolvedPokemonLastBattle[0].title;
                string evo = pokemonEvolvedIntoLastBattle[0].title;
                titleObject.text = "<size=0.55><#ff0>" + preEvo + "</color> has evolved into" + evo + "</color>!</size>";
            }
            else
            {
                titleObject.text = "<size=0.55><#ff0>" + evolvedPokemonLastBattle.Count + "</color> Pokemon have evolved!";
            }
            //RemoveSilhouettes();
            sequence.container1.ClearAndDestroyAllImmediately();
            yield return CreateCardsAlt(pokemonEvolvedIntoLastBattle, true);
            //AttachSilhouettes(1f);
            continueButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Continue");
            Button back = sequence.group.GetComponentInChildren<Button>();
            back.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            back.onClick.AddListener(() => End());
        }

        private static Vector3 gap = new Vector3(0.4f, 0f, 0f);
        public static IEnumerator CreateCardsAlt(List<CardData> cards, bool flipped = false)
        {
            sequence.SetScaleAndPosition(cards.Count);
            Routine.Clump clump = new Routine.Clump();
            int num = Math.Min(cards.Count, 4);
            sequence.container1.gap = gap;
            sequence.container1.SetSize(num, 0.8f);
            foreach (CardData card in cards)
            {
                clump.Add(sequence.CreateCard(card, sequence.container1, startFlipped: flipped));
            }
            yield return clump.WaitForEnd();
            sequence.group.SetActive(value: true);
            sequence.container1.SetChildPositions();
        }

        public static void ProgressEvent()
        {
            eventProgress++;
            continueButton.interactable = false;
        }

        public static void BackgroundFade(float from, float to, float dur, LeanTweenType type)
        {
            LeanTween.cancel(fader);
            LeanTween.value(fader, from, to, dur).setEase(type).setOnUpdate(delegate (float a)
            {
                fader.GetComponent<Image>().color = Color.white.WithAlpha(a);
            });
        }

        public static void AttachSilhouettes(float alpha)
        {
            foreach(Entity entity in sequence.container1)
            {
                GameObject obj;
                foreach(Transform transform in entity.GetComponentsInChildren<Transform>())
                {
                    if (transform.name == "ImageContainer")
                    {
                        obj = transform.GetChild(0).gameObject;
                        GameObject box = UICollector.PullPrefab("Box", "Silhouette", obj);
                        box.GetComponent<Image>().color = new Color(1f,1f,1f,alpha);
                        obj.AddComponent<Mask>();
                        silhouettes.Add(box);
                        break;
                    }
                }
            }
        }

        public static void SilhouetteFade(float from, float to, float dur, LeanTweenType type)
        {
            foreach(GameObject obj in silhouettes)
            {
                LeanTween.cancel(obj);
                LeanTween.value(obj, from, to, dur).setEase(type).setOnUpdate(delegate (float a)
                {
                    obj.GetComponent<Image>().color = Color.white.WithAlpha(a);
                });
            }
        }

        public static void RemoveSilhouettes()
        {
            for(int i = silhouettes.Count - 1; i >= 0; i--)
            {
                silhouettes[i].transform.parent.GetComponent<Mask>().Destroy();
                silhouettes[i].Destroy();
            }
            silhouettes.Clear();
        }
    }

    public class CommandFillEvolves : Console.Command
    {
        public override string id => "evofill";

        public override string format => "evofill";

        public override void Run(string args)
        {
            for(int i=0; i<3; i++)
            {
                EvolutionPopUp.evolvedPokemonLastBattle.Add(Pokefrost.instance.Get<CardData>("eevee").Clone());
                EvolutionPopUp.pokemonEvolvedIntoLastBattle.Add(Pokefrost.instance.Get<CardData>(StatusEffectEvolveEevee.eeveelutions.RandomItem()).Clone());
            }

            EvolutionPopUp.evolvedPokemonLastBattle.Add(Pokefrost.instance.Get<CardData>("BerryPet").Clone());
            EvolutionPopUp.pokemonEvolvedIntoLastBattle.Add(Pokefrost.instance.Get<CardData>(StatusEffectEvolveEevee.eeveelutions.RandomItem()).Clone());
        }
    }

    public class CommandFillEvolveDebug1 : Console.Command
    {
        public override string id => "evo1";

        public override string format => "evo1";

        public override void Run(string args)
        {
            References.instance.StartCoroutine(EvolutionPopUp.Run());
        }
    }
}

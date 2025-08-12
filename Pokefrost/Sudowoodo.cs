using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pokefrost
{
    internal class Sudowoodo
    {
        public static float sudoChance = 0.185f;
        static string[] detail = { "0", "1", "2" };

        public static void OnSceneChanged(Scene scene)
        {
            if (scene.name != "MapNew" 
                || Dead.PettyRandom.Range(0f,1f) >= sudoChance 
                || References.PlayerData?.inventory?.deck?.FirstOrDefault(c => c.name == "websiteofsites.wildfrost.pokefrost.sudowoodo") != null 
                || References.PlayerData?.inventory?.reserve?.FirstOrDefault(c => c.name == "websiteofsites.wildfrost.pokefrost.sudowoodo") != null)
            { return; }

            ActionQueue.Add(new ActionSequence(SudoSpawn()));
        }

        public static IEnumerator SudoSpawn()
        {
            yield return new WaitUntil(() => References.Map != null || Battle.instance != null);
            if (Battle.instance != null) { yield break; }

            Debug.Log($"[Pokefrost] Counting nodes...");
            List<MapNode> nodes = References.Map.nodes;
            Debug.Log($"[Pokefrost] Sudo Spawned! {nodes.Count}");
            MapNode node = nodes.Where(n => n!=null 
            && n.gameObject.activeSelf 
            && detail.Contains(n.campaignNode?.type?.letter ?? "Sudowoodo"))
                .InRandomOrder().FirstOrDefault();
            if (node == null) { yield break; }

            node.gameObject.SetActive(false);
            SudowoodoButton(node.transform);
            yield break;
        }

        public static Transform SudowoodoButton(Transform t)
        {
            GameObject sudo = new GameObject("Sudo");
            sudo.transform.localScale = new Vector3(0.4f, 0.4f, 1);
            sudo.transform.SetParent(t.parent);
            sudo.transform.position = t.position;
            sudo.AddComponent<CircleCollider2D>().radius = 1.5f;
            sudo.AddComponent<SpriteRenderer>().sprite = Pokefrost.instance.ImagePath("sudoMap.png").ToSprite();
            EventTrigger button = sudo.AddComponent<EventTrigger>();
            AddListener(button, EventTriggerType.PointerClick, _ => Recruit(sudo.transform));
            AddListener(button, EventTriggerType.PointerEnter, _ => ChangeScale(sudo.transform, 0.45f));
            AddListener(button, EventTriggerType.PointerExit, _ => ChangeScale(sudo.transform, 0.4f));
            return sudo.transform;
        }

        public static void AddListener(EventTrigger et, EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry()
            {
                eventID = type,
            };
            entry.callback.AddListener(callback);
            et.triggers.Add(entry);
        }

        public static void Recruit(Transform t)
        {
            t.gameObject.SetActive(false);
            Debug.Log("[Pokefrost] You found Sudowoodo! Good job!");
            ActionQueue.Stack(new ActionSequence(Sequence()));
        }

        public static void ChangeScale(Transform t, float scale)
        {
            t.localScale = new Vector3(scale, scale, 1);
        }

        public static IEnumerator Sequence()
        {
            SecretNakedGnomeSystem system = GameObject.FindObjectOfType<SecretNakedGnomeSystem>(true);
            InspectNewUnitSequence sequence = UnityEngine.Object.Instantiate(system.gainNakedGnomeSequencePrefab, References.Player.entity.display.transform);
            sequence.cardSelector.character = References.Player;
            sequence.GetComponent<CardSelector>()?.selectEvent.AddListener(Events.InvokeEntityChosen);
            CardData data = Pokefrost.instance.Get<CardData>("sudowoodo").Clone();
            Card card = CardManager.Get(data, null, References.Player, inPlay: false, isPlayerCard: true);
            card.transform.SetParent(sequence.cardHolder);
            card.transform.localPosition = SecretNakedGnomeSystem.startPos;
            yield return card.UpdateData();
            sequence.SetUnit(card.entity, updateGreeting: true);
            sequence.gameObject.GetComponentInChildren<SpeechBubble>().textElement.text = "Sudowoodo is not fooling anyone...";
            Events.InvokeEntityOffered(card.entity);
            yield return sequence.Run();
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Extensions = Deadpan.Enums.Engine.Components.Modding.Extensions;

namespace Pokefrost
{
    internal static class PickupRoutine
    {
        static GameObject objectGroup;
        static GameObject obj;
        static ChooseNewCardSequence sequence;
        static Transform deadCards;

        public static void Debug1()
        {
            Create("<color=#8F0>Pizza Frog</color> Found A Card");
        }

        public static void Debug2(int amount = 4)
        {
            RewardPool r = Extensions.GetRewardPool("GeneralItemPool");
            References.instance.StartCoroutine(AddRandomCards(amount, new RewardPool[] { r }, (c) => c.cardType.name != "Friendly"));
        }

        public static async Task Debug3()
        {
            References.instance.StartCoroutine(Run());
        }

        public static void Debug4()
        {
            Events.PreBattleEnd += Debug3;
        }

        public static void Create(string text)
        {
            if (objectGroup != null)
            {
                objectGroup.GetComponentInChildren<FloatingText>().SetText(text);
                return;
            }

            objectGroup = new GameObject("SelectCardRoutine");
            objectGroup.SetActive(false);
            objectGroup.transform.SetParent(GameObject.Find("CameraContainer/CameraMover/MinibossZoomer/CameraPositioner/CameraPointer/Animator/Rumbler/Shaker/InspectSystem").transform);
            objectGroup.transform.SetAsFirstSibling();

            GameObject background = UICollector.PullPrefab("Box", "Background", objectGroup);
            background.GetComponent<RectTransform>().sizeDelta = 25 * Vector2.one;
            background.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);

            GameObject title = UICollector.PullPrefab("Text", "Title", objectGroup);
            title.GetComponent<FloatingText>().SetText(text);
            title.transform.position = new Vector3(0, 4.5f, 0);

            obj = new GameObject("SelectCard");
            obj.transform.SetParent(objectGroup.transform);
            obj.transform.Translate(new Vector3(0, 1f, 0));
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(7, 2);
            CardControllerSelectCard cc =  obj.AddComponent<CardControllerSelectCard>();
            cc.owner = References.Player;
            cc.unHoverEvent = new UnityEventEntity();
            cc.hoverEvent = new UnityEventEntity();
            cc.pressEvent = new UnityEventEntity();
            cc.pressEvent.AddListener(Select);
            CardHand container = obj.AddComponent<CardHand>();
            container.fanCircleAngleAddCurve = new AnimationCurve();
            container.staticAngleAdd = true;
            container.holder = obj.GetComponent<RectTransform>();
            container.onAdd = new UnityEventEntity();
            container.onRemove = new UnityEventEntity();
            container.AssignController(cc);
            container.SetSize(5, 0.889f);
            CardSelector cs = obj.AddComponent<CardSelector>();
            cs.character = References.Player;
            cs.selectEvent = new UnityEventEntity();

            GameObject deadObject = new GameObject("DeadObject");
            deadCards = deadObject.transform;
            deadObject.SetActive(false);

            sequence = objectGroup.AddComponent<ChooseNewCardSequence>();
            sequence.cardContainer = container;
            sequence.cardSelector = cs;
            sequence.cardController = cc;
            sequence.background = background.GetComponent<RectTransform>();
            sequence.cardGroupLayout = obj;
        }

        public static IEnumerator AddRandomCards(int amount, RewardPool[] rewards, Func<CardData, bool> criteria)
        {
            CardHand hand = obj.GetComponent<CardHand>();
            Debug.Log("[Pokefrost] Cleared!");
            hand.Clear();
            IEnumerable<DataFile> data = new List<DataFile>();
            foreach (RewardPool r in rewards)
            {
                data = data.Concat(r.list);
            }
            IEnumerable<CardData> cards = data.Where((d) => d is CardData).Select((d) => (CardData)d);

            Routine.Clump clump = new Routine.Clump();
            foreach (CardData card in cards.InRandomOrder())
            {
                if (criteria(card))
                {
                    Card item = CardManager.Get(card.Clone(), obj.GetComponent<CardController>(), References.Player, false, true);
                    clump.Add(item.UpdateData());
                    obj.GetComponent<CardContainer>().Add(item.entity);
                    amount--;
                    if (amount <=0 )
                    {
                        break;
                    }
                }
            }
            yield return clump.WaitForEnd();
            obj.GetComponent<CardContainer>().TweenChildPositions();
        }

        public static IEnumerator Run()
        {
            obj.SetActive(true);
            sequence.cardController.Enable();
            sequence.promptEnd = obj.GetComponent<CardHand>().entities.Count == 0;
            yield return objectGroup.GetComponent<ChooseNewCardSequence>().Run();
        }

        public static void Select(Entity entity)
        {
            References.PlayerData.inventory.deck.Add(entity.data);
            sequence.cardController.Disable();
            sequence.promptEnd = true;
            Events.InvokeEntityChosen(entity);
        }
    }
}

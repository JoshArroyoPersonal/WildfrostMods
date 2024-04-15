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
    internal class EventRoutineTrade : EventRoutine
    {
        public CardControllerSelectCard cc;
        public CardSelector cs;
        public CardLane topRow;
        public CardLane bottomRow;
        public GameObject backButton;

        public GameObject TradeLines;
        public List<Image> TradeLinesList = new List<Image>();
        public Vector2 linesScale = new Vector2(2.5f, 8.25f);
        public Color offColor = new Color(0f, 0f, 0f, 0f);
        public Color onColor = new Color(0.3f, 0.8f, 0.5f, 0.7f);

        public GameObject selectionBackground;
        public GameObject cancelObject;
        public GameObject confirmObject;

        public TextMeshProUGUI title;
        public int swapped = -1;
        //public List<int> swapped = new List<int>();

        public bool selected = false;
        public float selectTime = 0.5f;
        public LeanTweenType selectType = LeanTweenType.easeOutQuart;
        Vector3 left = new Vector3(-2.5f, 1f, 0f);
        Vector3 right = new Vector3(2.5f, 1f, 0f);

        public float tradeTime = 1f;

        public bool routineActive = true;
        //public int maxTrades = 1;

        public override IEnumerator Populate()
        {
            string[] saveCollection = base.data.GetSaveCollection<string>("cards");
            string[] upgradeCollection = base.data.GetSaveCollection<string>("charms");
            int size = saveCollection.Count();
            if (!data.ContainsKey("currentcompanions"))
            {
                List<CardData> items = new List<CardData>();
                foreach(CardData cardData in References.PlayerData.inventory.deck)
                {
                    if (cardData.cardType.name == "Friendly")
                    {
                        items.Add(cardData);
                    }
                }
                foreach (CardData cardData in References.PlayerData.inventory.reserve)
                {
                    if (cardData.cardType.name == "Friendly")
                    {
                        items.Add(cardData);
                    }
                }
                size = Math.Min(size, items.Count());
                items = items.InRandomOrder().ToList().GetRange(0, size);
                node.data.Add("currentcompanions", items.ToSaveCollectionOfNames());
            }
            string[] savedOffers = base.data.GetSaveCollection<string>("currentcompanions");
            size = Math.Min(saveCollection.Count(), savedOffers.Count());

            topRow.SetSize(saveCollection.Length, 0.7f);
            bottomRow.SetSize(saveCollection.Length, 0.7f);
            Routine.Clump clump = new Routine.Clump();
            for (int i = 0; i < size; i++)
            {
                clump.Add(CreateCardsFromLoader(saveCollection[i], new List<string> { upgradeCollection[2 * i], upgradeCollection[2 * i + 1] }, cc, topRow)); //Edit Save Collection to remember charms
                clump.Add(CreateCardsFromDeck(savedOffers[i], cc, bottomRow));
            }

            yield return clump.WaitForEnd();
            topRow.SetChildPositions();
            bottomRow.SetChildPositions();
            SetSize();

            cc.pressEvent.AddListener(Select);
            cc.hoverEvent.AddListener(Hover);
            cc.unHoverEvent.AddListener(Unhover);
            cs.character = References.Player;
            backButton.GetComponentInChildren<Button>().onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            backButton.GetComponentInChildren<Button>().onClick.AddListener(Leave);
            cancelObject.GetComponentInChildren<Button>().onClick.AddListener(Cancel);
            cancelObject.GetComponentInChildren<Button>().onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            confirmObject.GetComponentInChildren<Button>().onClick.AddListener(Confirm);
            confirmObject.GetComponentInChildren<Button>().onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            cancelObject.GetComponentInChildren<ButtonAnimator>().baseColour = new Color(0.9f, 0.3f, 0.3f, 1f);
        }

        public void PrepareButtons()
        {
            cancelObject.GetComponentInChildren<ButtonAnimator>().baseColour = new Color(0.9f, 0.3f, 0.3f, 1f);
            cancelObject.SetActive(false);
            cancelObject.SetActive(true);
            cancelObject.GetComponentInChildren<TextMeshProUGUI>().SetText("Cancel");
            confirmObject.GetComponentInChildren<TextMeshProUGUI>().SetText("Confirm");
        }

        private void SetSize()
        {
            for(int i=0; i<bottomRow.entities.Count; i++)
            {
                GameObject box = UICollector.PullPrefab("Box", "Box", TradeLines);
                box.transform.localPosition = new Vector3(bottomRow.GetChildPosition(bottomRow.entities[i]).x, 0, 0);
                box.GetComponent<RectTransform>().sizeDelta = linesScale;
                TradeLinesList.Add(box.GetComponent<Image>());
                TradeLinesList[i].color = offColor;
            }
        }

        public override IEnumerator Run()
        {
            while(routineActive)
            {
                if (swapped != -1)
                {
                    title.SetText("Select a trade!");
                    topRow.entities[swapped].transform.SetParent(topRow.transform, true);
                    bottomRow.entities[swapped].transform.SetParent(bottomRow.transform, true);
                    topRow.TweenChildPositions();
                    bottomRow.TweenChildPositions();
                    swapped = -1;
                }
                cc.enabled = true;
                selectionBackground.SetActive(false);
                yield return new WaitUntil(() => selected);

                //Leave
                if (swapped == -1)
                {
                    yield break;
                }

                //Trade Selected
                title.SetText($"{bottomRow.entities[swapped].data.title} for {topRow.entities[swapped].data.title}?");
                selectionBackground.SetActive(true);
                PrepareButtons();
                UnhoverAll();
                topRow.entities[swapped].transform.SetParent(selectionBackground.transform, true);
                bottomRow.entities[swapped].transform.SetParent(selectionBackground.transform, true);
                cc.enabled = false;
                LeanTween.moveLocal(topRow.entities[swapped].gameObject, right, selectTime).setEase(selectType);
                LeanTween.moveLocal(bottomRow.entities[swapped].gameObject, left, selectTime).setEase(selectType);
                LeanTween.scale(topRow.entities[swapped].gameObject, Vector3.one, selectTime).setEase(selectType);
                LeanTween.scale(bottomRow.entities[swapped].gameObject, Vector3.one, selectTime).setEase(selectType);
                yield return Sequences.Wait(selectTime + 0.1f);
                yield return new WaitUntil(() => !selected);
            }
            //Trade Confirmed
            LeanTween.moveLocal(topRow.entities[swapped].gameObject, left, tradeTime).setEase(selectType);
            LeanTween.moveLocal(bottomRow.entities[swapped].gameObject, right, tradeTime).setEase(selectType);
            yield return Sequences.Wait(tradeTime + 0.1f);
            cs.TakeCard(topRow.entities[swapped]);
            yield return Sequences.Wait(0.2f);
        }

        private static IEnumerator CreateCardsFromDeck(string cardName, CardController cardController, CardContainer cardContainer, bool startFlipped = true)
        {
            foreach(CardData cardData in References.PlayerData.inventory.deck)
            {
                if (cardData.name == cardName)
                {
                    return CreateCards(cardData, cardController, cardContainer, startFlipped);
                }    
            }
            foreach (CardData cardData in References.PlayerData.inventory.reserve)
            {
                if (cardData.name == cardName)
                {
                    return CreateCards(cardData, cardController, cardContainer, startFlipped);
                }
            }
            return null;
        }

        private static IEnumerator CreateCardsFromLoader(string cardName, List<string> upgrades, CardController cardController, CardContainer cardContainer, bool startFlipped = true)
        {
            CardData cardData = AddressableLoader.Get<CardData>("CardData", cardName).Clone();

            foreach(string upgrade in upgrades)
            {
                CardUpgradeData upgradeData = AddressableLoader.Get<CardUpgradeData>("CardUpgradeData", upgrade);
                if (upgradeData.CanAssign(cardData))
                {
                    upgradeData.Clone().Assign(cardData);
                }
            }

            return CreateCards(cardData,cardController, cardContainer, startFlipped);
        }

        private static IEnumerator CreateCards(CardData cardData, CardController cardController, CardContainer cardContainer, bool startFlipped = true)
        {
            Card card = CardManager.Get(cardData, cardController, null, inPlay: false, isPlayerCard: true);
            if (startFlipped)
            {
                card.entity.flipper.FlipDownInstant();
            }

            Debug.Log("Trading!");
            Debug.Log(cardContainer != null);
            cardContainer.Add(card.entity);
            yield return card.UpdateData();
            if (startFlipped)
            {
                card.entity.flipper.FlipUp(force: true);
            }
        }

        public void Hover(Entity entity)
        {
            int index = FindIndex(entity);
            if (index != -1)
            {
                TradeLinesList[index].color = onColor;
            }
        }

        public void Unhover(Entity entity)
        {
            int index = FindIndex(entity);
            if (index != -1)
            {
                TradeLinesList[index].color = offColor;
            }
        }

        public void UnhoverAll()
        {
            foreach(Image image in TradeLinesList)
            {
                image.color = offColor;
            }
        }

        public int FindIndex(Entity entity)
        {
            int i;
            i = topRow.entities.IndexOf(entity);
            if (i == -1)
            {
                i = bottomRow.entities.IndexOf(entity);
            }
            return i;
        }

        public void Select(Entity entity)
        {
            cc.UnHover(entity);
            swapped = FindIndex(entity);
            if(swapped != -1)
            {
                selected = true;
            }
        }

        public void Swap(Entity entity)
        {
            int i;
            i = topRow.entities.IndexOf(entity);
            if (i == -1)
            {
                i = bottomRow.entities.IndexOf(entity);
            }
            if (i != -1)
            {
                Swap(i);
            }
        }

        public void Swap(int index)
        {
            if (index >= Math.Min(topRow.entities.Count, bottomRow.entities.Count))
            {
                Debug.Log("[Pokeforst] How did that happen?");
                return;
            }

            Entity topEntity = topRow.entities[index];
            Entity bottomEntity = bottomRow.entities[index];

            topRow.RemoveAt(index);
            bottomRow.RemoveAt(index);
            topRow.Insert(index, bottomEntity);
            bottomRow.Insert(index, topEntity);

            topRow.TweenChildPositions();
            bottomRow.TweenChildPositions();

            if(swapped == index)
            //if (swapped.Contains(index))
            {
                swapped = -1;
                //swapped.Remove(index);
                TradeLinesList[index].color = offColor;
            }
            else
            {
                swapped = index;
                //swapped.Add(index);
                TradeLinesList[index].color = onColor;
            }

            selected = true;
            /*if(swapped.Count > maxTrades || swapped.Count == 0)
            {
                confirmObject.GetComponentInChildren<ButtonAnimator>().interactable = false;
            }
            else
            {
                confirmObject.GetComponentInChildren<ButtonAnimator>().interactable = true;
            }*/
        }

        public void Leave()
        {
            selected = true; //Not Really
        }

        public void Cancel()
        {
            selected = false;
            Debug.Log("[Pokefrost] Cancel");
        }

        public void Confirm()
        {
            int index = swapped;
            {
                foreach(CardUpgradeData cardUpgradeData in bottomRow.entities[index].data.upgrades)
                {
                    if (cardUpgradeData.type != CardUpgradeData.Type.Charm && cardUpgradeData.canBeRemoved)
                    {
                        References.PlayerData.inventory.upgrades.Add(AddressableLoader.Get<CardUpgradeData>("CardUpgradeData", cardUpgradeData.name).Clone());
                    }
                }
                bool breakFlag = false;
                foreach(CardData cardData in References.PlayerData.inventory.deck)
                {
                    if (cardData.name == bottomRow.entities[index].data.name)
                    {
                        References.PlayerData.inventory.deck.Remove(cardData);
                        //References.PlayerData.inventory.deck.Add(topRow.entities[index].data);
                        breakFlag = true;
                        break;
                    }
                }
                if (!breakFlag)
                {
                    foreach (CardData cardData in References.PlayerData.inventory.reserve)
                    {
                        if (cardData.name == bottomRow.entities[index].data.name)
                        {
                            References.PlayerData.inventory.reserve.Remove(cardData);
                            //References.PlayerData.inventory.reserve.Add(topRow.entities[index].data);
                            break;
                        }
                    }
                }
            }
            node.SetCleared();
            routineActive = false;
            selected = false;
            Debug.Log("[Pokefrost] Confirm");
        }
    }

    
}

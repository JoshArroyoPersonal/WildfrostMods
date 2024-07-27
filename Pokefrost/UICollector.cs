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
    public static class UICollector
    {
        public static Dictionary<String, GameObject> Prefabs = new Dictionary<String, GameObject>();
        public static GameObject gameObject = null;

        public static IEnumerator CollectPrefabs()
        {
            if (gameObject != null)
            {
                yield break;
            }

            gameObject = new GameObject("UICollector");
            GameObject.DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);

            GameObject box = new GameObject("Box");
            box.AddComponent<Image>();
            Prefabs.Add(box.name, box);
            box.transform.SetParent(gameObject.transform, false);

            yield return new WaitUntil(() => SceneManager.Loaded.ContainsKey("Global"));
            GameObject floatingText = GameObject.FindObjectOfType<FloatingTextManager>().CreatePrefab().gameObject;
            floatingText.name = "Text";
            Prefabs.Add(floatingText.name, floatingText);
            floatingText.transform.SetParent(gameObject.transform, false);

            yield return new WaitUntil(() => SceneManager.Loaded.ContainsKey("MainMenu"));
            GameObject button = GameObject.Find("ModsButton").InstantiateKeepName();
            button.name = "Button";
            Prefabs.Add(button.name, button);
            button.transform.SetParent(gameObject.transform, false);

            yield return new WaitUntil(() => SceneManager.Loaded.ContainsKey("Town"));
            GameObject backButton = GameObject.Find("Canvas/SafeArea/Back Button").InstantiateKeepName();
            backButton.name = "BackButton";
            Prefabs.Add(backButton.name, backButton);
            backButton.transform.SetParent(gameObject.transform, false);
            PokemonTradeEvent();

        }

        public static GameObject FindEx(string s)
        {
            return GameObject.Find(s);
        }

        public static GameObject PullPrefab(string key, string name, GameObject parent)
        {
            if (!Prefabs.ContainsKey(key))
            {
                Debug.Log($"[UICollector] Could not find a prefab with key {key}");
            }
            GameObject g = GameObject.Instantiate(Prefabs[key], parent.transform);
            g.name = name;
            return g;
        }

        public static void PokemonTradeEvent()
        {
            GameObject controller = new GameObject("TradeEventManager");
            controller.SetActive(false);
            CardControllerSelectCard cc = controller.AddComponent<CardControllerSelectCard>();
            cc.pressEvent = new UnityEventEntity();
            cc.hoverEvent = new UnityEventEntity();
            cc.unHoverEvent = new UnityEventEntity();
            CardSelector cs = controller.AddComponent<CardSelector>();
            cs.selectEvent = new UnityEventEntity();

            //Background
            GameObject background = new GameObject("Background");
            background.SetActive(false);
            background.transform.SetParent(controller.transform, false);
            background.AddComponent<Image>().sprite = Pokefrost.instance.ImagePath("trade_background.png").ToSprite();
            //background.transform.localPosition = new Vector3(-2.1f, -20.44f, 0);
            //background.transform.localScale = new Vector3(5.2f, 1.2f, 0);
            background.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
            background.transform.localScale = new Vector3(0.3f, 0.3f, 0);

            //TradeLines
            GameObject lineHolder = new GameObject("LineHolder");
            lineHolder.transform.SetParent(controller.transform);
            lineHolder.transform.Translate(new Vector3(0f, -0.1f, 0f));

            //Top Row
            GameObject lane1 = new GameObject("CardLane1");
            lane1.SetActive(false);
            lane1.AddComponent<Image>();
            lane1.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            CardLane cardLane1 = lane1.AddComponent<CardLane>();
            cardLane1.holder = lane1.GetComponent<RectTransform>();
            cardLane1.onAdd = new UnityEventEntity();
            lane1.transform.SetParent(controller.transform);
            lane1.transform.Translate(new Vector3(0f, 2.3f, 0f));
            cardLane1.gap = new Vector3(0.7f, 0f, 0f);

            //Bottom Row
            GameObject lane2 = new GameObject("CardLane2");
            lane2.SetActive(false);
            lane2.AddComponent<Image>();
            lane2.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            CardLane cardLane2 = lane2.AddComponent<CardLane>();
            cardLane2.holder = lane2.GetComponent<RectTransform>();
            cardLane2.onAdd = new UnityEventEntity();
            lane2.transform.SetParent(controller.transform);
            lane2.transform.Translate(new Vector3(0f, -2.3f, 0f));
            cardLane2.gap = new Vector3(0.7f, 0f, 0f);

            //Back Button
            GameObject backButton = UICollector.PullPrefab("BackButton", "BackButton", controller);
            backButton.transform.localPosition = new Vector3(-8f, 0f, 0f);

            //Selected Background
            GameObject background2 = new GameObject("SelectBackground");
            background2.SetActive(false);
            background2.transform.SetParent(controller.transform, false);
            background2.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);
            //background.transform.localPosition = new Vector3(-2.1f, -20.44f, 0);
            //background.transform.localScale = new Vector3(5.2f, 1.2f, 0);
            background2.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
            background2.AddComponent<UINavigationLayer>();

            //Cancel Button
            GameObject cancelButton = UICollector.PullPrefab("Button", "CancelButton", background2);
            cancelButton.GetComponent<RectTransform>().sizeDelta = new Vector2(4f, 1);
            cancelButton.transform.localPosition = new Vector3(0f, -3.7f, 0f);

            //Confirm Button
            GameObject confirmButton = UICollector.PullPrefab("Button", "ConfirmButton", background2);
            confirmButton.GetComponent<RectTransform>().sizeDelta = new Vector2(4f, 1);
            confirmButton.transform.localPosition = new Vector3(0f, -2.5f, 0f);

            //Title
            GameObject title = UICollector.PullPrefab("Text", "Title", controller);
            title.GetComponentInChildren<TextMeshProUGUI>().text = "Trade Offers";
            title.transform.Translate(new Vector3(0f, 4.8f, 0f));

            EventRoutineTrade trade = controller.AddComponent<EventRoutineTrade>();
            trade.cc = cc;
            trade.cs = cs;
            trade.title = title.GetComponentInChildren<TextMeshProUGUI>();
            trade.TradeLines = lineHolder;
            trade.topRow = cardLane1;
            trade.bottomRow = cardLane2;
            trade.backButton = backButton;
            trade.selectionBackground = background2;
            trade.cancelObject = cancelButton;
            trade.confirmObject = confirmButton;
            GameObject.DontDestroyOnLoad(controller);
            background.SetActive(true);
            lane1.SetActive(true);
            lane2.SetActive(true);
            CampaignNodeTypeBetterEvent.Prefabs.Add("Trade", controller);
        }
        //Canvas/Safe Area/Menu/ButtonLayout/SettingsButton
        //FloatingTextManager

        //
        //

        //6.86 0.72 0
        //2 2 0
    }
}

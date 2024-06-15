using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

namespace Pokefrost
{
    public static class Ext
    {
        public static StringTable Collection => LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
        public static StringTable KeyCollection => LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
        public static T CreateStatus<T>(string name, string desc = null, string textInsert = null, string type = "", bool boostable = false, bool stackable = true) where T : StatusEffectData
        {
            T status = ScriptableObject.CreateInstance<T>();
            status.name = name;
            status.targetConstraints = new TargetConstraint[0];
            if (!desc.IsNullOrEmpty())
            {
                Collection.SetString(name + "_text", desc);
                status.textKey = Collection.GetString(name + "_text");
                if (!textInsert.IsNullOrEmpty())
                {
                    status.textInsert = textInsert;
                }
            }
            status.type = type;
            status.canBeBoosted = boostable;
            status.stackable = stackable;
            return status;
        }

        public static T SetConstraints<T>(this T t, params TargetConstraint[] ts) where T : StatusEffectData
        {
            t.targetConstraints = ts;
            return t;
        }

        public static T ApplyX<T>(this T t, StatusEffectData effectToApply, StatusEffectApplyX.ApplyToFlags flags) where T:StatusEffectApplyX
        {
            t.effectToApply = effectToApply;
            t.applyToFlags = flags;
            return t;
        }

        public static T SetApplyConstraints<T>(this T t, params TargetConstraint[] ts) where T : StatusEffectApplyX
        {
            t.applyConstraints = ts;
            return t;
        }

        public static T Register<T>(this T status, WildfrostMod mod) where T : StatusEffectData
        {
            status.ModAdded = mod;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", status);
            return status;
        }

        public static GameObject CreateButtonIcon(this WildfrostMod mod, string name, Sprite sprite, string type, string copyTextFrom, Color textColor, KeywordData[] keys)
        {
            GameObject gameObject = new GameObject(name);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
            StatusIconExt icon = gameObject.AddComponent<StatusIconExt>();
            Dictionary<string, GameObject> cardIcons = CardManager.cardIcons;
            icon.animator = gameObject.AddComponent<ButtonAnimator>();
            icon.button = gameObject.AddComponent<ButtonExt>();
            icon.animator.button = icon.button;
            if (!copyTextFrom.IsNullOrEmpty())
            {
                GameObject text = cardIcons[copyTextFrom].GetComponentInChildren<TextMeshProUGUI>().gameObject.InstantiateKeepName();
                text.transform.SetParent(gameObject.transform);
                icon.textElement = text.GetComponent<TextMeshProUGUI>();
                icon.textColour = textColor;
                icon.textColourAboveMax = textColor;
                icon.textColourBelowMax = textColor;
            }
            icon.onCreate = new UnityEngine.Events.UnityEvent();
            icon.onDestroy = new UnityEngine.Events.UnityEvent();
            icon.onValueDown = new UnityEventStatStat();
            icon.onValueUp = new UnityEventStatStat();
            icon.afterUpdate = new UnityEngine.Events.UnityEvent();
            UnityEngine.UI.Image image = gameObject.AddComponent<UnityEngine.UI.Image>();
            image.sprite = sprite;
            CardHover cardHover = gameObject.AddComponent<CardHover>();
            cardHover.enabled = false;
            cardHover.IsMaster = false;
            CardPopUpTarget cardPopUp = gameObject.AddComponent<CardPopUpTarget>();
            cardPopUp.keywords = keys;
            cardHover.pop = cardPopUp;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta *= 0.008f;
            gameObject.SetActive(true);
            icon.type = type;
            cardIcons[type] = gameObject;

            return gameObject;
        }

        public static GameObject CreateIcon(this WildfrostMod mod, string name, Sprite sprite, string type, string copyTextFrom, Color textColor, KeywordData[] keys)
        {
            GameObject gameObject = new GameObject(name);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
            StatusIcon icon = gameObject.AddComponent<StatusIcon>();
            Dictionary<string, GameObject> cardIcons = CardManager.cardIcons;
            if (!copyTextFrom.IsNullOrEmpty())
            {
                GameObject text = cardIcons[copyTextFrom].GetComponentInChildren<TextMeshProUGUI>().gameObject.InstantiateKeepName();
                text.transform.SetParent(gameObject.transform);
                icon.textElement = text.GetComponent<TextMeshProUGUI>();
                icon.textColour = textColor;
                icon.textColourAboveMax = textColor;
                icon.textColourBelowMax = textColor;
            }
            icon.onCreate = new UnityEngine.Events.UnityEvent();
            icon.onDestroy = new UnityEngine.Events.UnityEvent();
            icon.onValueDown = new UnityEventStatStat();
            icon.onValueUp = new UnityEventStatStat();
            icon.afterUpdate = new UnityEngine.Events.UnityEvent();
            UnityEngine.UI.Image image = gameObject.AddComponent<UnityEngine.UI.Image>();
            image.sprite = sprite;
            CardHover cardHover = gameObject.AddComponent<CardHover>();
            cardHover.enabled = false;
            cardHover.IsMaster = false;
            CardPopUpTarget cardPopUp = gameObject.AddComponent<CardPopUpTarget>();
            cardPopUp.keywords = keys;
            cardHover.pop = cardPopUp;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta *= 0.012f;
            gameObject.SetActive(true);
            icon.type = type;
            cardIcons[type] = gameObject;

            return gameObject;
        }

        public static KeywordData CreateBasicKeyword(this WildfrostMod mod, string name, string title, string desc)
        {
            KeywordData data = ScriptableObject.CreateInstance<KeywordData>();
            data.name = name;
            KeyCollection.SetString(data.name + "_text", title);
            data.titleKey = KeyCollection.GetString(data.name + "_text");
            KeyCollection.SetString(data.name + "_desc", desc);
            data.descKey = KeyCollection.GetString(data.name + "_desc");
            data.ModAdded = mod;
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", data);
            return data;
        }

        public static T CreateStatusButton<T>(this WildfrostMod mod, string name, string type, string iconGroup = "counter") where T : StatusEffectData
        {
            T status = ScriptableObject.CreateInstance<T>();
            status.name = name;
            status.targetConstraints = new TargetConstraint[0];
            status.type = type;
            status.isStatus = true;
            status.iconGroupName = iconGroup;
            status.visible = true;
            status.stackable = false;
            return status;
        }

        public static CampaignNodeTypeBuilder CreateCampaignNodeType<T>(WildfrostMod mod, string name, string letter, bool canSkip = true) where T : CampaignNodeType
        {
            return new CampaignNodeTypeBuilder(mod)
                .Create<T>(name)
                .WithCanEnter(true)
                .WithCanLink(true)
                .WithInteractable(true)
                .WithCanSkip(canSkip)
                .WithLetter(letter)
                .WithZoneName(name);
        }

        public static CampaignNodeTypeBuilder BetterEvent(this CampaignNodeTypeBuilder cn, string key, WildfrostMod mod)
        {
            MapNode mapNode = mod.Get<CampaignNodeType>("CampaignNodeCharm").mapNodePrefab.InstantiateKeepName();
            mapNode.transform.SetParent(Pokefrost.pokefrostUI.transform, false);
            StringTable collection = LocalizationHelper.GetCollection("UI Text", SystemLanguage.English);
            collection.SetString("map_" + mapNode.name, key);
            mapNode.label.GetComponentInChildren<LocalizeStringEvent>().StringReference = collection.GetString("map_" + mapNode.name);
            mapNode.spriteOptions[0] = mod.ImagePath("trade_event.png").ToSprite();
            mapNode.clearedSpriteOptions[0] = mod.ImagePath("trade_done.png").ToSprite();
            return cn.WithMapNodePrefab(mapNode)
                .FreeModify<CampaignNodeTypeBetterEvent>((data) =>
                {
                    data.key = key;
                });
        }

        public static void Register(this CampaignNodeTypeBuilder cn, WildfrostMod mod)
        {
            CampaignNodeType c = cn.Build();
            c.ModAdded = mod;
            AddressableLoader.AddToGroup<CampaignNodeType>("CampaignNodeType", c);
        }

        public static CardDataBuilder SStartEffects(this CardDataBuilder b, params (string,int)[] statusEffects)
        {
            return b.SubscribeToAfterAllBuildEvent(
                (data) =>
                {
                    CardData.StatusEffectStacks[] stacks = statusEffects.Select((status) => new CardData.StatusEffectStacks(Pokefrost.instance.Get<StatusEffectData>(status.Item1), status.Item2)).ToArray();
                    data.startWithEffects = stacks;
                });
        }

        public static CardDataBuilder SAttackEffects(this CardDataBuilder b, params (string, int)[] statusEffects)
        {
            return b.SubscribeToAfterAllBuildEvent(
                (data) =>
                {
                    CardData.StatusEffectStacks[] stacks = statusEffects.Select((status) => new CardData.StatusEffectStacks(Pokefrost.instance.Get<StatusEffectData>(status.Item1), status.Item2)).ToArray();
                    data.attackEffects = stacks;
                });
        }

        public static CardDataBuilder STraits(this CardDataBuilder b, params (string, int)[] traits)
        {
            return b.SubscribeToAfterAllBuildEvent(
                (data) =>
                {
                    List<CardData.TraitStacks> t = traits.Select((tr) => new CardData.TraitStacks(Pokefrost.instance.Get<TraitData>(tr.Item1), tr.Item2)).ToList();
                    data.traits = t;
                });
        }
    }


}

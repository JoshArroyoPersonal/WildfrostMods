using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
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
using UnityEngine.U2D;
using ADD = Pokefrost.AddressableExtMethods;

namespace Pokefrost
{
    public static class Ext
    {
        public static StringTable Collection => LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
        public static StringTable KeyCollection => LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);

        public static Sprite panel;
        public static Sprite panelSmall;

        public static void CreateColoredInkAnim(WildfrostMod mod, Color color, string type)
        {
            VfxStatusSystem vfx = GameObject.FindObjectOfType<VfxStatusSystem>();
            GameObject obj = GameObject.Instantiate(vfx.profileLookup["ink"].applyEffectPrefab, Pokefrost.pokefrostUI.transform);

            ParticleSystem system;

            //Dust - base color is white, no need to change sprites
            system = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
            ParticleSystem.ColorOverLifetimeModule life = system.colorOverLifetime;
            life.color = color;
            ParticleSystem.MainModule module = system.main; //Cannot combine with the bottom line :/
            module.startColor = color;

            //Splatter (x3)
            Sprite spr = ADD.ASprite("Splatter.png");
            system = obj.transform.GetChild(4).GetComponent<ParticleSystem>();
            system.textureSheetAnimation.AddSprite(spr);
            system.textureSheetAnimation.RemoveSprite(0);
            module = system.main;
            module.startColor = color;
            system = obj.transform.GetChild(5).GetComponent<ParticleSystem>();
            system.textureSheetAnimation.AddSprite(spr);
            system.textureSheetAnimation.RemoveSprite(0);
            module = system.main;
            module.startColor = color;
            system = obj.transform.GetChild(6).GetComponent<ParticleSystem>();
            system.textureSheetAnimation.AddSprite(spr);
            system.textureSheetAnimation.RemoveSprite(0);
            module = system.main;
            module.startColor = color;

            //Splat
            spr = mod.ImagePath("Splat.png").ToSprite();
            system = obj.transform.GetChild(9).GetComponent<ParticleSystem>();
            system.textureSheetAnimation.AddSprite(spr);
            system.textureSheetAnimation.RemoveSprite(0);
            module = system.main;
            module.startColor = color;

            VfxStatusSystem.Profile profile = new VfxStatusSystem.Profile
            {
                type = type,
                applyEffectPrefab = obj
            };
            vfx.profiles = vfx.profiles.AddItem(profile).ToArray();
            vfx.profileLookup["juice"] = profile;
        }

        public static void PopupText(Transform transform, string s, bool localized = true)
        {
            NoTargetTextSystem noText = GameSystem.FindObjectOfType<NoTargetTextSystem>();
            if (noText != null)
            {
                TMP_Text textElement = noText.textElement;
                StringTable tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
                String text = localized ? tooltips.GetString(s).GetLocalizedString() : s;
                textElement.text = text;
                noText.PopText(transform.position);
            }
        }

        public static void AddLookup(string summoned, string summoner)
        {
            CreatedByLookup.lookup[summoned] = summoner;
        }

        public static void LoadPanels(WildfrostMod mod)
        {
            panel = ADD.ASprite("Panel");
            panelSmall = ADD.ASprite("PanelSmall");
            /*
            Texture2D tex = mod.ImagePath("Panel.png").ToTex();
            panel = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), 0.5f * Vector2.one, 15000, 0, SpriteMeshType.FullRect, new Vector4(25, 175, 200, 25));
            tex = mod.ImagePath("PanelSmall.png").ToTex();
            panelSmall = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), 0.5f * Vector2.one, 15000, 0, SpriteMeshType.FullRect, new Vector4(50, 125, 150, 50));
            */
        }
        public static T[] RemoveNulls<T>(this T[] data, WildfrostMod mod) where T : DataFile
        {
            List<T> list = data.ToList();
            list.RemoveAll(x => x == null || x?.ModAdded == mod);
            return list.ToArray();
        }

        public static T CreateTrait<T>(string name, WildfrostMod mod, KeywordData keyword, params StatusEffectData[] effects) where T : TraitData
        {
            T trait = ScriptableObject.CreateInstance<T>();
            trait.name = name;
            trait.effects = effects;
            trait.keyword = keyword;
            trait.ModAdded = mod;
            AddressableLoader.AddToGroup<TraitData>("TraitData", trait);
            return trait;
        }

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
            status.hiddenKeywords = new KeywordData[0];
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
            Pokefrost.statusList.Add(status);
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", status);
            return status;
        }

        public static GameObject CreateButtonIcon(this WildfrostMod mod, string name, Sprite sprite, string type, string copyTextFrom, Color textColor, KeywordData[] keys)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetParent(Pokefrost.pokefrostUI.transform);
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
            gameObject.AddComponent<UINavigationItem>();

            return gameObject;
        }

        public static GameObject CreateIcon(this WildfrostMod mod, string name, Sprite sprite, string type, string copyTextFrom, Color textColor, KeywordData[] keys, int posX = 1)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetParent(Pokefrost.pokefrostUI.transform);
            gameObject.SetActive(false);
            StatusIcon icon = gameObject.AddComponent<StatusIconExt>();
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
            cardPopUp.posX = posX;
            cardHover.pop = cardPopUp;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta *= 0.01f;
            gameObject.SetActive(true);
            icon.type = type;
            cardIcons[type] = gameObject;

            return gameObject;
        }

        public static KeywordData CreateBasicKeyword(this WildfrostMod mod, string name, string title, string desc, bool useSmallPanel = false)
        {
            KeywordData data = ScriptableObject.CreateInstance<KeywordData>();
            data.name = name;
            KeyCollection.SetString(data.name + "_text", title);
            data.titleKey = KeyCollection.GetString(data.name + "_text");
            KeyCollection.SetString(data.name + "_desc", desc);
            data.descKey = KeyCollection.GetString(data.name + "_desc");
            data.ModAdded = mod;
            data.showName = true;
            data.panelSprite = useSmallPanel ? panelSmall : panel;
            data.panelColor = new Color(0.15f, 0.15f, 0.15f, 0.90f);
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", data);
            return data;
        }

        public static KeywordData CreateIconKeyword(this WildfrostMod mod, string name, string title, string desc, string icon, bool useSmallPanel = false)
        {
            KeywordData data = ScriptableObject.CreateInstance<KeywordData>();
            data.name = name;
            KeyCollection.SetString(data.name + "_text", title);
            data.titleKey = KeyCollection.GetString(data.name + "_text");
            KeyCollection.SetString(data.name + "_desc", desc);
            data.descKey = KeyCollection.GetString(data.name + "_desc");
            data.showIcon = true;
            data.showName = false;
            data.iconName = icon;
            data.ModAdded = mod;
            data.panelSprite = useSmallPanel ? panelSmall : panel;
            data.panelColor = new Color(0.15f, 0.15f, 0.15f, 0.90f);
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", data);
            return data;
        }

        public static KeywordData ChangeColor(this KeywordData data, Color? title = null, Color? body = null, Color? note = null)
        {
            if (title is Color c1) { data.titleColour = c1; }
            if (body is Color c2) { data.bodyColour = c2; }
            if (note is Color c3) { data.noteColour = c3; }
            return data;
        }

        public static KeywordData ChangePanel(this KeywordData data, Sprite panel = null, Color? color = null)
        {
            if (panel != null) { data.panelSprite = panel; }
            if (color is Color c1) { data.panelColor = c1; }
            else { data.panelColor = Color.white; }
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

        public static CampaignNodeTypeBuilder CreateCampaignNodeType<T>(this WildfrostMod mod, string name, string letter, bool canSkip = true) where T : CampaignNodeType
        {
            return new CampaignNodeTypeBuilder(mod)
                .Create<T>(name)
                .WithCanEnter(true)
                .WithCanLink(true)
                .WithInteractable(true)
                .WithCanSkip(canSkip)
                .WithMustClear(!canSkip)
                .WithLetter(letter)
                .WithZoneName(name);
        }

        public static GameModifierDataBuilder CreateBell(this WildfrostMod mod, string name, string title, string description)
        {
            return new GameModifierDataBuilder(mod)
                .Create(name)
                .WithTitle(title)
                .WithDescription(description)
                .WithRingSfxEvent(mod.Get<GameModifierData>("DoubleBlingsFromCombos").ringSfxEvent);
        }

        public static GameModifierDataBuilder ChangeSprites(this GameModifierDataBuilder b, string bell = null, string dinger = null)
        {
            GameModifierData data = b._data;
            //Bell Sprite
            if (!bell.IsNullOrEmpty())
            {
                Texture2D tex = Pokefrost.instance.ImagePath(bell).ToTex();
                data.bellSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.9f), 327);
            }
            //Dinger Sprite
            if (!dinger.IsNullOrEmpty())
            {
                Texture2D tex2 = Pokefrost.instance.ImagePath(dinger).ToTex();
                data.dingerSprite = Sprite.Create(tex2, new Rect(0, 0, tex2.width, tex2.height), new Vector2(0.5f, 1.5f), 327);
            }
            return b;
        }

        public static CampaignNodeTypeBuilder BetterEvent(this CampaignNodeTypeBuilder cn, string key, WildfrostMod mod)
        {
            MapNode mapNode = mod.Get<CampaignNodeType>("CampaignNodeCharm").mapNodePrefab.InstantiateKeepName();
            mapNode.transform.SetParent(Pokefrost.pokefrostUI.transform, false);
            StringTable collection = LocalizationHelper.GetCollection("UI Text", SystemLanguage.English);
            collection.SetString("map_" + mapNode.name, key);
            mapNode.label.GetComponentInChildren<LocalizeStringEvent>().StringReference = collection.GetString("map_" + mapNode.name);
            mapNode.spriteOptions[0] = ADD.ASprite("trade_event");
            mapNode.clearedSpriteOptions[0] = ADD.ASprite("trade_done");
            return cn.WithMapNodePrefab(mapNode)
                .FreeModify<CampaignNodeTypeTrade>((data) =>
                {
                    data.key = key;
                });
        }

        public static CampaignNodeTypeBuilder BetterBattle(this CampaignNodeTypeBuilder cn, WildfrostMod mod)
        {
            MapNode mapNode = mod.Get<CampaignNodeType>("CampaignNodeBattle").mapNodePrefab.InstantiateKeepName();
            mapNode.transform.SetParent(Pokefrost.pokefrostUI.transform, false);
            return cn.WithMapNodePrefab(mapNode);
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

        public static CardUpgradeDataBuilder SEffects(this CardUpgradeDataBuilder b, params (string, int)[] statusEffects)
        {
            return b.SubscribeToAfterAllBuildEvent(
                (data) =>
                {
                    CardData.StatusEffectStacks[] stacks = statusEffects.Select((status) => new CardData.StatusEffectStacks(Pokefrost.instance.Get<StatusEffectData>(status.Item1), status.Item2)).ToArray();
                    data.effects = stacks;
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

        public static CardUpgradeDataBuilder SAttackEffects(this CardUpgradeDataBuilder b, params (string, int)[] statusEffects)
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

        public static CardUpgradeDataBuilder STraits(this CardUpgradeDataBuilder b, params (string, int)[] traits)
        {
            return b.SubscribeToAfterAllBuildEvent(
                (data) =>
                {
                    CardData.TraitStacks[] t = traits.Select((tr) => new CardData.TraitStacks(Pokefrost.instance.Get<TraitData>(tr.Item1), tr.Item2)).ToArray();
                    data.giveTraits = t;
                });
        }
    }


}

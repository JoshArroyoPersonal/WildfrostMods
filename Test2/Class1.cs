using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;
using UnityEngine.Localization.Tables;
using WildfrostHopeMod.Utils;
using WildfrostHopeMod.VFX;
using WildfrostHopeMod.SFX;
using TMPro;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using Extensions = Deadpan.Enums.Engine.Components.Modding.Extensions;


namespace Tutorial6_StatusIcons
{

    public static class Ext
    {
        public static StringTable Collection => LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
        public static StringTable KeyCollection => LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);


        // Creates the GameObject that is the icon
        // Make sure type is set to the same string as what you set type to for your status effect
        // copyTextFrom copies the text formating from an existing icon
        public static GameObject CreateIcon(this WildfrostMod mod, string name, Sprite sprite, string type, string copyTextFrom, Color textColor, KeywordData[] keys)
        {
            GameObject gameObject = new GameObject(name);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
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

        //This creates the keyword
        public static KeywordData CreateIconKeyword(this WildfrostMod mod, string name, string title, string desc, string icon, Color body, Color titleC, Color panel)
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
            data.bodyColour = body;
            data.titleColour = titleC;
            data.panelColor = panel;
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", data);
            return data;
        }


        //This custom class extends the StatusIcon class to automatically add listeners so that the number on the icon will update automatically
        public class StatusIconExt : StatusIcon
        {
            public override void Assign(Entity entity)
            {
                base.Assign(entity);
                SetText();
                onValueDown.AddListener(delegate { Ping(); });
                onValueUp.AddListener(delegate { Ping(); });
                afterUpdate.AddListener(SetText);
                onValueDown.AddListener(CheckDestroy);
            }
        }

    }

    public class StatusEffectVamp : StatusEffectApplyXWhenHit
    {

        public Hit storedHit;

        public override void Init()
        {
            base.OnTurnEnd += Decrease;
            base.Init();
        }

        public override bool RunHitEvent(Hit hit)
        {

            if (target.enabled && hit.target == target && hit.canRetaliate && (!targetMustBeAlive || (target.alive && Battle.IsOnBoard(target))) && hit.Offensive && hit.BasicHit)
            {
                
                storedHit = hit;

                return false;
            }

            return false;
        }

        public override bool RunPostHitEvent(Hit hit)
        {
            return hit == storedHit;
        }

        public override bool RunTurnEndEvent(Entity entity)
        {
            if (target.enabled)
            {
                return entity == target;
            }

            return false;
        }

        public IEnumerator Decrease(Entity entity) 
        {
            int amount = 1;
            Events.InvokeStatusEffectCountDown(this, ref amount);
            if (amount != 0)
            {
                yield return CountDown(entity, amount);
            }
        }

    }

    public class Tutorial6 : WildfrostMod
    {
        public Tutorial6(string modDirectory)
          : base(modDirectory)
        {
        }


        public override string GUID => "websiteofsites.wildfrost.tutorial";
        public override string[] Depends => new string[] { };
        public override string Title => "Tutorial 6";
        public override string Description => "Learn how to make a new status effect with a custom icon!";

        //usual stuff here

        private bool preLoaded = false;
        public static List<object> assets = new List<object>();
        private CardData.StatusEffectStacks SStack(string name, int amount) => new CardData.StatusEffectStacks(Get<StatusEffectData>(name), amount);
        public static StringTable Collection => LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
        public static StringTable keycollection => LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);

        
        //this is here to allow us to make video and sound effects
        public static GIFLoader VFX;
        public static SFXLoader SFX;

        //this is here to allow our icon to appear in the text box of cards
        public TMP_SpriteAsset assetSprites;
        public override TMP_SpriteAsset SpriteAsset => assetSprites;



        private void CreateModAssets()
        {

            //Needed to get sprites in text boxes
            assetSprites = HopeUtils.CreateSpriteAsset("assetSprites", directoryWithPNGs: this.ImagePath("Sprites"), textures: new Texture2D[] { }, sprites: new Sprite[] { });

            foreach (var character in assetSprites.spriteCharacterTable)
            {
                character.scale = 1.3f;
            }

            this.CreateIconKeyword("vamp", "Vamp", "When hit, restore <keyword=health> to the attacker equal to damage dealt|Counts down each turn", "vampicon"
                , new Color(1f, 1f, 1f), new Color(0.627f, 0.125f, 0.941f), new Color(0f, 0f, 0f));

            //make sure you icon is in both the images folder and the sprites subfolder
            this.CreateIcon("vampicon", ImagePath("vampicon.png").ToSprite(), "vamp", "frost", Color.white, new KeywordData[] { Get<KeywordData>("vamp") })
                .GetComponentInChildren<TextMeshProUGUI>(true).enabled = true;

            var vampEffect = new StatusEffectDataBuilder(this)
                .Create<StatusEffectVamp>("Vamp")
                .WithIconGroupName("health")
                .WithVisible(true)
                .WithIsStatus(true)
                .WithStackable(true)
                .WithTextInsert("{a}")
                .WithKeyword("vamp")
                .WithType("vamp")
                .FreeModify<StatusEffectApplyXWhenHit>(
                    delegate (StatusEffectApplyXWhenHit data)
                    {
                        data.applyEqualAmount = true;
                        data.effectToApply = Get<StatusEffectData>("Heal");
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Attacker;
                        data.applyFormatKey = Get<StatusEffectData>("Shroom").applyFormatKey; //Just makes it say "Apply {0}" for attack effects, however copying it lets it work in all languages
                        data.targetMustBeAlive = false;
                    });

            assets.Add(vampEffect);

            assets.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeVamp")
                    .WithTier(1)
                    .WithImage("vampcharm.png")
                    .SetConstraints(Get<CardUpgradeData>("CardUpgradeShroom").targetConstraints)
                    .SetBecomesTarget(true)
                    .WithTitle("Bat Charm")
                    .WithText("Apply <3><keyword=vamp>")
                    .SubscribeToAfterAllBuildEvent(
                        (data) =>
                        {
                            data.attackEffects = new CardData.StatusEffectStacks[] { SStack("Vamp", 3) };
                        }
                    )
            );



            //Code for cards

            preLoaded = true;
        }

        public override List<T> AddAssets<T, Y>()
        {
            if (assets.OfType<T>().Any())
                Debug.LogWarning($"[{Title}] adding {typeof(Y).Name}s: {assets.OfType<T>().Count()}");
            return assets.OfType<T>().ToList();
        }

        public override void Load()
        {
            if (!preLoaded) { CreateModAssets(); }

            base.Load();

            //needed for custom icons
            FloatingText ftext = GameObject.FindObjectOfType<FloatingText>(true);
            ftext.textAsset.spriteAsset.fallbackSpriteAssets.Add(assetSprites);


        }
        public override void Unload()
        {
            base.Unload();
        }



    }




}


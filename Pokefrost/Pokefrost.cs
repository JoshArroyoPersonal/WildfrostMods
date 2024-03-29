﻿using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Localization.Tables;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;
using AssortedPatchesCollection;
using Rewired;
using UnityEngine.Localization;
using System.IO;
using TMPro;
using Rewired.Utils.Attributes;
using System.Runtime.CompilerServices;
using static CombineCardSystem;
using Extensions = Deadpan.Enums.Engine.Components.Modding.Extensions;
using static Text;

namespace Pokefrost
{
    public class Pokefrost : WildfrostMod
    {
        public static string[] basicPool = new string[] {"croagunk", "salazzle", "magcargo" };
        public static string[] magicPool = new string[] { "carvanha", "duskull", "chandelure" };
        public static string[] clunkPool = new string[] { "weezing", "hippowdon", "trubbish" };

        private List<CardDataBuilder> list;
        private List<CardUpgradeDataBuilder> charmlist;
        private List<StatusEffectData> statusList;
        private bool preLoaded = false;
        private static float shinyrate = 1/100f;
        public static WildfrostMod instance;

        public Pokefrost(string modDirectory) : base(modDirectory)
        {
            instance = this;
        }

        private void CreateModAssets()
        {
            statusList = new List<StatusEffectData>(30);
            /*
            FloatingText floating = GameObject.FindObjectOfType<FloatingText>(true);
            Debug.Log("Got Icon sheet"+floating.name.ToString());
            TMP_SpriteAsset iconsheet = floating.GetComponentInChildren<TextMeshProUGUI>().spriteAsset;
            for(int i = 0; i < 144; i++)
            {
                TMP_Sprite testaroo = new TMP_Sprite { sprite = iconsheet.spriteGlyphTable[i].sprite };
                iconsheet.spriteInfoList.Add(testaroo);
            }
            TMP_Sprite testaroo2 = new TMP_Sprite { sprite = this.ImagePath("overshroomicon.png").ToSprite() };
            iconsheet.spriteInfoList.Add(testaroo2);
            Debug.Log("Got Glyph");
            Debug.Log("[Josh] Changed icon_sheet");*/

            TMP_SpriteAsset tempSpriteAsset = (Resources.FindObjectsOfTypeAll(typeof(TMP_SpriteAsset)) as TMP_SpriteAsset[])[1];
            TextMeshProUGUI.OnSpriteAssetRequest += new Func<int, string, TMP_SpriteAsset>((i, s) => { return s == "test" ? tempSpriteAsset : null; });

            StringTable keycollection = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
            KeywordData evolvekey = Get<KeywordData>("explode").InstantiateKeepName();
            evolvekey.name = "Evolve";
            keycollection.SetString(evolvekey.name + "_text", "Evolve");
            evolvekey.titleKey = keycollection.GetString(evolvekey.name + "_text");
            keycollection.SetString(evolvekey.name + "_desc", "If the condition is met at the end of battle evolve into a new Pokemon|Inactive while in reserve");
            evolvekey.descKey = keycollection.GetString(evolvekey.name + "_desc");
            evolvekey.panelSprite = this.ImagePath("panel.png").ToSprite();
            evolvekey.panelColor = new Color(1f, 1f, 1f);
            evolvekey.ModAdded = this;
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", evolvekey);

            KeywordData tauntkey = Get<KeywordData>("hellbent").InstantiateKeepName();
            tauntkey.name = "Taunt";
            keycollection.SetString(tauntkey.name + "_text", "Taunt");
            tauntkey.titleKey = keycollection.GetString(tauntkey.name + "_text");
            keycollection.SetString(tauntkey.name + "_desc", "All enmeies are <keyword=taunted>");
            tauntkey.descKey = keycollection.GetString(tauntkey.name + "_desc");
            tauntkey.ModAdded = this;
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", tauntkey);

            KeywordData tauntedkey = Get<KeywordData>("hellbent").InstantiateKeepName();
            tauntedkey.name = "Taunted";
            keycollection.SetString(tauntedkey.name + "_text", "Taunted");
            tauntedkey.titleKey = keycollection.GetString(tauntedkey.name + "_text");
            keycollection.SetString(tauntedkey.name + "_desc", "Target only enemies with <keyword=taunt>|Hits them all!");
            tauntedkey.descKey = keycollection.GetString(tauntedkey.name + "_desc");
            tauntedkey.ModAdded = this;
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", tauntedkey);

            KeywordData randomkey = Get<KeywordData>("hit").InstantiateKeepName();
            randomkey.name = "Random Effect";
            keycollection.SetString(randomkey.name + "_text", "Random Effect");
            randomkey.titleKey = keycollection.GetString(randomkey.name + "_text");
            keycollection.SetString(randomkey.name + "_desc", "Does a random effect from the listed options");
            randomkey.descKey = keycollection.GetString(randomkey.name + "_desc");
            randomkey.panelColor = new Color(0.75f, 0.42f, 0.94f);
            randomkey.bodyColour = new Color(0.1f, 0.1f, 0.1f);
            randomkey.titleColour = new Color(0, 0, 0);
            randomkey.ModAdded = this;
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", randomkey);

            StatusEffectFreeTrait wilder = ScriptableObject.CreateInstance<StatusEffectFreeTrait>();
            wilder.trait = this.Get<TraitData>("Wild");
            wilder.silenced = null;
            wilder.added = null;
            wilder.addedAmount = 0;
            wilder.targetConstraints = new TargetConstraint[0];
            wilder.offensive = true;
            wilder.isKeyword = false;
            wilder.stackable = false;
            StringTable collection = LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
            wilder.name = "Apply Wild Trait";
            collection.SetString(wilder.name + "_text", "Apply <keyword=wild>");
            wilder.textKey = collection.GetString(wilder.name + "_text");
            wilder.ModAdded = this;
            wilder.textInsert = "Wild Party!";
            wilder.applyFormat = "";
            wilder.type = "";
            //wilder.textKey = this.Get<StatusEffectData>("Instant Gain Aimless").textKey;
            wilder.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", wilder);
            statusList.Add(wilder);

            Debug.Log("[Pokefrost] Before Overshroom");
            //Overshroom Start
            GameObject gameObject = new GameObject("OvershroomIcon");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            StatusIcon overshroomicon = gameObject.AddComponent<StatusIcon>();
            Dictionary<string, GameObject> dicty = CardManager.cardIcons;
            GameObject text = dicty["shroom"].GetComponentInChildren<TextMeshProUGUI>().gameObject.InstantiateKeepName();
            text.transform.SetParent(gameObject.transform);
            overshroomicon.textElement = text.GetComponent<TextMeshProUGUI>();
            overshroomicon.onCreate = new UnityEngine.Events.UnityEvent();
            overshroomicon.onDestroy = new UnityEngine.Events.UnityEvent();
            overshroomicon.onValueDown = new UnityEventStatStat();
            overshroomicon.onValueUp = new UnityEventStatStat();
            overshroomicon.textColour = dicty["shroom"].GetComponent<StatusIcon>().textColour;
            overshroomicon.textColourAboveMax = overshroomicon.textColour;
            overshroomicon.textColourBelowMax = overshroomicon.textColour;
            UnityEngine.Events.UnityEvent afterupdate = new UnityEngine.Events.UnityEvent();
            overshroomicon.afterUpdate = afterupdate;
            UnityEngine.UI.Image image = gameObject.AddComponent<UnityEngine.UI.Image>();
            gameObject.SetActive(false);
            image.sprite = this.ImagePath("overshroomicon.png").ToSprite();
            CardHover cardHover = gameObject.AddComponent<CardHover>();
            cardHover.enabled = false;
            cardHover.IsMaster = false;
            CardPopUpTarget cardPopUp = gameObject.AddComponent<CardPopUpTarget>();

            Debug.Log("[Pokefrost] Overshroom 1");

            cardHover.pop = cardPopUp;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta *= 0.01f;
            gameObject.SetActive(true);
            overshroomicon.type = "overshroom";
            dicty["overshroom"] = gameObject;

            Debug.Log("[Pokefrost] Overshroom 2");

            KeywordData overshroomkey = Get<KeywordData>("shroom").InstantiateKeepName();
            overshroomkey.name = "Overshroom";
            keycollection.SetString(overshroomkey.name + "_text", "Overshroom");
            overshroomkey.titleKey = keycollection.GetString(overshroomkey.name + "_text");
            keycollection.SetString(overshroomkey.name + "_desc", "Acts like both <sprite name=overload> and <sprite name=shroom>|Counts as both too!");
            overshroomkey.descKey = keycollection.GetString(overshroomkey.name + "_desc");
            overshroomkey.noteColour = new Color(0, 0.6f, 0.6f);
            overshroomkey.titleColour = new Color(0, 0.6f, 0.6f);
            overshroomkey.showIcon = false;
            overshroomkey.showName = true;
            overshroomkey.ModAdded = this;
            overshroomkey.iconName = "icon_sheet_131";
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", overshroomkey);

            cardPopUp.keywords = new KeywordData[1] { overshroomkey };

            StatusEffectDummy dummyoverload = ScriptableObject.CreateInstance<StatusEffectDummy>();
            dummyoverload.name = "Overload";
            dummyoverload.type = "overload";
            dummyoverload.iconGroupName = "health";
            dummyoverload.visible = false;
            dummyoverload.targetConstraints = new TargetConstraint[0];
            dummyoverload.applyFormat = "";
            dummyoverload.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            dummyoverload.keyword = "";
            dummyoverload.textOrder = 0;
            dummyoverload.textInsert = "";
            //dummyoverload.ModAdded = this;

            StatusEffectDummy dummyshroom = ScriptableObject.CreateInstance<StatusEffectDummy>();
            dummyshroom.name = "Shroom";
            dummyshroom.type = "shroom";
            dummyshroom.iconGroupName = "health";
            dummyshroom.visible = false;
            dummyshroom.targetConstraints = new TargetConstraint[0];
            dummyshroom.applyFormat = "";
            dummyshroom.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            dummyshroom.keyword = "";
            dummyshroom.textOrder = 0;
            dummyshroom.textInsert = "";
            //dummyshroom.ModAdded = this;

            Debug.Log("[Pokefrost] Overshroom 3");

            StatusEffectOvershroom overshroom = ScriptableObject.CreateInstance<StatusEffectOvershroom>();
            overshroom.name = "Overshroom";
            overshroom.type = "overshroom";
            overshroom.dummy1 = dummyoverload;
            overshroom.dummy2 = dummyshroom;
            overshroom.visible = true;
            overshroom.stackable = true;
            overshroom.buildupAnimation = ((StatusEffectOverload)Get<StatusEffectData>("Overload")).buildupAnimation;
            overshroom.iconGroupName = "health";
            overshroom.offensive = true;
            overshroom.applyFormat = "";
            overshroom.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            overshroom.keyword = "";
            overshroom.targetConstraints = new TargetConstraint[1] { new TargetConstraintCanBeHit() };
            overshroom.textOrder = 0;
            overshroom.textInsert = "{a}";
            overshroom.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", overshroom);
            statusList.Add(overshroom);

            Debug.Log("[Pokefrost] Overshroom 4");

            StatusEffectBecomeOvershroom giveovershroom = ScriptableObject.CreateInstance<StatusEffectBecomeOvershroom>();
            giveovershroom.name = "Turn Overload and Shroom to Overshroom";
            giveovershroom.applyFormat = "";
            giveovershroom.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            giveovershroom.keyword = "";
            giveovershroom.targetConstraints = new TargetConstraint[0];
            giveovershroom.textKey = new UnityEngine.Localization.LocalizedString();
            giveovershroom.type = "";
            giveovershroom.textOrder = 0;
            giveovershroom.textInsert = "";
            giveovershroom.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", giveovershroom);
            statusList.Add(giveovershroom);

            Debug.Log("[Pokefrost] Overshroom 5");

            StatusEffectWhileActiveX activeovershroom = ScriptableObject.CreateInstance<StatusEffectWhileActiveX>();
            activeovershroom.applyConstraints = new TargetConstraint[0];
            activeovershroom.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            activeovershroom.doPing = true;
            activeovershroom.effectToApply = giveovershroom;
            activeovershroom.pauseAfter = 0;
            activeovershroom.targetMustBeAlive = true;
            activeovershroom.applyFormat = "";
            activeovershroom.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            activeovershroom.keyword = "";
            activeovershroom.hiddenKeywords = new KeywordData[0];
            activeovershroom.targetConstraints = new TargetConstraint[0];
            activeovershroom.textInsert = "";
            activeovershroom.name = "While Active It Is Overshroom";
            collection.SetString(activeovershroom.name + "_text", "While active, your <keyword=overload> and <keyword=shroom> become <keyword=overshroom>");
            activeovershroom.textKey = collection.GetString(activeovershroom.name + "_text");
            activeovershroom.textOrder = 0;
            activeovershroom.type = "";
            activeovershroom.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", activeovershroom);
            statusList.Add(activeovershroom);
            //Overshroom End

            Debug.Log("[Pokefrost] After Overshroom");


            StatusEffectApplyXWhenHitFree supergreed = ScriptableObject.CreateInstance<StatusEffectApplyXWhenHitFree>();
            supergreed.attackerConstraints = new TargetConstraint[0];
            supergreed.applyConstraints = new TargetConstraint[0];
            supergreed.applyToFlags = StatusEffectApplyX.ApplyToFlags.Attacker;
            supergreed.effectToApply = ((StatusEffectApplyX)Get<StatusEffectData>("Pre Turn Take Gold")).effectToApply;
            //supergreed.eqaulAmountBonusMult = 0 probably private
            //supergreed.noTargetType = None probably private
            //supergreed.noTargetTypeArgs probably private
            supergreed.pauseAfter = 0;
            supergreed.applyFormat = "";
            supergreed.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            supergreed.canBeBoosted = true;
            supergreed.count = 0;
            supergreed.eventPriority = 0;
            supergreed.hiddenKeywords = new KeywordData[] { Get<KeywordData>("hit") };
            supergreed.iconGroupName = "";
            supergreed.keyword = "";
            supergreed.stackable = true;
            supergreed.targetConstraints = new TargetConstraint[0];
            supergreed.temporary = 0;
            supergreed.textInsert = "<{a}><keyword=blings>";
            supergreed.name = "Drop Bling on Hit";
            collection.SetString(supergreed.name + "_text", "Lose <{a}><keyword=blings> when hit");
            supergreed.textKey = collection.GetString(supergreed.name + "_text");
            supergreed.textOrder = 0;
            supergreed.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", supergreed);
            statusList.Add(supergreed);

            StatusEffectApplyXOnTurn hazeall = ScriptableObject.CreateInstance<StatusEffectApplyXOnTurn>();
            hazeall.applyConstraints = new TargetConstraint[0];
            hazeall.applyToFlags = StatusEffectApplyX.ApplyToFlags.Allies | StatusEffectApplyX.ApplyToFlags.Enemies | StatusEffectApplyX.ApplyToFlags.Self;
            hazeall.doPing = true;
            hazeall.effectToApply = Get<StatusEffectData>("Haze");
            hazeall.pauseAfter = 0;
            hazeall.targetMustBeAlive = true;
            hazeall.waitForAnimationEnd = true;
            hazeall.applyFormat = "";
            hazeall.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            hazeall.canBeBoosted = true;
            hazeall.keyword = "";
            hazeall.stackable = true;
            hazeall.targetConstraints = new TargetConstraint[0];
            hazeall.textInsert = "<{a}><keyword=haze>";
            hazeall.name = "Apply Haze to All";
            collection.SetString(hazeall.name + "_text", "Apply <{a}><keyword=haze> to all");
            hazeall.textKey = collection.GetString(hazeall.name + "_text");
            hazeall.textOrder = 0;
            hazeall.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", hazeall);
            statusList.Add(hazeall);

            StatusEffectApplyXOnTurn inkall = hazeall.InstantiateKeepName();
            inkall.effectToApply = Get<StatusEffectData>("Null");
            inkall.name = "Apply Ink to All";
            inkall.textInsert = "<{a}><keyword=null>";
            collection.SetString(inkall.name + "_text", "Apply <{a}><keyword=null> to all");
            inkall.textKey = collection.GetString(inkall.name + "_text");
            inkall.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", inkall);
            statusList.Add(inkall);

            StatusEffectApplyXOnTurn selfheal = ScriptableObject.CreateInstance<StatusEffectApplyXOnTurn>();
            selfheal.applyConstraints = new TargetConstraint[0];
            selfheal.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            selfheal.doPing = true;
            selfheal.effectToApply = Get<StatusEffectData>("Heal (No Ping)");
            selfheal.pauseAfter = 0;
            selfheal.targetMustBeAlive = true;
            selfheal.waitForAnimationEnd = true;
            selfheal.applyFormat = "";
            selfheal.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            selfheal.canBeBoosted = true;
            selfheal.keyword = "";
            selfheal.stackable = true;
            selfheal.targetConstraints = new TargetConstraint[0];
            selfheal.textInsert = "<{a}><keyword=health>";
            selfheal.name = "Heal Self";
            collection.SetString(selfheal.name + "_text", "Restore <{a}><keyword=health> to self");
            selfheal.textKey = collection.GetString(selfheal.name + "_text");
            selfheal.textOrder = 0;
            selfheal.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", selfheal);
            statusList.Add(selfheal);

            StatusEffectApplyXOnTurn restoreall = ScriptableObject.CreateInstance<StatusEffectApplyXOnTurn>();
            restoreall.applyConstraints = new TargetConstraint[0];
            restoreall.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self | StatusEffectApplyX.ApplyToFlags.Allies;
            restoreall.doPing = true;
            restoreall.effectToApply = Get<StatusEffectData>("Heal & Cleanse (No Ping)");
            restoreall.pauseAfter = 0;
            restoreall.targetMustBeAlive = true;
            restoreall.waitForAnimationEnd = true;
            restoreall.applyFormat = "";
            restoreall.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            restoreall.canBeBoosted = true;
            restoreall.keyword = "";
            restoreall.stackable = true;
            restoreall.targetConstraints = new TargetConstraint[0];
            restoreall.textInsert = "<{a}><keyword=health>";
            restoreall.name = "Heal & Cleanse All";
            collection.SetString(restoreall.name + "_text", "Restore <{a}><keyword=health> and <keyword=cleanse> all allies and self");
            restoreall.textKey = collection.GetString(restoreall.name + "_text");
            restoreall.textOrder = 0;
            restoreall.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", restoreall);
            statusList.Add(restoreall);

            StatusEffectWhileActiveX boostallies = ScriptableObject.CreateInstance<StatusEffectWhileActiveX>();
            boostallies.applyConstraints = new TargetConstraint[0];
            boostallies.applyToFlags = StatusEffectApplyX.ApplyToFlags.AlliesInRow;
            boostallies.doPing = true;
            boostallies.effectToApply = Get<StatusEffectData>("Ongoing Increase Effects");
            boostallies.pauseAfter = 0;
            boostallies.targetMustBeAlive = true;
            boostallies.applyFormat = "";
            boostallies.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            boostallies.canBeBoosted = true;
            boostallies.keyword = "";
            boostallies.stackable = true;
            boostallies.targetConstraints = new TargetConstraint[0];
            boostallies.textInsert = "";
            boostallies.name = "Boost Allies";
            collection.SetString(boostallies.name + "_text", "While active, boost the effects of allies in the row by <{a}>");
            boostallies.textKey = collection.GetString(boostallies.name + "_text");
            boostallies.textOrder = 0;
            boostallies.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", boostallies);
            statusList.Add(boostallies);

            StatusEffectWhileActiveX unmovable = ScriptableObject.CreateInstance<StatusEffectWhileActiveX>();
            unmovable.applyConstraints = new TargetConstraint[0];
            unmovable.applyToFlags = StatusEffectApplyX.ApplyToFlags.FrontAlly;
            unmovable.doPing = true;
            unmovable.effectToApply = Get<StatusEffectData>("Temporary Unmovable");
            unmovable.pauseAfter = 0;
            unmovable.targetMustBeAlive = true;
            unmovable.applyFormat = "";
            unmovable.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            unmovable.keyword = "";
            unmovable.hiddenKeywords = new KeywordData[] { Get<KeywordData>("Active") };
            unmovable.targetConstraints = new TargetConstraint[0];
            unmovable.textInsert = "<keyword=unmovable>";
            unmovable.name = "Unmovable to Some Allies";
            collection.SetString(unmovable.name + "_text", "While active, add <keyword=unmovable> to allies in other row");
            unmovable.textKey = collection.GetString(unmovable.name + "_text");
            unmovable.textOrder = 0;
            unmovable.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", unmovable);
            statusList.Add(unmovable);

            StatusEffectApplyXOnCardPlayed hitrow = ScriptableObject.CreateInstance<StatusEffectApplyXOnCardPlayed>();
            hitrow.applyConstraints = new TargetConstraint[0];
            hitrow.applyToFlags = StatusEffectApplyX.ApplyToFlags.AlliesInRow;
            hitrow.effectToApply = Get<StatusEffectData>("Trigger Against (Don't Count As Trigger)");
            hitrow.pauseAfter = 0;
            hitrow.separateActions = true;
            hitrow.targetMustBeAlive = true;
            hitrow.applyFormat = "";
            hitrow.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            hitrow.keyword = "";
            hitrow.targetConstraints = new TargetConstraint[0];
            hitrow.textInsert = "Hit Row";
            hitrow.name = "Hit Your Row";
            collection.SetString(hitrow.name + "_text", "Also hits allies in row");
            hitrow.textKey = collection.GetString(hitrow.name + "_text");
            hitrow.textOrder = 0;
            hitrow.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", hitrow);
            statusList.Add(hitrow);

            StatusEffectApplyXOnCardPlayed sundrum = ScriptableObject.CreateInstance<StatusEffectApplyXOnCardPlayed>();
            sundrum.applyConstraints = new TargetConstraint[0];
            sundrum.applyToFlags = StatusEffectApplyX.ApplyToFlags.AlliesInRow;
            sundrum.effectToApply = Get<StatusEffectData>("Reduce Counter");
            sundrum.pauseAfter = 0;
            sundrum.separateActions = true;
            sundrum.targetMustBeAlive = true;
            sundrum.canBeBoosted = true;
            sundrum.applyFormat = "";
            sundrum.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            sundrum.keyword = "";
            sundrum.targetConstraints = new TargetConstraint[0];
            sundrum.textInsert = "Reduce Counter Row";
            sundrum.name = "On Card Played Reduce Counter Row";
            collection.SetString(sundrum.name + "_text", "Count down <keyword=counter> by <{a}> to allies in row");
            sundrum.textKey = collection.GetString(sundrum.name + "_text");
            sundrum.textOrder = 0;
            sundrum.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", sundrum);
            statusList.Add(sundrum);

            StatusEffectMultEffects tea = ScriptableObject.CreateInstance<StatusEffectMultEffects>();
            tea.effects = new List<StatusEffectData> { Get<StatusEffectData>("Increase Max Counter"), Get<StatusEffectData>("MultiHit") };
            tea.silenced = null;
            tea.targetConstraints = Get<CardData>("BlazeTea").attackEffects[1].data.targetConstraints;
            tea.canBeBoosted = true;
            tea.offensive = false;
            tea.isKeyword = false;
            tea.name = "Apply Blaze Tea";
            collection.SetString(tea.name + "_text", "Add <x{a}> <keyword=frenzy> and increase <keyword=counter> by <{a}>");
            tea.textKey = collection.GetString(tea.name + "_text");
            tea.ModAdded = this;
            tea.textInsert = "Tea Party!";
            tea.applyFormat = "";
            tea.type = "";
            tea.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", tea);
            statusList.Add(tea);

            StatusEffectApplyXOnCardPlayed blazetea = ScriptableObject.CreateInstance<StatusEffectApplyXOnCardPlayed>();
            blazetea.applyConstraints = new TargetConstraint[0];
            blazetea.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomAlly;
            blazetea.effectToApply = Get<StatusEffectData>("Apply Blaze Tea");
            blazetea.pauseAfter = 0;
            blazetea.separateActions = true;
            blazetea.targetMustBeAlive = true;
            blazetea.canBeBoosted = true;
            blazetea.applyFormat = "";
            blazetea.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            blazetea.keyword = "";
            blazetea.targetConstraints = Get<CardData>("BlazeTea").attackEffects[1].data.targetConstraints;
            blazetea.textInsert = "Add MultiHit To Random Ally";
            blazetea.name = "On Card Played Blaze Tea Random Ally";
            collection.SetString(blazetea.name + "_text", "Add <x{a}> <keyword=frenzy> and increase <keyword=counter> by <{a}> to a random ally");
            blazetea.textKey = collection.GetString(blazetea.name + "_text");
            blazetea.textOrder = 0;
            blazetea.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", blazetea);
            statusList.Add(blazetea);

            StatusEffectSummon scrap1 = Get<StatusEffectData>("Summon Junk").InstantiateKeepName() as StatusEffectSummon;
            scrap1.summonCard = Get<CardData>("ScrapPile");
            scrap1.name = "Summon Scrap Pile";
            collection.SetString(scrap1.name + "_text", "Summon Scrap Pile");
            scrap1.textKey = collection.GetString(scrap1.name + "_text");
            scrap1.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", scrap1);
            statusList.Add(scrap1);
            StatusEffectInstantSummon scrap2 = Get<StatusEffectData>("Instant Summon Junk In Hand").InstantiateKeepName() as StatusEffectInstantSummon;
            scrap2.targetSummon = Get<StatusEffectData>("Summon Scrap Pile") as StatusEffectSummon;
            scrap2.name = "Instant Summon Scrap Pile In Hand";
            collection.SetString(scrap2.name + "_text", "Add Scrap Pile to hand");
            scrap2.textKey = collection.GetString(scrap2.name + "_text");
            scrap2.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", scrap2);
            statusList.Add(scrap2);
            StatusEffectApplyXWhenHit scrap3 = Get<StatusEffectData>("When Hit Add Junk To Hand").InstantiateKeepName() as StatusEffectApplyXWhenHit;
            scrap3.effectToApply = Get<StatusEffectData>("Instant Summon Scrap Pile In Hand") as StatusEffectInstantSummon;
            scrap3.canBeBoosted = false;
            scrap3.name = "When Hit Add Scrap Pile To Hand";
            collection.SetString(scrap3.name + "_text", "Add <Scrap Pile> to hand when hit");
            scrap3.textKey = collection.GetString(scrap3.name + "_text");
            scrap3.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", scrap3);
            statusList.Add(scrap3);

            StatusEffectSummon askull1 = Get<StatusEffectData>("Summon Junk").InstantiateKeepName() as StatusEffectSummon;
            askull1.summonCard = Get<CardData>("ZapOrb");
            askull1.name = "Summon Azul Skull";
            collection.SetString(askull1.name + "_text", "Summon Azul Skull");
            askull1.textKey = collection.GetString(askull1.name + "_text");
            askull1.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", askull1);
            statusList.Add(askull1);
            StatusEffectInstantSummon askull2 = Get<StatusEffectData>("Instant Summon Junk In Hand").InstantiateKeepName() as StatusEffectInstantSummon;
            askull2.targetSummon = Get<StatusEffectData>("Summon Azul Skull") as StatusEffectSummon;
            askull2.name = "Instant Summon Azul Skull In Hand";
            collection.SetString(askull2.name + "_text", "Add Azul Skull to hand");
            askull2.textKey = collection.GetString(askull2.name + "_text");
            askull2.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", askull2);
            statusList.Add(askull2);
            StatusEffectSummon tskull1 = Get<StatusEffectData>("Summon Junk").InstantiateKeepName() as StatusEffectSummon;
            tskull1.summonCard = Get<CardData>("SharkTooth");
            tskull1.name = "Summon Tiger Skull";
            collection.SetString(tskull1.name + "_text", "Summon Tiger Skull");
            tskull1.textKey = collection.GetString(tskull1.name + "_text");
            tskull1.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", tskull1);
            statusList.Add(tskull1);
            StatusEffectInstantSummon tskull2 = Get<StatusEffectData>("Instant Summon Junk In Hand").InstantiateKeepName() as StatusEffectInstantSummon;
            tskull2.targetSummon = Get<StatusEffectData>("Summon Tiger Skull") as StatusEffectSummon;
            tskull2.name = "Instant Summon Tiger Skull In Hand";
            collection.SetString(tskull2.name + "_text", "Add Tiger Skull to hand");
            tskull2.textKey = collection.GetString(tskull2.name + "_text");
            tskull2.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", tskull2);
            statusList.Add(tskull2);
            StatusEffectSummon yskull1 = Get<StatusEffectData>("Summon Junk").InstantiateKeepName() as StatusEffectSummon;
            yskull1.summonCard = Get<CardData>("SnowMaul");
            yskull1.name = "Summon Yeti Skull";
            collection.SetString(yskull1.name + "_text", "Summon Yeti Skull");
            yskull1.textKey = collection.GetString(yskull1.name + "_text");
            yskull1.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", yskull1);
            statusList.Add(yskull1);
            StatusEffectInstantSummon yskull2 = Get<StatusEffectData>("Instant Summon Junk In Hand").InstantiateKeepName() as StatusEffectInstantSummon;
            yskull2.targetSummon = Get<StatusEffectData>("Summon Yeti Skull") as StatusEffectSummon;
            yskull2.name = "Instant Summon Yeti Skull In Hand";
            collection.SetString(yskull2.name + "_text", "Add Yeti Skull to hand");
            yskull2.textKey = collection.GetString(yskull2.name + "_text");
            yskull2.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", yskull2);
            statusList.Add(yskull2);
            StatusEffectApplyRandomOnCardPlayed duskulleffect = ScriptableObject.CreateInstance<StatusEffectApplyRandomOnCardPlayed>();
            duskulleffect.type = "duskull";
            duskulleffect.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            duskulleffect.effectsToapply = new StatusEffectData[] { askull2, tskull2, yskull2 };
            duskulleffect.canBeBoosted = false;
            duskulleffect.name = "When Ally Summoned Add Skull To Hand";
            duskulleffect.applyFormat = "";
            duskulleffect.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            duskulleffect.keyword = "";
            duskulleffect.targetConstraints = new TargetConstraint[0];
            collection.SetString(duskulleffect.name + "_text", "Add a random skull to hand");
            duskulleffect.textKey = collection.GetString(duskulleffect.name + "_text");
            duskulleffect.textOrder = 0;
            duskulleffect.textInsert = "";
            duskulleffect.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", duskulleffect);
            statusList.Add(duskulleffect);
            StatusEffectTriggerWhenSummonDeployed duskulltrigger = ScriptableObject.CreateInstance<StatusEffectTriggerWhenSummonDeployed>();
            duskulltrigger.name = "Trigger When Summon";
            duskulltrigger.isReaction = true;
            duskulltrigger.applyFormat = "";
            duskulltrigger.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            duskulltrigger.keyword = "";
            duskulltrigger.targetConstraints = new TargetConstraint[0];
            collection.SetString(duskulltrigger.name + "_text", "Trigger when anything is summoned");
            duskulltrigger.descColorHex = "F99C61";
            duskulltrigger.textKey = collection.GetString(duskulltrigger.name + "_text");
            duskulltrigger.textOrder = 0;
            duskulltrigger.textInsert = "";
            duskulltrigger.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", duskulltrigger);
            statusList.Add(duskulltrigger);

            StatusEffectApplyXOnCardPlayed demonizeally = Get<StatusEffectData>("On Card Played Apply Spice To RandomAlly").InstantiateKeepName() as StatusEffectApplyXOnCardPlayed;
            demonizeally.effectToApply = Get<StatusEffectData>("Demonize");
            demonizeally.name = "On Card Apply Demonize To RandomAlly";
            collection.SetString(demonizeally.name + "_text", "Apply <{a}> <keyword=demonize> to a random ally");
            demonizeally.textKey = collection.GetString(demonizeally.name + "_text");
            demonizeally.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", demonizeally);
            statusList.Add(demonizeally);

            StatusEffectApplyRandomOnCardPlayed triattack = ScriptableObject.CreateInstance<StatusEffectApplyRandomOnCardPlayed>();
            triattack.applyToFlags = StatusEffectApplyX.ApplyToFlags.Target;
            triattack.effectsToapply = new StatusEffectData[] { Get<StatusEffectData>("Shroom"), Get<StatusEffectData>("Overload"), Get<StatusEffectData>("Weakness") };
            triattack.canBeBoosted = true;
            triattack.name = "On Card Played Apply Shroom Overburn Or Bom";
            triattack.applyFormat = "";
            triattack.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            triattack.keyword = "";
            triattack.hiddenKeywords = new KeywordData[] {Get<KeywordData>("random effect")};
            triattack.targetConstraints = new TargetConstraint[0];
            collection.SetString(triattack.name + "_text", "Apply <{a}> <keyword=shroom>/<keyword=overload>/<keyword=weakness>");
            triattack.textKey = collection.GetString(triattack.name + "_text");
            triattack.textOrder = 0;
            triattack.textInsert = "<keyword=shroom>, <keyword=overload>, <keyword=weakness>";
            triattack.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", triattack);
            statusList.Add(triattack);

            StatusEffectApplyXPreTurn overburnself = Get<StatusEffectData>("Pre Turn Take Gold").InstantiateKeepName() as StatusEffectApplyXPreTurn;
            overburnself.effectToApply = Get<StatusEffectData>("Overload");
            overburnself.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            overburnself.name = "Overload Self";
            collection.SetString(overburnself.name + "_text", "Gain <{a}><keyword=overload>");
            overburnself.textKey = collection.GetString(overburnself.name + "_text");
            overburnself.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", overburnself);
            statusList.Add(overburnself);

            StatusEffectApplyXOnEffect overoverburn = ScriptableObject.CreateInstance<StatusEffectApplyXOnEffect>();
            overoverburn.applyToFlags = StatusEffectApplyX.ApplyToFlags.Target;
            overoverburn.doPing = false;
            overoverburn.conditionEffect = Get<StatusEffectData>("Overload");
            overoverburn.effectToApply = overoverburn.conditionEffect;
            overoverburn.applyEqualAmount = true;
            overoverburn.name = "Apply Overload Equal To Overload";
            overoverburn.applyFormat = "";
            overoverburn.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            overoverburn.keyword = "";
            overoverburn.targetConstraints = new TargetConstraint[0];
            collection.SetString(overoverburn.name + "_text", "Apply current <keyword=overload>");
            overoverburn.textKey = collection.GetString(overoverburn.name + "_text");
            overoverburn.textOrder = 0;
            overoverburn.textInsert = "";
            overoverburn.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", overoverburn);
            statusList.Add(overoverburn);

            TraitData aimlesstrait = Get<TraitData>("Aimless");
            TraitData barragetrait = Get<TraitData>("Barrage");
            TraitData longshottrait = Get<TraitData>("Longshot");

            KeywordData pluckkey = Get<KeywordData>("hellbent").InstantiateKeepName();
            pluckkey.name = "Pluck";
            keycollection.SetString(pluckkey.name + "_text", "Pluck");
            pluckkey.titleKey = keycollection.GetString(pluckkey.name + "_text");
            keycollection.SetString(pluckkey.name + "_desc", "Hits lowest health target in row|Prioritizes front target in case of tie");
            pluckkey.descKey = keycollection.GetString(pluckkey.name + "_desc");
            pluckkey.ModAdded = this;
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", pluckkey);

            TargetModeLowHealth lowhealthtarget = ScriptableObject.CreateInstance<TargetModeLowHealth>();
            StatusEffectChangeTargetMode pluckmode = Get<StatusEffectData>("Hit Random Target").InstantiateKeepName() as StatusEffectChangeTargetMode;
            pluckmode.name = "Hit Lowest Health Target";
            pluckmode.targetMode = lowhealthtarget;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", pluckmode);
            statusList.Add(pluckmode);
            TraitData plucktrait = Get<TraitData>("Aimless").InstantiateKeepName();
            plucktrait.name = "Pluck";
            plucktrait.keyword = pluckkey;
            StatusEffectData[] plucktemp = { pluckmode };
            plucktrait.effects = plucktemp;
            plucktrait.overrides = plucktrait.overrides.Append(aimlesstrait).ToArray();
            plucktrait.ModAdded = this;
            AddressableLoader.AddToGroup<TraitData>("TraitData", plucktrait);

            aimlesstrait.overrides = aimlesstrait.overrides.Append(plucktrait).ToArray();
            barragetrait.overrides = barragetrait.overrides.Append(plucktrait).ToArray();
            longshottrait.overrides = longshottrait.overrides.Append(plucktrait).ToArray();

            StatusEffectChangeTargetMode taunteffect = Get<StatusEffectData>("Hit All Enemies").InstantiateKeepName() as StatusEffectChangeTargetMode;
            taunteffect.name = "Hit All Taunt";
            taunteffect.targetMode = ScriptableObject.CreateInstance<TargetModeTaunt>();
            //collection.SetString(taunteffect.name + "_text", "");
            taunteffect.textKey = new UnityEngine.Localization.LocalizedString();
            taunteffect.ModAdded = this;

            //TargetModeAll taunttargetmode = taunteffect.targetMode.InstantiateKeepName() as TargetModeAll;

            StatusEffectWhileActiveX hittaunt = Get<StatusEffectData>("While Active Aimless To Enemies").InstantiateKeepName() as StatusEffectWhileActiveX;
            hittaunt.name = "Target Mode Taunt";
            hittaunt.keyword = "";
            //collection.SetString(hittaunt.name + "_text", "");
            hittaunt.textKey = new UnityEngine.Localization.LocalizedString();
            hittaunt.ModAdded = this;

            TraitData taunttrait = Get<TraitData>("Hellbent").InstantiateKeepName();
            taunttrait.name = "Taunt";
            taunttrait.keyword = tauntkey;
            StatusEffectData[] taunttemp = { hittaunt };
            taunttrait.effects = taunttemp;
            taunttrait.ModAdded = this;

            TraitData tauntedtrait = Get<TraitData>("Hellbent").InstantiateKeepName();
            tauntedtrait.name = "Taunted";
            tauntedtrait.keyword = tauntedkey;
            StatusEffectData[] tauntedtemp = { taunteffect };
            tauntedtrait.effects = tauntedtemp;
            TraitData[] tempoverrides = { Get<TraitData>("Aimless"), Get<TraitData>("Barrage"), Get<TraitData>("Longshot"), plucktrait };
            tauntedtrait.overrides = tempoverrides;
            tauntedtrait.ModAdded = this;

            TargetConstraintHasTrait tauntconstraint = ScriptableObject.CreateInstance<TargetConstraintHasTrait>();
            tauntconstraint.name = "Has Taunt Trait";
            tauntconstraint.trait = taunttrait;
            TargetConstraint[] temptaunteffect = { tauntconstraint };
            //taunttargetmode.constraints = temptaunteffect;
            //taunteffect.targetMode = taunttargetmode;

            StatusEffectTemporaryTrait imtaunted = Get<StatusEffectData>("Temporary Aimless").InstantiateKeepName() as StatusEffectTemporaryTrait;
            imtaunted.name = "Temporary Taunted";
            imtaunted.trait = tauntedtrait;
            //collection.SetString(imtaunted.name + "_text", "");
            //imtaunted.textKey = collection.GetString(imtaunted.name + "_text");
            imtaunted.ModAdded = this;

            hittaunt.effectToApply = imtaunted;

            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", taunteffect);
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", hittaunt);
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", imtaunted);
            statusList.Add(taunteffect); statusList.Add(hittaunt); statusList.Add(imtaunted);
            AddressableLoader.AddToGroup<TraitData>("TraitData", taunttrait);
            AddressableLoader.AddToGroup<TraitData>("TraitData", tauntedtrait);

            StatusEffectInstantEat woollydrekeat = Get<StatusEffectData>("Eat (Health, Attack & Effects)") as StatusEffectInstantEat;
            woollydrekeat.illegalEffects = woollydrekeat.illegalEffects.AddItem<StatusEffectData>(imtaunted).ToArray();

            StatusEffectApplyXOnHit shroomhit = Get<StatusEffectData>("On Hit Equal Snow To Target").InstantiateKeepName() as StatusEffectApplyXOnHit;
            shroomhit.effectToApply = Get<StatusEffectData>("Shroom");
            shroomhit.name = "On Hit Equal Shroom To Target";
            collection.SetString(shroomhit.name + "_text", "Apply <keyword=shroom> equal to damage dealt");
            shroomhit.textKey = collection.GetString(shroomhit.name + "_text");
            shroomhit.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", shroomhit);
            statusList.Add(shroomhit);

            StatusEffectTemporaryTrait gainexplode = Get<StatusEffectData>("Temporary Aimless").InstantiateKeepName() as StatusEffectTemporaryTrait;
            gainexplode.name = "Temporary Explode";
            gainexplode.trait = Get<TraitData>("Explode");
            gainexplode.ModAdded = this;
            gainexplode.targetConstraints = new TargetConstraint[0];
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", gainexplode);
            statusList.Add(gainexplode);

            StatusEffectApplyXOnCardPlayed explodeself = Get<StatusEffectData>("On Card Played Apply Attack To Self").InstantiateKeepName() as StatusEffectApplyXOnCardPlayed;
            explodeself.effectToApply = gainexplode;
            explodeself.canBeBoosted = true;
            explodeself.doesDamage = true;
            explodeself.targetConstraints = new TargetConstraint[0];
            explodeself.name = "On Card Played Give Self Explode";
            collection.SetString(explodeself.name + "_text", "Gain <keyword=explode> <{a}>");
            explodeself.textKey = collection.GetString(explodeself.name + "_text");
            explodeself.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", explodeself);
            statusList.Add(explodeself);

            StatusEffectApplyXPreTurn bomall = Get<StatusEffectData>("Pre Turn Take Gold").InstantiateKeepName() as StatusEffectApplyXPreTurn;
            bomall.effectToApply = Get<StatusEffectData>("Weakness");
            bomall.applyToFlags = StatusEffectApplyX.ApplyToFlags.Enemies;
            bomall.name = "Pre Turn Weakness All Enemies";
            collection.SetString(bomall.name + "_text", "Before triggering, apply <{a}> <keyword=weakness> to all enemies");
            bomall.textKey = collection.GetString(bomall.name + "_text");
            bomall.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", bomall);
            statusList.Add(bomall);

            StatusEffectTriggerWhenDamageType teethtrigger = ScriptableObject.CreateInstance<StatusEffectTriggerWhenDamageType>();
            teethtrigger.name = "Trigger When Teeth Damage";
            teethtrigger.isReaction = true;
            teethtrigger.applyFormat = "";
            teethtrigger.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            teethtrigger.keyword = "";
            teethtrigger.targetConstraints = new TargetConstraint[0];
            teethtrigger.triggerdamagetype = "spikes";
            collection.SetString(teethtrigger.name + "_text", "Trigger when <keyword=teeth> damage is taken");
            teethtrigger.descColorHex = "F99C61";
            teethtrigger.textKey = collection.GetString(teethtrigger.name + "_text");
            teethtrigger.textOrder = 0;
            teethtrigger.textInsert = "";
            teethtrigger.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", teethtrigger);
            statusList.Add(teethtrigger);

            Debug.Log("[Pokefrost] Before Sneasel Effect");

            StatusEffectIncreaseAttackBasedOnCardsDrawnThisTurn drawattack = ScriptableObject.CreateInstance<StatusEffectIncreaseAttackBasedOnCardsDrawnThisTurn>();
            drawattack.name = "Increase Attack Based on Cards Drawn";
            drawattack.effectToGain = Get<StatusEffectData>("Ongoing Increase Attack");
            drawattack.canBeBoosted = true;
            drawattack.applyFormat = "";
            drawattack.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            drawattack.keyword = "";
            drawattack.targetConstraints = new TargetConstraint[0];
            collection.SetString(drawattack.name + "_text", "<+{a}><keyword=attack> for each card drawn this turn");
            drawattack.textKey = collection.GetString(drawattack.name + "_text");
            drawattack.textOrder = 0;
            drawattack.textInsert = "";
            drawattack.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", drawattack);
            statusList.Add(drawattack);

            StatusEffectApplyXWhenClunkerDestroyed clunkertrash = ScriptableObject.CreateInstance<StatusEffectApplyXWhenClunkerDestroyed>();
            clunkertrash.name = "When Clunker Destroyed Add Junk To Hand";
            clunkertrash.effectToApply = Get<StatusEffectData>("Instant Summon Junk In Hand");
            clunkertrash.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            clunkertrash.waitForAnimationEnd = true;
            clunkertrash.canBeBoosted = true;
            clunkertrash.applyFormat = "";
            clunkertrash.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            clunkertrash.keyword = "";
            clunkertrash.targetConstraints = new TargetConstraint[0];
            collection.SetString(clunkertrash.name + "_text", "<keyword=trash> <{a}> when a <Clunker> is destroyed");
            clunkertrash.textKey = collection.GetString(clunkertrash.name + "_text");
            clunkertrash.textOrder = 0;
            clunkertrash.textInsert = "";
            clunkertrash.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", clunkertrash);
            statusList.Add(clunkertrash);

            StatusEffectApplyXWhenClunkerDestroyed clunkerscrap = ScriptableObject.CreateInstance<StatusEffectApplyXWhenClunkerDestroyed>();
            clunkerscrap.name = "When Clunker Destroyed Gain Scrap";
            clunkerscrap.effectToApply = Get<StatusEffectData>("Scrap");
            clunkerscrap.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            clunkerscrap.waitForAnimationEnd = true;
            clunkerscrap.canBeBoosted = true;
            clunkerscrap.applyFormat = "";
            clunkerscrap.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            clunkerscrap.keyword = "";
            clunkerscrap.targetConstraints = new TargetConstraint[0];
            collection.SetString(clunkerscrap.name + "_text", "Gain <{a}><keyword=scrap> when a <Clunker> is destroyed");
            clunkerscrap.textKey = collection.GetString(clunkerscrap.name + "_text");
            clunkerscrap.textOrder = 0;
            clunkerscrap.textInsert = "";
            clunkerscrap.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", clunkerscrap);
            statusList.Add(clunkerscrap);

            StatusEffectApplyXOnCardPlayed increasehandattack = ScriptableObject.CreateInstance<StatusEffectApplyXOnCardPlayed>();
            increasehandattack.name = "On Card Played Increase Attack Of Cards In Hand";
            increasehandattack.effectToApply = Get<StatusEffectData>("Increase Attack");
            increasehandattack.applyToFlags = StatusEffectApplyX.ApplyToFlags.Hand;
            increasehandattack.canBeBoosted = true;
            increasehandattack.applyFormat = "";
            increasehandattack.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            increasehandattack.keyword = "";
            increasehandattack.targetConstraints = new TargetConstraint[] { new TargetConstraintDoesDamage() };
            collection.SetString(increasehandattack.name + "_text", "Add +<{a}><keyword=attack> to <Cards> in your hand");
            increasehandattack.textKey = collection.GetString(increasehandattack.name + "_text");
            increasehandattack.textOrder = 0;
            increasehandattack.textInsert = "";
            increasehandattack.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", increasehandattack);
            statusList.Add(increasehandattack);

            StatusEffectApplyXWhenHit cleanseteamonhit = ScriptableObject.CreateInstance<StatusEffectApplyXWhenHit>();
            cleanseteamonhit.name = "When Hit Cleanse Team";
            cleanseteamonhit.effectToApply = Get<StatusEffectData>("Cleanse");
            cleanseteamonhit.applyToFlags = StatusEffectApplyX.ApplyToFlags.Allies | StatusEffectApplyX.ApplyToFlags.Self;
            cleanseteamonhit.waitForAnimationEnd = true;
            cleanseteamonhit.applyFormat = "";
            cleanseteamonhit.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            cleanseteamonhit.keyword = "";
            cleanseteamonhit.targetConstraints = new TargetConstraint[0];
            collection.SetString(cleanseteamonhit.name + "_text", "<keyword=cleanse> self and allies when hit");
            cleanseteamonhit.textKey = collection.GetString(cleanseteamonhit.name + "_text");
            cleanseteamonhit.textOrder = 0;
            cleanseteamonhit.textInsert = "";
            cleanseteamonhit.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", cleanseteamonhit);
            statusList.Add(cleanseteamonhit);

            StatusEffectApplyXWhenCardDestroyed triggerwhencarddestroyed = Get<StatusEffectData>("Trigger When Enemy Is Killed").InstantiateKeepName() as StatusEffectApplyXWhenCardDestroyed;
            triggerwhencarddestroyed.name = "Trigger When Card Destroyed";
            triggerwhencarddestroyed.canBeAlly = true;
            triggerwhencarddestroyed.mustBeOnBoard = false;
            collection.SetString(triggerwhencarddestroyed.name + "_text", "Trigger when a card is destroyed");
            triggerwhencarddestroyed.descColorHex = "F99C61";
            triggerwhencarddestroyed.textKey = collection.GetString(triggerwhencarddestroyed.name + "_text");
            triggerwhencarddestroyed.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", triggerwhencarddestroyed);
            statusList.Add(cleanseteamonhit);

            StatusEffectApplyXOnCardPlayed triggerclunker = Get<StatusEffectData>("On Card Played Trigger RandomAlly").InstantiateKeepName() as StatusEffectApplyXOnCardPlayed;
            triggerclunker.name = "Trigger Clunker Ahead";
            triggerclunker.applyToFlags = StatusEffectApplyX.ApplyToFlags.AllyInFrontOf;
            TargetConstraintIsCardType clunkerconstraint = new TargetConstraintIsCardType();
            clunkerconstraint.allowedTypes = new CardType[] { Get<CardType>("Clunker") };
            triggerclunker.applyConstraints = triggerclunker.applyConstraints.Append(clunkerconstraint).ToArray();
            collection.SetString(triggerclunker.name + "_text", "Trigger <Clunker> ahead");
            triggerclunker.textKey = collection.GetString(triggerclunker.name + "_text");
            triggerclunker.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", triggerclunker);
            statusList.Add(triggerclunker);

            StatusEffectApplyXOnCardPlayed reduceowncounter = Get<StatusEffectData>("On Card Played Vim To Self").InstantiateKeepName() as StatusEffectApplyXOnCardPlayed;
            reduceowncounter.name = "On Card Played Reduce Own Max Counter";
            reduceowncounter.effectToApply = Get<StatusEffectData>("Reduce Max Counter");
            collection.SetString(reduceowncounter.name + "_text", "Reduce own <keyword=counter> by <{a}>");
            reduceowncounter.textKey = collection.GetString(reduceowncounter.name + "_text");
            reduceowncounter.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", reduceowncounter);
            statusList.Add(reduceowncounter);

            StatusEffectXActsLikeShell snowActsLikeShell = ScriptableObject.CreateInstance<StatusEffectXActsLikeShell>();
            snowActsLikeShell.name = "Snow Acts Like Shell";
            snowActsLikeShell.targetType = "snow";
            snowActsLikeShell.imagePath = ImagePath("shnell.png");
            collection.SetString(snowActsLikeShell.name + "_text", "Uses <keyword=snow> as <keyword=shell>");
            snowActsLikeShell.textKey = collection.GetString(snowActsLikeShell.name + "_text");
            snowActsLikeShell.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", snowActsLikeShell);
            statusList.Add(snowActsLikeShell);

            StatusEffectInstantApplyXCardInDeck buffCardInDeck = ScriptableObject.CreateInstance<StatusEffectInstantApplyXCardInDeck>();
            buffCardInDeck.name = "Instant Buff Card In Deck";
            collection.SetString(buffCardInDeck.name + "_text", "");
            buffCardInDeck.textKey = collection.GetString(buffCardInDeck.name + "_text");
            buffCardInDeck.type = "";
            buffCardInDeck.effectToApply = Get<StatusEffectData>("Increase Attack");
            buffCardInDeck.targetConstraints = new TargetConstraint[0];
            buffCardInDeck.constraints = new TargetConstraint[1] { ScriptableObject.CreateInstance<TargetConstraintDoesDamage>() };
            buffCardInDeck.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", buffCardInDeck);
            statusList.Add(buffCardInDeck);

            StatusEffectApplyXOnKill buffCardInDeckOnKill = ScriptableObject.CreateInstance<StatusEffectApplyXOnKill>();
            buffCardInDeckOnKill.name = "Buff Card In Deck On Kill";
            collection.SetString(buffCardInDeckOnKill.name + "_text", "<i>Permamently</i> give <+{a}><keyword=attack> to a card on kill");
            buffCardInDeckOnKill.textKey = collection.GetString(buffCardInDeckOnKill.name + "_text");
            buffCardInDeckOnKill.canBeBoosted = true;
            buffCardInDeckOnKill.type = "";
            buffCardInDeckOnKill.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            buffCardInDeckOnKill.ModAdded = this;
            buffCardInDeckOnKill.effectToApply = Get<StatusEffectData>("Instant Buff Card In Deck");
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", buffCardInDeckOnKill);
            statusList.Add(buffCardInDeckOnKill);

            StatusEffectApplyXWhenYAppliedTo snowWhenSnowed = ScriptableObject.CreateInstance<StatusEffectApplyXWhenYAppliedTo>();
            snowWhenSnowed.name = "When Snowed Snow Random Enemy";
            collection.SetString(snowWhenSnowed.name + "_text", "When <keyword=snow>'d, apply equal <keyword=snow> to a random enemy");
            snowWhenSnowed.textKey = collection.GetString(snowWhenSnowed.name + "_text");
            snowWhenSnowed.type = "";
            snowWhenSnowed.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomEnemy;
            snowWhenSnowed.ModAdded = this;
            snowWhenSnowed.effectToApply = Get<StatusEffectData>("Snow");
            snowWhenSnowed.applyEqualAmount = true;
            snowWhenSnowed.queue = true;
            snowWhenSnowed.targetMustBeAlive = true;
            snowWhenSnowed.doPing = true;
            snowWhenSnowed.whenAppliedToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            snowWhenSnowed.whenAppliedType = "snow";
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", snowWhenSnowed);
            statusList.Add(snowWhenSnowed);

            StatusEffectApplyXWhenUnitIsKilled frenzyDeath = ScriptableObject.CreateInstance<StatusEffectApplyXWhenUnitIsKilled>();
            frenzyDeath.name = "Gain Frenzy When Companion Is Killed";
            collection.SetString(frenzyDeath.name + "_text", "When a <Companion> is killed, gain <x{a}><keyword=frenzy>");
            frenzyDeath.textKey = collection.GetString(frenzyDeath.name + "_text");
            frenzyDeath.canBeBoosted = true;
            frenzyDeath.type = "";
            frenzyDeath.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            frenzyDeath.ModAdded = this;
            frenzyDeath.effectToApply = Get<StatusEffectData>("MultiHit");
            TargetConstraintIsCardType frenzyDeathTarget = ScriptableObject.CreateInstance<TargetConstraintIsCardType>();
            frenzyDeathTarget.name = "Must be Friendly";
            frenzyDeathTarget.allowedTypes = new CardType[] { Get<CardType>("Friendly") };
            frenzyDeath.unitConstraints = new TargetConstraint[] { frenzyDeathTarget };
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", frenzyDeath);
            statusList.Add(frenzyDeath);

            KeywordData wonderGuardKey = ScriptableObject.CreateInstance<KeywordData>();
            wonderGuardKey.name = "WonderGuard";
            keycollection.SetString(wonderGuardKey.name + "_text", "Wonder Guard");
            wonderGuardKey.titleKey = keycollection.GetString(wonderGuardKey.name + "_text");
            keycollection.SetString(wonderGuardKey.name + "_desc", "Immune to direct damage");
            wonderGuardKey.descKey = keycollection.GetString(wonderGuardKey.name + "_desc");
            wonderGuardKey.showIcon = false;
            wonderGuardKey.showName = true;
            wonderGuardKey.ModAdded = this;
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", wonderGuardKey);

            StatusEffectImmuneToDamage wonderGuard = ScriptableObject.CreateInstance<StatusEffectImmuneToDamage>();
            wonderGuard.name = "Wonder Guard";
            collection.SetString(wonderGuard.name + "_text", "<keyword=wonderguard>");
            wonderGuard.textKey = collection.GetString(wonderGuard.name + "_text");
            wonderGuard.immuneTypes = new List<string> { "basic" };
            wonderGuard.type = "";
            wonderGuard.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", wonderGuard);
            statusList.Add(wonderGuard);

            StatusEffectSummon summonShedinja = Get<StatusEffectData>("Summon Plep").InstantiateKeepName() as StatusEffectSummon;
            summonShedinja.name = "Summon Shedinja";
            collection.SetString(summonShedinja.name + "_text", "Summon <card=websiteofsites.wildfrost.pokefrost.shedinja>");
            summonShedinja.textKey = collection.GetString(summonShedinja.name + "_text");
            summonShedinja.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", summonShedinja);
            statusList.Add(summonShedinja);

            StatusEffectSketch sketch = ScriptableObject.CreateInstance<StatusEffectSketch>();
            sketch.name = "Sketch";
            sketch.type = "";
            collection.SetString(sketch.name + "_text", "");
            sketch.textKey = collection.GetString(sketch.name + "_text");
            sketch.ModAdded = this;
            sketch.targetConstraints = new TargetConstraint[0];
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", sketch);
            statusList.Add(sketch);

            StatusEffectSketchOnDeploy sketchOnDeploy = ScriptableObject.CreateInstance<StatusEffectSketchOnDeploy>();
            sketchOnDeploy.name = "When Deployed Sketch";
            sketchOnDeploy.type = "";
            sketchOnDeploy.canBeBoosted = false;
            sketchOnDeploy.queue = true;
            sketchOnDeploy.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomEnemyInRow;
            collection.SetString(sketchOnDeploy.name + "_text", "<Sketch {a}>");
            sketchOnDeploy.textKey = collection.GetString(sketchOnDeploy.name + "_text");
            sketchOnDeploy.ModAdded = this;
            sketchOnDeploy.effectToApply = Get<StatusEffectData>("Sketch");
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", sketchOnDeploy);
            statusList.Add(sketchOnDeploy);

            Debug.Log("[Pokefrost] Before Evolves");

            StatusEffectEvolveFromKill ev1 = ScriptableObject.CreateInstance<StatusEffectEvolveFromKill>();
            ev1.Autofill("Evolve Magikarp", "<keyword=evolve>: Kill <{a}> bosses", this);
            ev1.SetEvolution("websiteofsites.wildfrost.pokefrost.gyarados");
            ev1.SetConstraints(StatusEffectEvolveFromKill.ReturnTrueIfCardTypeIsBossOrMiniboss);
            ev1.Confirm();
            statusList.Add(ev1);

            StatusEffectEvolve ev2 = ScriptableObject.CreateInstance<StatusEffectEvolveEevee>();
            ev2.Autofill("Evolve Eevee", "<keyword=evolve>: Equip charm", this);
            ev2.SetEvolution("f");
            ev2.Confirm();
            statusList.Add(ev2);

            StatusEffectEvolve ev3 = ScriptableObject.CreateInstance<StatusEffectEvolveExternalFactor>();
            ev3.Autofill("Evolve Meowth", "<keyword=evolve>: Have <{a}><keyword=blings>", this);
            ev3.SetEvolution("websiteofsites.wildfrost.pokefrost.persian");
            ev3.Confirm();
            statusList.Add(ev3);

            StatusEffectEvolveFromKill ev4 = ScriptableObject.CreateInstance<StatusEffectEvolveFromKill>();
            ev4.Autofill("Evolve Lickitung", "<keyword=evolve>: Consume <{a}> cards", this);
            ev4.SetEvolution("websiteofsites.wildfrost.pokefrost.lickilicky");
            ev4.anyKill = true;
            ev4.SetConstraints(StatusEffectEvolveFromKill.ReturnTrueIfCardWasConsumed);
            ev4.Confirm();
            statusList.Add(ev4);

            StatusEffectEvolveExternalFactor ev5 = ScriptableObject.CreateInstance<StatusEffectEvolveExternalFactor>();
            ev5.Autofill("Evolve Munchlax", "<keyword=evolve>: Have an empty deck", this);
            ev5.SetEvolution("websiteofsites.wildfrost.pokefrost.snorlax");
            ev5.SetConstraint(StatusEffectEvolveExternalFactor.ReturnTrueIfEmptyDeck);
            ev5.Confirm();
            statusList.Add(ev5);

            StatusEffectEvolveFromStatusApplied ev6 = ScriptableObject.CreateInstance<StatusEffectEvolveFromStatusApplied>();
            ev6.Autofill("Evolve Croagunk", "<keyword=evolve>: Apply <{a}> <keyword=shroom>", this);
            ev6.SetEvolution("websiteofsites.wildfrost.pokefrost.toxicroak");
            ev6.targetType = "shroom";
            ev6.faction = "ally";
            ev6.Confirm();
            statusList.Add(ev6);

            StatusEffectEvolveFromKill ev7 = ScriptableObject.CreateInstance<StatusEffectEvolveFromKill>();
            ev7.Autofill("Evolve Voltorb", "<keyword=evolve>: Kill 3 in a battle", this);
            ev7.SetEvolution("websiteofsites.wildfrost.pokefrost.electrode");
            ev7.SetConstraints(StatusEffectEvolveFromKill.ReturnTrue);
            ev7.persist = false;
            ev7.Confirm();
            statusList.Add(ev7);

            StatusEffectEvolveFromHitApplied ev8 = ScriptableObject.CreateInstance<StatusEffectEvolveFromHitApplied>();
            ev8.Autofill("Evolve Carvanha", "<keyword=evolve>: Apply <{a}> <keyword=teeth> damage", this);
            ev8.SetEvolution("websiteofsites.wildfrost.pokefrost.sharpedo");
            ev8.targetType = "spikes";
            ev8.faction = "ally";
            ev8.Confirm();
            statusList.Add(ev8);

            StatusEffectEvolveExternalFactor ev9 = ScriptableObject.CreateInstance<StatusEffectEvolveExternalFactor>();
            ev9.Autofill("Evolve Trubbish", "<keyword=evolve>: Have <5> <card=Junk> on battle end", this);
            ev9.SetEvolution("websiteofsites.wildfrost.pokefrost.garbodor");
            ev9.SetConstraint(StatusEffectEvolveExternalFactor.ReturnTrueIfEnoughJunk);
            ev9.Confirm();
            statusList.Add(ev9);

            StatusEffectEvolveFromKill ev10 = ScriptableObject.CreateInstance<StatusEffectEvolveFromKill>();
            ev10.Autofill("Evolve Litwick", "<keyword=evolve>: Kill <{a}> enemy", this);
            ev10.SetEvolution("websiteofsites.wildfrost.pokefrost.lampent");
            ev10.SetConstraints(StatusEffectEvolveFromKill.ReturnTrue);
            ev10.Confirm();
            statusList.Add(ev10);

            StatusEffectEvolveNincada ev11 = ScriptableObject.CreateInstance<StatusEffectEvolveNincada>();
            ev11.Autofill("Evolve Nincada", "<keyword=evolve>: <{a}> battles", this);
            ev11.SetEvolution("websiteofsites.wildfrost.pokefrost.ninjask");
            ev11.Confirm();
            statusList.Add(ev11);

            StatusEffectEvolveCrown ev12 = ScriptableObject.CreateInstance<StatusEffectEvolveCrown>();
            ev12.Autofill("Evolve Murkrow", "<keyword=evolve>: Wear <sprite name=crown>", this);
            ev12.SetEvolution("websiteofsites.wildfrost.pokefrost.honchkrow");
            ev12.Confirm();
            statusList.Add(ev12);

            StatusEffectShiny shiny = ScriptableObject.CreateInstance<StatusEffectShiny>();
            shiny.name = "Shiny";
            shiny.type = "shiny";
            shiny.applyFormat = "";
            shiny.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            shiny.textKey = new UnityEngine.Localization.LocalizedString();
            shiny.targetConstraints = new TargetConstraint[0];
            shiny.keyword = "";
            shiny.textOrder = 0;
            shiny.textInsert = "";
            shiny.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", shiny);
            statusList.Add(shiny);

            collection.SetString(Get<StatusEffectData>("Double Negative Effects").name + "_text", "Double the target's negative effects");
            Get<StatusEffectData>("Double Negative Effects").textKey = collection.GetString(Get<StatusEffectData>("Double Negative Effects").name + "_text");

            collection.SetString(Get<StatusEffectData>("While Active Increase Effects To Hand").name + "_text", "While active, boost effects of cards in hand by <{a}>");
            Get<StatusEffectData>("While Active Increase Effects To Hand").textKey = collection.GetString(Get<StatusEffectData>("While Active Increase Effects To Hand").name + "_text");

            collection.SetString(Get<StatusEffectData>("Redraw Cards").name + "_text", "<Redraw>");
            Get<StatusEffectData>("Redraw Cards").textKey = collection.GetString(Get<StatusEffectData>("Redraw Cards").name + "_text");

        }

        private void CreateModAssetsCards()
        {
            list = new List<CardDataBuilder>();

            Debug.Log("[Pokefrost] Loading Cards");
            //Add our cards here
            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("alolansandslash", "Alolan Sandslash", bloodProfile: "Blood Profile Snow")
                    .SetStats(6, 2, 4)
                    .SetSprites("alolansandslash.png", "alolansandslashBG.png")
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Snow"), 3), new CardData.StatusEffectStacks(Get<StatusEffectData>("Block"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("meowth", "Meowth")
                    .SetStats(4, 3, 3)
                    .SetSprites("meowth.png", "meowthBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Kill Apply Gold To Self"), 5), new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Meowth"), 300))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("persian", "Persian")
                    .SetStats(7, 0, 4)
                    .SetSprites("persian.png", "persianBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Greed"), 1))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("While Active Frenzy To Crown Allies"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("magneton", "Magneton", idleAnim: "FloatAnimationProfile", bloodProfile: "Blood Profile Black")
                    .SetStats(3, 0, 3)
                    .SetSprites("magneton.png", "magnetonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Apply Shroom Overburn Or Bom"), 3))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("voltorb", "Voltorb", idleAnim: "PulseAnimationProfile", bloodProfile: "Blood Profile Black")
                    .SetStats(4, null, 1)
                    .SetSprites("voltorb.png", "voltorbBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Give Self Explode"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Voltorb"), 3))
                    .WithValue(50)
                    .AddPool("GeneralItemPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("electrode", "Electrode", idleAnim: "PulseAnimationProfile", bloodProfile: "Blood Profile Black")
                    .SetStats(6, null, 1)
                    .SetSprites("electrode.png", "electrodeBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Give Self Explode"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("When Hit Trigger To Self"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("lickitung", "Lickitung", bloodProfile: "Blood Profile Berry")
                    .SetStats(7, 3, 3)
                    .SetSprites("lickitung.png", "lickitungBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Longshot"), 1), new CardData.TraitStacks(Get<TraitData>("Pull"), 1))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Lickitung"), 5))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("weezing", "Weezing", idleAnim: "FloatAnimationProfile", bloodProfile: "Blood Profile Husk")
                    .SetStats(8, 2, 3)
                    .SetSprites("weezing.png", "weezingBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Apply Ink to All"), 4))
                    .AddPool("ClunkUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("magikarp", "Magikarp", idleAnim: "ShakeAnimationProfile")
                    .SetStats(1, 0, 4)
                    .SetSprites("magikarp.png", "magikarpBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Magikarp"), 2))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("gyarados", "Gyarados", idleAnim: "GiantAnimationProfile")
                    .SetStats(8, 4, 4)
                    .SetSprites("gyarados.png", "gyaradosBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Fury"), 4))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Hit Your Row"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("MultiHit"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("eevee", "Eevee")
                    .SetStats(4, 3, 3)
                    .SetSprites("eevee.png", "eeveeBG.png")
                    .IsPet((ChallengeData)null, true)
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Eevee"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("vaporeon", "Vaporeon", bloodProfile: "Blood Profile Blue (x2)")
                    .SetStats(4, 3, 3)
                    .SetSprites("vaporeon.png", "vaporeonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Block"), 1))
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Null"), 2))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("jolteon", "Jolteon")
                    .SetStats(4, 2, 3)
                    .SetSprites("jolteon.png", "jolteonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("MultiHit"), 1))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Draw"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("flareon", "Flareon")
                    .SetStats(4, 1, 3)
                    .SetSprites("flareon.png", "flareonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("While Active Increase Attack To Allies"), 2))
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Overload"), 2))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("snorlax", "Snorlax", idleAnim: "SquishAnimationProfile")
                    .SetStats(14, 6, 5)
                    .SetSprites("snorlax.png", "snorlaxBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("While Active Consume To Items In Hand"), 1))
                    .WithFlavour("Its stomach can digest any kind of food, even if it happens to be a durain fruit")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("espeon", "Espeon")
                    .SetStats(3, 3, 3)
                    .SetSprites("espeon.png", "espeonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("While Active Increase Effects To Hand"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("umbreon", "Umbreon")
                    .SetStats(8, 1, 3)
                    .SetSprites("umbreon.png", "umbreonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Teeth"), 2))
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Demonize"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("murkrow", "Murkrow")
                    .SetStats(7, 4, 4)
                    .SetSprites("murkrow.png", "murkrowBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Murkrow"), 1))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Pluck"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("sneasel", "Sneasel", idleAnim:"PingAnimationProfile")
                    .SetStats(6, 0, 2)
                    .SetSprites("sneasel.png", "sneaselBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Increase Attack Based on Cards Drawn"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("When Hit Draw"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("magcargo", "Magcargo", idleAnim: "GoopAnimationProfile")
                    .SetStats(15, 0, 6)
                    .SetSprites("magcargo.png", "magcargoBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Hit Apply Spice To Allies & Enemies & Self"), 1))
                    .AddPool("BasicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("smeargle", "Smeargle")
                    .SetStats(1, 1, 4)
                    .SetSprites("smeargle.png", "smeargleBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Deployed Sketch"), 4))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Pigheaded"),1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("nincada", "Nincada")
                    .SetStats(6, 2, 5)
                    .SetSprites("nincada.png", "nincadaBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Nincada"), 1))
                    .AddPool("MagicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("ninjask", "Ninjask")
                    .SetStats(6, 2, 5)
                    .SetSprites("ninjask.png", "ninjaskBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Reduce Own Max Counter"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("shedinja", "Shedinja")
                    .WithCardType("Summoned")
                    .SetStats(1, 1, 5)
                    .SetSprites("shedinja.png", "shedinjaBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Wonder Guard"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Destroy Self After Turn"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("nosepass", "Nosepass", bloodProfile: "Blood Profile Husk")
                    .SetStats(8, 4, 4)
                    .SetSprites("nosepass.png", "nosepassBG.png")
                    .WithFlavour("My magnetic field attracts tons of charms from the sidelines")
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("sableye", "Sableye", bloodProfile: "Blood Profile Pink Wisp")
                    .SetStats(10, 0, 3)
                    .SetSprites("sableye.png", "sableyeBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Drop Bling on Hit"), 25))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Greed"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("carvanha", "Carvanha", idleAnim: "FloatAnimationProfile")
                    .SetStats(6, 3, 4)
                    .SetSprites("carvanha.png", "carvanhaBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Teeth"), 3), new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Carvanha"), 50))
                    .AddPool("MagicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("sharpedo", "Sharpedo", idleAnim: "FloatAnimationProfile")
                    .SetStats(7, 3, 4)
                    .SetSprites("sharpedo.png", "sharpedoBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Teeth"), 3), new CardData.StatusEffectStacks(Get<StatusEffectData>("Trigger When Teeth Damage"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("spinda", "Spinda", idleAnim: "Heartbeat2AnimationProfile")
                    .SetStats(5, 4, 4)
                    .SetSprites("spinda.png", "spindaBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Apply Haze to All"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Haze"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("cradily", "Cradily", idleAnim: "GoopAnimationProfile", bloodProfile: "Blood Profile Fungus")
                    .SetStats(12, null, 5)
                    .SetSprites("cradily.png", "cradilyBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Heal Self"), 6))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Frontline"), 1), new CardData.TraitStacks(Get<TraitData>("Pigheaded"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("duskull", "Duskull", idleAnim: "FloatAnimationProfile", bloodProfile: "Blood Profile Black")
                    .SetStats(8, null, 0)
                    .SetSprites("duskull.png", "duskullBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Ally Summoned Add Skull To Hand"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Trigger When Summon"), 1))
                    .AddPool("MagicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("dusclops", "Dusclops", bloodProfile: "Blood Profile Black")
                    .SetStats(10, 4, 0) 
                    .SetSprites("dusclops.png", "dusclopsBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Ally Summoned Add Skull To Hand"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Trigger When Summon"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("absol", "Absol")
                    .SetStats(5, 5, 2)
                    .SetSprites("absol.png", "absolBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Apply Demonize To RandomAlly"), 1))
                    .WithFlavour("Once mistaken to be the bringer of Wildfrost")
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("piplup", "Piplup")
                    .SetStats(4, 2, 3)
                    .SetSprites("piplup.png", "piplupBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Snow Applied To Self Gain Equal Attack"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("prinplup", "Prinplup")
                    .SetStats(6, 3, 3)
                    .SetSprites("prinplup.png", "prinplupBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Snow Acts Like Shell"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("When Snow Applied To Self Gain Equal Attack"), 1))
                ) ;

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("empoleon", "Empoleon")
                    .SetStats(8, 4, 3)
                    .SetSprites("empoleon.png", "empoleonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Snow Acts Like Shell"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("When Snow Applied To Self Gain Equal Attack"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("When Snowed Snow Random Enemy"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("bastiodon", "Bastiodon", idleAnim: "SquishAnimationProfile")
                    .SetStats(12, 4, 6)
                    .SetSprites("bastiodon.png", "bastiodonBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Taunt"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("honchkrow", "Honchkrow")
                    .SetStats(7, 4, 4)
                    .SetSprites("honchkrow.png", "honchkrowBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Pluck"), 1))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Buff Card In Deck On Kill"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("chingling", "Chingling", idleAnim: "HangAnimationProfile")
                    .SetStats(6, 3, 0)
                    .SetSprites("chingling.png", "chinglingBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Trigger When Redraw Hit"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("hippowdon", "Hippowdon", idleAnim: "SquishAnimationProfile")
                    .SetStats(12, 3, 5)
                    .SetSprites("hippowdon.png", "hippowdonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Pre Turn Weakness All Enemies"), 1))
                    .AddPool("ClunkUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("munchlax", "Munchlax")
                    .SetStats(7, 3, 5)
                    .SetSprites("munchlax.png", "munchlaxBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("While Active Consume To Items In Hand"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Munchlax"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("croagunk", "Croagunk", bloodProfile: "Blood Profile Fungus")
                    .SetStats(5, 2, 4)
                    .SetSprites("croagunk.png", "croagunkBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Hit Equal Shroom To Target"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Croagunk"), 80))
                    .AddPool("BasicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("toxicroak", "Toxicroak", bloodProfile: "Blood Profile Fungus")
                    .SetStats(7, 3, 4)
                    .SetSprites("toxicroak.png", "toxicroakBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Hit Equal Shroom To Target"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("lickilicky", "Lickilicky", idleAnim: "SquishAnimationProfile", bloodProfile: "Blood Profile Berry")
                    .SetStats(8, 3, 3)
                    .SetSprites("lickilicky.png", "lickilickyBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Barrage"), 1), new CardData.TraitStacks(Get<TraitData>("Pull"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("leafeon", "Leafeon", bloodProfile: "Blood Profile Fungus")
                    .SetStats(4, 1, 3)
                    .SetSprites("leafeon.png", "leafeonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Turn Apply Shell To AllyInFrontOf"), 2))
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Shroom"), 2))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("glaceon", "Glaceon", bloodProfile: "Blood Profile Snow")
                    .SetStats(4, 3, 3)
                    .SetSprites("glaceon.png", "glaceonBG.png")
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Snow"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Frost"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("froslass", "Froslass", idleAnim: "FloatAnimationProfile", bloodProfile: "Blood Profile Pink Wisp")
                    .SetStats(4, 1, 4)
                    .SetSprites("froslass.png", "froslassBG.png")
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Frost"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Double Negative Effects"), 1))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Aimless"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("rotom", "Rotom", idleAnim: "Heartbeat2AnimationProfile", bloodProfile: "Blood Profile Blue (x2)")
                    .SetStats(8, 3, 4)
                    .SetSprites("rotom.png", "rotomBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Trigger Clunker Ahead"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Damage To Self"), 1))
                    .IsPet((ChallengeData)null, true)
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("rotomheat", "Rotom Heat", bloodProfile: "Blood Profile Black")
                    .SetStats(5, 5, 4)
                    .SetSprites("rotomheat.png", "rotomBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Increase Attack Of Cards In Hand"), 3))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("rotomwash", "Rotom Wash", bloodProfile: "Blood Profile Black")
                    .SetStats(10, 5, 4)
                    .SetSprites("rotomwash.png", "rotomBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Hit Cleanse Team"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("rotomfrost", "Rotom Frost", bloodProfile: "Blood Profile Black")
                    .SetStats(10, 2, 4)
                    .SetSprites("rotomfrost.png", "rotomBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Hit Apply Frost To RandomEnemy"), 3))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("rotomfan", "Rotom Fan", bloodProfile: "Blood Profile Black")
                    .SetStats(6, 4, 4)
                    .SetSprites("rotomfan.png", "rotomBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Redraw Cards"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("rotommow", "Rotom Mow", idleAnim: "ShakeAnimationProfile", bloodProfile: "Blood Profile Black")
                    .SetStats(7, 3, 4)
                    .SetSprites("rotommow.png", "rotomBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Trigger When Card Destroyed"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("crustle", "Crustle", idleAnim: "GiantAnimationProfile", bloodProfile: "Blood Profile Husk")
                    .SetStats(8, 3, 4)
                    .SetSprites("crustle.png", "crustleBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Hit Add Scrap Pile To Hand"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("trubbish", "Trubbish", idleAnim: "SquishAnimationProfile", bloodProfile: "Blood Profile Husk")
                    .SetStats(6, 3, 4)
                    .SetSprites("trubbish.png", "trubbishBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Clunker Destroyed Add Junk To Hand"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Trubbish"), 5))
                    .AddPool("ClunkUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("garbodor", "Garbodor", idleAnim: "GiantAnimationProfile", bloodProfile: "Blood Profile Husk")
                    .SetStats(6, 3, 4)
                    .SetSprites("garbodor.png", "garbodorBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Clunker Destroyed Gain Scrap"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Pre Trigger Gain Frenzy Equal To Scrap"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("litwick", "Litwick", bloodProfile: "Blood Profile Black")
                    .SetStats(3, 0, 2)
                    .SetSprites("litwick.png", "litwickBG.png")
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Overload"), 1))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Litwick"), 1))
                    .AddPool("MagicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("lampent", "Lampent", idleAnim: "HangAnimationProfile", bloodProfile: "Blood Profile Black")
                    .SetStats(10, 0, 4)
                    .SetSprites("lampent.png", "lampentBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Overload Self"), 3), new CardData.StatusEffectStacks(Get<StatusEffectData>("Apply Overload Equal To Overload"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("chandelure", "Chandelure", idleAnim: "HangAnimationProfile", bloodProfile: "Blood Profile Black")
                    .SetStats(10, 0, 4)
                    .SetSprites("chandelure.png", "chandelureBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Barrage"), 1))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Overload Self"), 3), new CardData.StatusEffectStacks(Get<StatusEffectData>("Apply Overload Equal To Overload"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("volcarona", "Volcarona", idleAnim: "FlyAnimationProfile")
                    .SetStats(6, 4, 4)
                    .SetSprites("volcarona.png", "volcaronaBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Reduce Counter Row"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("tyrantrum", "Tyrantrum", idleAnim: "GiantAnimationProfile")
                    .SetStats(7, 4, 4)
                    .SetSprites("tyrantrum.png", "tyrantrumBG.png")
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Apply Wild Trait"), 1))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("MultiHit"), 1))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Aimless"), 1), new CardData.TraitStacks(Get<TraitData>("Wild"), 1))
                    .WithFlavour("")
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("sylveon", "Sylveon", bloodProfile: "Blood Profile Berry")
                    .SetStats(4, 3, 3)
                    .SetSprites("sylveon.png", "sylveonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Turn Heal & Cleanse Allies"), 3))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("goomy", "Goomy")
                    .SetStats(13, 1, 3)
                    .SetSprites("goomy.png", "goomyBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When X Health Lost Split"), 3))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("klefki", "Klefki", idleAnim: "ShakeAnimationProfile", bloodProfile: "Blood Profile Husk")
                    .SetStats(6, 2, 2)
                    .SetSprites("klefki.png", "klefkiBG.png")
                    .WithFlavour("I can hold all of your charms for safe keeping ;)")
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("salazzle", "Salazzle")
                    .SetStats(7, 1, 3)
                    .SetSprites("salazzle.png", "salazzleBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("While Active It Is Overshroom"), 1))
                    .AddPool("BasicUnitPool")
                    .AddPool("MagicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("polteageist", "Polteageist", idleAnim: "FloatAnimationProfile", bloodProfile: "Blood Profile Husk")
                    .SetStats(6, null, 5)
                    .SetSprites("polteageist.png", "polteageistBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Blaze Tea Random Ally"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("kingambit", "Kingambit")
                    .SetStats(10, 5, 5)
                    .SetSprites("kingambit.png", "kingambitBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Gain Frenzy When Companion Is Killed"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateItem("shedinjamask", "Shedinja Mask", idleAnim: "FloatAnimationProfile")
                    .SetSprites("shedinja_mask.png", "shedinja_maskBG.png")
                    .CanPlayOnBoard(true)
                    .CanPlayOnEnemy(false)
                    .FreeModify(delegate(CardData c)
                    {
                        c.playOnSlot = true;
                    })
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Consume"), 1))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Summon Shedinja"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateItem("microwave", "Microwave")
                    .SetSprites("microwave.png", "rotomBG.png")
                    .WithFlavour("Appears to be a safe without a lock. What use is that?")
                    .CanPlayOnFriendly(false)
                    .CanPlayOnEnemy(false)
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateItem("washingmachine", "Washing Machine")
                    .SetSprites("washingmachine.png", "rotomBG.png")
                    .WithFlavour("A device that spins and makes loud noises. What use is that?")
                    .CanPlayOnFriendly(false)
                    .CanPlayOnEnemy(false)
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateItem("fridge", "Fridge")
                    .SetSprites("fridge.png", "rotomBG.png")
                    .WithFlavour("This strange device seems to... keep things cold? What use is that?")
                    .CanPlayOnFriendly(false)
                    .CanPlayOnEnemy(false)
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateItem("fan", "Fan")
                    .SetSprites("fan.png", "rotomBG.png")
                    .WithFlavour("A strange saw sealed by an even stranger cage. What use is that?")
                    .CanPlayOnFriendly(false)
                    .CanPlayOnEnemy(false)
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateItem("lawnmower", "Lawnmower")
                    .SetSprites("lawnmower.png", "rotomBG.png")
                    .WithFlavour("Seems to be some sort of vehicle, but lacks seating. What use is that?")
                    .CanPlayOnFriendly(false)
                    .CanPlayOnEnemy(false)
                );

            //
        }

        private void CreateModAssetsCharms()
        {
            Debug.Log("[Pokefrost] Loading Charms");

            CardUpgradeData fish = Get<CardUpgradeData>("CardUpgradeAimless");
            CardUpgradeData pom = Get<CardUpgradeData>("CardUpgradeBarrage");
            CardUpgradeData gnome = Get<CardUpgradeData>("CardUpgradeWildcard");
            TargetConstraintHasTrait nopluck = new TargetConstraintHasTrait();
            nopluck.name = "Does Not Have Pluck";
            nopluck.trait = Get<TraitData>("Pluck");
            nopluck.not = true;
            TargetConstraintHasTrait noaimless = new TargetConstraintHasTrait();
            noaimless.name = "Does Not Have Aimless";
            noaimless.trait = Get<TraitData>("Aimless");
            noaimless.not = true;
            TargetConstraint[] crow = new TargetConstraint[] { };
            foreach(TargetConstraint tc in fish.targetConstraints) { crow = crow.Append(tc).ToArray(); };
            crow = crow.Append(noaimless).ToArray();
            fish.targetConstraints = fish.targetConstraints.Append(nopluck).ToArray();
            pom.targetConstraints = pom.targetConstraints.Append(nopluck).ToArray();
            gnome.targetConstraints = gnome.targetConstraints.Append(nopluck).ToArray();

            charmlist = new List<CardUpgradeDataBuilder>();
            //Add our cards here
            charmlist.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeMagnemite")
                    .WithTier(0)
                    .WithImage("magnemiteCharm.png")
                    .WithType(CardUpgradeData.Type.Charm)
                    .SetEffects(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Apply Shroom Overburn Or Bom"), 1))
                    .SetConstraints(Get<CardUpgradeData>("CardUpgradeShroom").targetConstraints)
                    .SetBecomesTarget(true)
                    .WithTitle("Magnemite Charm")
                    .WithText("Apply <1> <keyword=shroom>/<keyword=overload>/<keyword=weakness>")
            );

            charmlist.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradePluck")
                    .WithTier(0)
                    .WithImage("murkrowCharm.png")
                    .WithType(CardUpgradeData.Type.Charm)
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Pluck"), 1))
                    .SetConstraints(crow)
                    .ChangeDamage(1)
                    .WithTitle("Murkrow Charm")
                    .WithText("Gain <keyword=pluck>\n<+1><keyword=attack>\nCA-CAW")
            );

            charmlist.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeTaunt")
                    .WithTier(0)
                    .WithImage("shieldonCharm.png")
                    .WithType(CardUpgradeData.Type.Charm)
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Taunt"), 1))
                    .ChangeHP(3)
                    .SetConstraints(Get<CardUpgradeData>("CardUpgradeHeart").targetConstraints)
                    .WithTitle("Shieldon Charm")
                    .WithText("Gain <keyword=taunt>\n<+3><keyword=health>")
            );


            charmlist.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeTyrunt")
                    .WithTier(2)
                    .WithImage("tyruntCharm.png")
                    .WithType(CardUpgradeData.Type.Charm)
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Wild"), 1))
                    .SetAttackEffects(new CardData.StatusEffectStacks(Get<StatusEffectData>("Apply Wild Trait"), 1))
                    .SetConstraints(Get<CardUpgradeData>("CardUpgradeSpark").targetConstraints[1], Get<CardUpgradeData>("CardUpgradeSpark").targetConstraints[2])
                    .WithTitle("Tyrunt Charm")
                    .WithText("Gain <keyword=wild>\nApply <keyword=wild>\nBE <WILD>")
            );

            preLoaded = true;
        }

        private void LoadStatusEffects()
        {
            AddressableLoader.AddRangeToGroup("StatusEffectData", statusList);
        }

        private void NosepassAttach()
        {
            CardDataList bench = References.PlayerData.inventory.reserve;
            foreach (CardData cardData in bench)
            {
                if (cardData.name == "websiteofsites.wildfrost.pokefrost.nosepass")
                {
                    Debug.Log("Nosepass's magentic field attrached a charm to it");
                    Debug.Log(cardData.name);
                    List<CardUpgradeData> options = AddressableLoader.GetGroup<CardUpgradeData>("CardUpgradeData").Clone();
                    Debug.Log("Found charm list");
                    for (int i = 0; i < 30; i++)
                    {
                        var r = UnityEngine.Random.Range(0, options.Count);
                        CardUpgradeData charm = options[r].Clone();
                        if (charm.CanAssign(cardData))
                        {
                            Debug.Log("Nosepass found " + charm.name.ToString());
                            charm.Assign(cardData);
                            break;
                        }
                    }
                }
            }
        }

        private void CheckEvolve()
        {
            if (References.Battle.winner != References.Player)
            {
                return;
            }

            CardDataList list = References.Player.data.inventory.deck;
            List<CardData> slateForEvolution = new List<CardData>();
            List<StatusEffectEvolve> evolveEffects = new List<StatusEffectEvolve>();
            foreach (CardData card in list)
            {
                foreach (CardData.StatusEffectStacks s in card.startWithEffects)
                {
                    if (s.data.type == "evolve1")
                    {
                        s.count -= 1;
                        if (s.count == 0)
                        {
                            if (((StatusEffectEvolve)s.data).ReadyToEvolve(card))
                            {
                                Debug.Log("[Pokefrost] Ready for evolution!");
                                slateForEvolution.Add(card);
                                evolveEffects.Add(((StatusEffectEvolve)s.data));
                            }
                            else
                            {
                                s.count += 1;
                                Debug.Log("[Pokefrost] Conditions not met.");
                            }
                        }
                    }
                    if (s.data.type == "evolve2")
                    {
                        if (((StatusEffectEvolve)s.data).ReadyToEvolve(card))
                        {
                            Debug.Log("[Pokefrost] Ready for evolution!");
                            slateForEvolution.Add(card);
                            evolveEffects.Add(((StatusEffectEvolve)s.data));
                        }
                        else
                        {
                            Debug.Log("[Pokefrost] Conditions not met.");
                        }
                    }
                }
            }
            int count = slateForEvolution.Count;

            for (int i = 0; i < count; i++)
            {
                if (References.Player.data.inventory.deck.RemoveWhere((CardData a) => slateForEvolution[i].id == a.id))
                {
                    Debug.Log("[" + slateForEvolution[i].name + "] Removed From [" + References.Player.name + "] deck");
                    evolveEffects[i].Evolve(this, slateForEvolution[i]);
                }
            }
        }

        private void ShinyPet()
        {
            if (References.PlayerData?.inventory?.deck == null)
            {
                return;
            }
            foreach (CardData card in References.PlayerData.inventory.deck)
            {
                if (card.name.Contains("websiteofsites.wildfrost.pokefrost") && card.cardType.name == "Friendly" && UnityEngine.Random.Range(0, 1f) < shinyrate)
                {
                    string[] splitName = card.name.Split('.');
                    string trueName = splitName[3];
                    Sprite sprite = this.ImagePath("shiny_" + trueName + ".png").ToSprite();
                    sprite.name = "shiny";
                    card.mainSprite = sprite;
                    CardData.StatusEffectStacks[] shinystatus = { new CardData.StatusEffectStacks(Get<StatusEffectData>("Shiny"), 1) };
                    card.startWithEffects = card.startWithEffects.Concat(shinystatus).ToArray();
                }
            }
        }

        private void GetShiny(Entity entity)
        {
            Debug.Log("[Pokefrost] Offering " + entity.data.name.ToString());
            if (entity.data.name.Contains("websiteofsites.wildfrost.pokefrost") && entity.data.cardType.name == "Friendly" && UnityEngine.Random.Range(0, 1f) < shinyrate)
            {
                string[] splitName = entity.data.name.Split('.');
                string trueName = splitName[3];
                string fileName = this.ImagePath("shiny_" + trueName + ".png");
                Debug.Log("shiny_" + trueName);
                if (!System.IO.File.Exists(fileName))
                {
                    Debug.Log("[Pokefrost] Oops, shiny file not found. Contact devs.");
                    return;
                }
                Sprite sprite = this.ImagePath("shiny_" + trueName + ".png").ToSprite();
                sprite.name = "shiny";
                entity.data.mainSprite = sprite;
                entity.GetComponent<Card>().mainImage.sprite = sprite;
                CardData.StatusEffectStacks[] shinystatus = { new CardData.StatusEffectStacks(Get<StatusEffectData>("Shiny"), 1) };
                entity.data.startWithEffects = entity.data.startWithEffects.Concat(shinystatus).ToArray();
            }
        }

        private void DebugShiny()
        {
            foreach (CardDataBuilder card in list)
            {
                string[] splitName = card._data.name.Split('.');
                string trueName = splitName[3];
                string fileName = this.ImagePath("shiny_" + trueName + ".png");
                if (!System.IO.File.Exists(fileName))
                {
                    Debug.Log("[Pokefrost] WARNING: Shiny file for " + trueName + "does not exist.");
                }
                else
                {
                    Debug.Log("[Pokefrost] " + trueName + "has a shiny version.");
                }
            }
        }

        public override void Load()
        {
            CreateModAssets();
            CreateModAssetsCards();
            CreateModAssetsCharms();
            base.Load();
            //Events.OnSceneLoaded += PokemonEdits;
            Events.OnBattleEnd += NosepassAttach;
            Events.OnBattleEnd += CheckEvolve;
            Events.PostBattle += DisplayEvolutions;
            Events.OnEntityOffered += GetShiny;
            Events.OnEntityEnterBackpack += RotomFuse;
            Events.OnCampaignStart += ShinyPet;
            Events.OnCampaignGenerated += ApplianceSpawns;
            Events.OnCardDraw += HowManyCardsDrawn;
            Events.OnBattlePhaseStart += ResetCardsDrawn;
            Events.OnStatusIconCreated += PatchOvershroom;
            //References.instance.classes[0] = Get<ClassData>("Basic");
            //References.instance.classes[1] = Get<ClassData>("Magic");
            //References.instance.classes[2] = Get<ClassData>("Clunk");
            Get<CardData>("websiteofsites.wildfrost.pokefrost.klefki").charmSlots = 100;
            ((StatusEffectSummon)Get<StatusEffectData>("Summon Shedinja")).summonCard = Get<CardData>("websiteofsites.wildfrost.pokefrost.shedinja");

            //DebugShiny();
            //Events.OnCardDataCreated += Wildparty;

            //for (int i = 0; i < References.Classes.Length; i++)
            //{
            //    References.Classes[i].startingInventory.deck.Add(Get<CardData>("tyrantrum"));
            //    UnityEngine.Debug.Log("Added to Deck");
            //}

        }


        public override void Unload()
        {
            base.Unload();
            //Events.OnSceneLoaded -= PokemonEdits;
            Events.OnBattleEnd -= NosepassAttach;
            Events.OnBattleEnd -= CheckEvolve;
            Events.PostBattle -= DisplayEvolutions;
            Events.OnEntityOffered -= GetShiny;
            Events.OnEntityEnterBackpack -= RotomFuse;
            Events.OnCampaignStart -= ShinyPet;
            Events.OnCampaignGenerated -= ApplianceSpawns;
            Events.OnCardDraw -= HowManyCardsDrawn;
            Events.OnBattlePhaseStart -= ResetCardsDrawn;
            Events.OnStatusIconCreated -= PatchOvershroom;
            CardManager.cardIcons["overshroom"].Destroy();
            CardManager.cardIcons.Remove("overshroom");
            RemoveFromPools();
            //Events.OnCardDataCreated -= Wildparty;

        }

        private void RemoveFromPools()
        {
            string[] poolsToCheck = { "GeneralUnitPool", "BasicUnitPool", "GeneralItemPool", "MagicUnitPool", "ClunkUnitPool", "GeneralCharmPool" };
            RewardPool pool;
            foreach (string poolName in poolsToCheck)
            {
                pool = Extensions.GetRewardPool(poolName);
                if (pool == null)
                {
                    Debug.Log($"[Pokefrost] Unknown pool name: {poolName}");
                    continue;
                }
                Debug.Log($"[Pokefrost] {poolName}");
                for (int i = pool.list.Count - 1; i >= 0; i--)
                {
                    if (pool.list[i] == null || pool.list[i]?.ModAdded == this)
                    {
                        pool.list.RemoveAt(i);
                    }
                }
            }
        }

        private async Task ApplianceSpawns()
        {
            if (References.PlayerData.inventory.deck.FirstOrDefault((CardData c) => { return c.name == "websiteofsites.wildfrost.pokefrost.rotom"; }) == null)
            {
                return;
            }
            string[] order = rotomAppliances.InRandomOrder().ToArray();
            for (int i = 0; i < References.Campaign.nodes.Count; i++)
            {
                CampaignNode node = References.Campaign.nodes[i];
                if (node.tier >= 2 && node.type.name == "CampaignNodeItem")
                {
                    SaveCollection<string> collection = (SaveCollection<string>)node.data["cards"];
                    collection.Add(order[i % 5]);
                    node.data["cards"] = collection;
                }
            }
        }



        private readonly string[] rotomAppliances = { "websiteofsites.wildfrost.pokefrost.microwave", "websiteofsites.wildfrost.pokefrost.washingmachine", "websiteofsites.wildfrost.pokefrost.fridge", "websiteofsites.wildfrost.pokefrost.fan", "websiteofsites.wildfrost.pokefrost.lawnmower" };
        private readonly string[] rotomForms = { "websiteofsites.wildfrost.pokefrost.rotomheat", "websiteofsites.wildfrost.pokefrost.rotomwash", "websiteofsites.wildfrost.pokefrost.rotomfrost", "websiteofsites.wildfrost.pokefrost.rotomfan", "websiteofsites.wildfrost.pokefrost.rotommow" };
        private CardData fusedRotom;

        private void RotomFuse(Entity entity)
        {
            for (int i = 0; i < rotomAppliances.Length; i++)
            {
                if (entity.data.name == rotomAppliances[i])
                {
                    CardData rotom = References.PlayerData.inventory.deck.FirstOrDefault((CardData c) => { return c.name == "websiteofsites.wildfrost.pokefrost.rotom"; });
                    if (rotom == null)
                    {
                        rotom = References.PlayerData.inventory.reserve.FirstOrDefault((CardData c) => { return c.name == "websiteofsites.wildfrost.pokefrost.rotom"; });
                        if (rotom == null) { return; }
                        References.PlayerData.inventory.reserve.Remove(rotom);
                        References.PlayerData.inventory.deck.Add(rotom);
                    }

                    if ((bool)rotom)
                    {
                        fusedRotom = rotom;
                        CombineCardSystem combineCardSystem = GameObject.FindObjectOfType<CombineCardSystem>();
                        CombineCardSystem.Combo c = new CombineCardSystem.Combo
                        {
                            cardNames = new string[2] { "websiteofsites.wildfrost.pokefrost.rotom", rotomAppliances[i] },
                            resultingCardName = rotomForms[i]
                        };
                        Events.OnEntityEnterBackpack += RotomAdjust;
                        combineCardSystem.StopAllCoroutines();
                        combineCardSystem.StartCoroutine(combineCardSystem.CombineSequence(c));
                    }
                }
            }
        }

        private void RotomAdjust(Entity entity)
        {
            if (fusedRotom.mainSprite.name == "shiny")
            {
                string[] splitName = entity.data.name.Split('.');
                string trueName = splitName[splitName.Length - 1];
                Sprite sprite = Pokefrost.instance.ImagePath("shiny_" + trueName + ".png").ToSprite();
                sprite.name = "shiny";
                entity.data.mainSprite = sprite;
                entity.GetComponent<Card>().mainImage.sprite = sprite;
                CardData.StatusEffectStacks[] shinystatus = { new CardData.StatusEffectStacks(Get<StatusEffectData>("Shiny"), 1) };
                entity.data.startWithEffects = entity.data.startWithEffects.Concat(shinystatus).ToArray();
            }

            foreach (CardUpgradeData upgrade in fusedRotom.upgrades)
            {
                if (upgrade.CanAssign(entity.data))
                {
                    upgrade.Assign(entity.data);
                }
            }
            //Checks for renames
            CardData basePreEvo = Pokefrost.instance.Get<CardData>(fusedRotom.name);
            if (basePreEvo.title != fusedRotom.title)
            {
                entity.data.forceTitle = fusedRotom.title;
                entity.GetComponent<Card>().SetName(fusedRotom.title);
                UnityEngine.Debug.Log("[Pokefrost] renamed evolution to " + fusedRotom.title);
                Events.InvokeRename(entity, fusedRotom.title);
            }
            Events.OnEntityEnterBackpack -= RotomAdjust;
        }

        public static int cardsdrawn = 0;
        private void HowManyCardsDrawn(int arg)
        {
            if (cardsdrawn == 0)
            {
                cardsdrawn = arg;
            }

        }

        private void ResetCardsDrawn(Battle.Phase arg0)
        {
            cardsdrawn = 0;
        }

        private void PokemonEdits(Scene scene)
        {
            if (scene.name != "MapNew")
            {
                return;
            }
            Debug.Log("Fixing Shinies");
            CardDataList playerdeck = References.PlayerData.inventory.deck;
            CardDataList playerreserve = References.PlayerData.inventory.reserve;

            foreach (CardData card in playerdeck)
            {
                if (card.name.Contains("websiteofsites.wildfrost.pokefrost"))
                {
                    bool shinyflag = false;
                    foreach (CardData.StatusEffectStacks status in card.startWithEffects)
                    {
                        if (status.data.name == "Shiny")
                        {
                            shinyflag = true;
                            break;
                        }
                    }
                    if (shinyflag)
                    {
                        string[] splitName = card.name.Split('.');
                        string trueName = splitName[3];
                        string fileName = this.ImagePath("shiny_" + trueName + ".png");
                        Debug.Log("shiny_" + trueName);
                        if (!System.IO.File.Exists(fileName))
                        {
                            Debug.Log("[Pokefrost] Oops, shiny file not found. Contact devs.");
                            return;
                        }
                        Sprite sprite = this.ImagePath("shiny_" + trueName + ".png").ToSprite();
                        sprite.name = "shiny";
                        card.mainSprite = sprite;
                    }
                }
            }

            foreach (CardData card in playerreserve)
            {
                if (card.name.Contains("websiteofsites.wildfrost.pokefrost"))
                {
                    bool shinyflag = false;
                    foreach (CardData.StatusEffectStacks status in card.startWithEffects)
                    {
                        if (status.data.name == "Shiny")
                        {
                            shinyflag = true;
                            break;
                        }
                    }
                    if (shinyflag)
                    {
                        string[] splitName = card.name.Split('.');
                        string trueName = splitName[3];
                        string fileName = this.ImagePath("shiny_" + trueName + ".png");
                        Debug.Log("shiny_" + trueName);
                        if (!System.IO.File.Exists(fileName))
                        {
                            Debug.Log("[Pokefrost] Oops, shiny file not found. Contact devs.");
                            return;
                        }
                        Sprite sprite = this.ImagePath("shiny_" + trueName + ".png").ToSprite();
                        sprite.name = "shiny";
                        card.mainSprite = sprite;
                    }
                }
            }
        }

        private void DisplayEvolutions(CampaignNode whatever)
        {
            if (StatusEffectEvolve.evolvedPokemonLastBattle.Count > 0)
            {
                References.instance.StartCoroutine(StatusEffectEvolve.EvolutionPopUp(this));
            }
        }

        private void PatchOvershroom(StatusIcon icon)
        {
            if (icon.type == "overshroom")
            {
                icon.SetText();
                icon.Ping();
                icon.onValueDown.AddListener(delegate { icon.Ping(); });
                icon.onValueUp.AddListener(delegate { icon.Ping(); });
                icon.afterUpdate.AddListener(icon.SetText);
                icon.onValueDown.AddListener(icon.CheckDestroy);
            }
        }

        public override string GUID => "websiteofsites.wildfrost.pokefrost";
        public override string[] Depends => new string[] { };
        public override string Title => "Pokefrost";
        public override string Description => "Pokemon Companions\r\n\r\n Adds 30 new companions, 2 new pets, and 3 new charms.";

        public override List<T> AddAssets<T, Y>()
        {
            var typeName = typeof(Y).Name;
            switch (typeName)
            {
                case "CardData": return list.Cast<T>().ToList();
                case "CardUpgradeData": return charmlist.Cast<T>().ToList();
            }

            return base.AddAssets<T, Y>();
        }
    }

    [HarmonyPatch(typeof(InspectSystem), "GetClass", new Type[]
        {
            typeof(CardData),
        })]
    internal static class FixTribeFlags
    {
        internal static bool Prefix(ref ClassData __result, CardData cardData)
        {
            string cardName = cardData.name;
            if (cardName.Contains("websiteofsites.wildfrost.pokefrost"))
            {
                foreach(string cardName2 in Pokefrost.basicPool)
                {
                    if(cardName.Contains(cardName2))
                    {
                        __result = References.Classes[0];
                        return false;
                    }
                }
                foreach (string cardName2 in Pokefrost.magicPool)
                {
                    if (cardName.Contains(cardName2))
                    {
                        __result = References.Classes[1];
                        return false;
                    }
                }
                foreach (string cardName2 in Pokefrost.clunkPool)
                {
                    if (cardName.Contains(cardName2))
                    {
                        __result = References.Classes[2];
                        return false;
                    }
                }
                return false;
            }
            return true;
        }
    }

}

using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Localization.Tables;
using UnityEngine;
using HarmonyLib;
using AssortedPatchesCollection;
using Rewired;
using UnityEngine.Localization;
using System.IO;

namespace Pokefrost
{
    public class Pokefrost : WildfrostMod
    {

        private List<CardDataBuilder> list;
        private List<CardUpgradeDataBuilder> charmlist;
        private List<StatusEffectData> statusList = new List<StatusEffectData>(30);
        private bool preLoaded = false;
        private static float shinyrate = 1/400f;

        public Pokefrost(string modDirectory) : base(modDirectory) 
        {
            
        }

        private void CreateModAssets()
        {
            StringTable keycollection = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
            KeywordData evolvekey = Get<KeywordData>("explode").InstantiateKeepName();
            evolvekey.name = "Evolve";
            keycollection.SetString(evolvekey.name + "_text", "Evolve");
            evolvekey.titleKey = keycollection.GetString(evolvekey.name + "_text");
            keycollection.SetString(evolvekey.name + "_desc", "If the condition is met at the end of battle evolve into a new Pokemon");
            evolvekey.descKey = keycollection.GetString(evolvekey.name + "_desc");
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
            keycollection.SetString(tauntedkey.name + "_desc", "Target only enemies with <keyword=taunt>");
            tauntedkey.descKey = keycollection.GetString(tauntedkey.name + "_desc");
            tauntedkey.ModAdded = this;
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", tauntedkey);

            StatusEffectFreeTrait wilder = ScriptableObject.CreateInstance<StatusEffectFreeTrait>();
            wilder.trait = this.Get<TraitData>("Wild");
            wilder.silenced = null;
            wilder.added = null;
            wilder.addedAmount = 0;
            wilder.targetConstraints = new TargetConstraint[0];
            wilder.offensive = true;
            wilder.isKeyword = false;
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
            supergreed.hiddenKeywords = new KeywordData[] { Get<KeywordData>("Hit") };
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
            collection.SetString(duskulltrigger.name + "_text", "<color=#F99C61>Trigger when anything is summoned</color>");
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
            triattack.hiddenKeywords = new KeywordData[] { Get<KeywordData>("shroom"), Get<KeywordData>("overload"), Get<KeywordData>("weakness") };
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
            collection.SetString(overburnself.name + "_text", "Apply <{a}> <keyword=overload> to self");
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
            collection.SetString(overoverburn.name + "_text", "Apply <keyword=overload> equal to your <keyword=overload>");
            overoverburn.textKey = collection.GetString(overoverburn.name + "_text");
            overoverburn.textOrder = 0;
            overoverburn.textInsert = "";
            overoverburn.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", overoverburn);
            statusList.Add(overoverburn);


            StatusEffectChangeTargetMode taunteffect = Get<StatusEffectData>("Hit All Enemies").InstantiateKeepName() as StatusEffectChangeTargetMode;
            taunteffect.name = "Hit All Taunt";
            //collection.SetString(taunteffect.name + "_text", "");
            taunteffect.textKey = new UnityEngine.Localization.LocalizedString();
            taunteffect.ModAdded = this;

            TargetModeAll taunttargetmode = taunteffect.targetMode.InstantiateKeepName() as TargetModeAll;

            StatusEffectWhileActiveX hittaunt = Get<StatusEffectData>("While Active Aimless To Enemies").InstantiateKeepName() as StatusEffectWhileActiveX;
            hittaunt.name = "Target Mode Taunt";
            hittaunt.keyword = "";
            //collection.SetString(hittaunt.name + "_text", "");
            hittaunt.textKey = new UnityEngine.Localization.LocalizedString();
            hittaunt.ModAdded = this;

            TraitData taunttrait = Get<TraitData>("Hellbent").InstantiateKeepName();
            taunttrait.name = "Taunt";
            taunttrait.keyword = tauntkey;
            StatusEffectData[] taunttemp = {hittaunt};
            taunttrait.effects = taunttemp;
            taunttrait.ModAdded = this;

            TraitData tauntedtrait = Get<TraitData>("Hellbent").InstantiateKeepName();
            tauntedtrait.name = "Taunted";
            tauntedtrait.keyword = tauntedkey;
            StatusEffectData[] tauntedtemp = {taunteffect};
            tauntedtrait.effects = tauntedtemp;
            TraitData[] tempoverrides = { Get<TraitData>("Aimless"), Get<TraitData>("Barrage"), Get<TraitData>("Longshot") };
            tauntedtrait.overrides = tempoverrides;
            tauntedtrait.ModAdded = this;

            TargetConstraintHasTrait tauntconstraint = ScriptableObject.CreateInstance<TargetConstraintHasTrait>();
            tauntconstraint.name = "Has Taunt Trait";
            tauntconstraint.trait = taunttrait;
            TargetConstraint[] temptaunteffect = {tauntconstraint};
            taunttargetmode.constraints = temptaunteffect;
            taunteffect.targetMode = taunttargetmode;

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
            collection.SetString(bomall.name + "_text", "Apply <{a}> <keyword=weakness> to all enemies");
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
            collection.SetString(teethtrigger.name + "_text", "<color=#F99C61>Trigger when <keyword=teeth> damage is dealt</color>");
            teethtrigger.textKey = collection.GetString(teethtrigger.name + "_text");
            teethtrigger.textOrder = 0;
            teethtrigger.textInsert = "";
            teethtrigger.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", teethtrigger);
            statusList.Add(teethtrigger);

            StatusEffectEvolveFromKill ev1 = ScriptableObject.CreateInstance<StatusEffectEvolveFromKill>();
            ev1.Autofill("Evolve Magikarp", "<keyword=evolve>: Kill <{a}> bosses", this);
            ev1.SetEvolution("websiteofsites.wildfrost.pokefrost.gyarados");
            ev1.SetConstraints(StatusEffectEvolveFromKill.ReturnTrueIfCardTypeIsBossOrMiniboss);
            ev1.Confirm();
            statusList.Add(ev1);

            StatusEffectEvolve ev2 = ScriptableObject.CreateInstance<StatusEffectEvolveEevee>();
            ev2.Autofill("Evolve Eevee", "<keyword=evolve>: Equip charm" , this);
            ev2.SetEvolution("f");
            ev2.Confirm();
            statusList.Add(ev2);

            StatusEffectEvolve ev3 = ScriptableObject.CreateInstance<StatusEffectEvolveFromMoney>();
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

            StatusEffectEvolveFromMoney ev5 = ScriptableObject.CreateInstance<StatusEffectEvolveFromMoney>();
            ev5.Autofill("Evolve Munchlax", "<keyword=evolve>: Have an empty deck", this);
            ev5.SetEvolution("websiteofsites.wildfrost.pokefrost.snorlax");
            ev5.SetConstraint(StatusEffectEvolveFromMoney.ReturnTrueIfEmptyDeck);
            ev5.Confirm();
            statusList.Add(ev5);

            StatusEffectEvolveFromStatusApplied ev6 = ScriptableObject.CreateInstance<StatusEffectEvolveFromStatusApplied>();
            ev6.Autofill("Evolve Croagunk", "<keyword=evolve>: Apply <{a}> <keyword=shroom>",this);
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

            collection.SetString(Get<StatusEffectData>("Double Negative Effects").name + "_text", "Double the target's negative effects");
            Get<StatusEffectData>("Double Negative Effects").textKey = collection.GetString(Get<StatusEffectData>("Double Negative Effects").name + "_text");

            collection.SetString(Get<StatusEffectData>("While Active Increase Effects To Hand").name + "_text", "While active, boost effects of cards in hand by <{a}>");
            Get<StatusEffectData>("While Active Increase Effects To Hand").textKey = collection.GetString(Get<StatusEffectData>("While Active Increase Effects To Hand").name + "_text");

            list = new List<CardDataBuilder>();
            //Add our cards here
            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("alolansandslash", "Alolan Sandslash")
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
                    .CreateUnit("magneton", "Magneton")
                    .SetStats(3, 0, 3)
                    .SetSprites("magneton.png", "magnetonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Apply Shroom Overburn Or Bom"), 3))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("voltorb", "Voltorb")
                    .SetStats(4, null, 1)
                    .SetSprites("voltorb.png", "voltorbBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Give Self Explode"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Voltorb"), 3))
                    .WithValue(50)
                    .AddPool("GeneralItemPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("electrode", "Electrode")
                    .SetStats(6, null, 1)
                    .SetSprites("electrode.png", "electrodeBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Give Self Explode"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("When Hit Trigger To Self"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("lickitung", "Lickitung")
                    .SetStats(7, 3, 3)
                    .SetSprites("lickitung.png", "lickitungBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Longshot"), 1), new CardData.TraitStacks(Get<TraitData>("Pull"), 1))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Lickitung"), 5))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("magikarp", "Magikarp")
                    .SetStats(1, 0, 4)
                    .SetSprites("magikarp.png", "magikarpBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Magikarp"), 2))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("gyarados", "Gyarados")
                    .SetStats(8, 4, 4)
                    .SetSprites("gyarados.png", "gyaradosBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Fury"), 4))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Hit Your Row"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("MultiHit"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("eevee", "Eevee")
                    .SetStats(3, 3, 3)
                    .SetSprites("eevee.png", "eeveeBG.png")
                    .IsPet((ChallengeData) null, true)
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Eevee"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("vaporeon", "Vaporeon")
                    .SetStats(3, 3, 3)
                    .SetSprites("vaporeon.png", "vaporeonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Block"), 1))
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Null"), 2))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("jolteon", "Jolteon")
                    .SetStats(3, 2, 3)
                    .SetSprites("jolteon.png", "jolteonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("MultiHit"), 1))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Draw"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("flareon", "Flareon")
                    .SetStats(3, 1, 3)
                    .SetSprites("flareon.png", "flareonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("While Active Increase Attack To Allies"), 2))
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Overload"), 2))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("snorlax", "Snorlax")
                    .SetStats(14, 6, 5)
                    .SetSprites("snorlax.png", "snorlaxBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("While Active Consume To Items In Hand"), 1))
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
                    .CreateUnit("magcargo", "Magcargo")
                    .SetStats(15, 0, 6)
                    .SetSprites("magcargo.png", "magcargoBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Hit Apply Spice To Allies & Enemies & Self"), 1))
                    .AddPool("BasicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("nosepass", "Nosepass")
                    .SetStats(8, 4, 4)
                    .SetSprites("nosepass.png", "nosepassBG.png")
                    .WithFlavour("My magnetic field attracts tons of charms from the sidelines")
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("sableye", "Sableye")
                    .SetStats(10, 0, 3)
                    .SetSprites("sableye.png", "sableyeBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Drop Bling on Hit"), 25))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Greed"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("carvanha", "Carvanha")
                    .SetStats(6, 3, 4)
                    .SetSprites("carvanha.png", "carvanhaBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Teeth"), 3), new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Carvanha"), 50))
                    .AddPool("MagicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("sharpedo", "Sharpedo")
                    .SetStats(7, 3, 4)
                    .SetSprites("sharpedo.png", "sharpedoBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Teeth"), 3), new CardData.StatusEffectStacks(Get<StatusEffectData>("Trigger When Teeth Damage"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("spinda", "Spinda")
                    .SetStats(5, 4, 4)
                    .SetSprites("spinda.png", "spindaBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Apply Haze to All"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Haze"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("cradily", "Cradily")
                    .SetStats(12, null, 5)
                    .SetSprites("cradily.png", "cradilyBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Heal Self"), 6))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Frontline"), 1), new CardData.TraitStacks(Get<TraitData>("Pigheaded"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("duskull", "Duskull")
                    .SetStats(8, 3, 0)
                    .SetSprites("duskull.png", "duskullBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Ally Summoned Add Skull To Hand"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Trigger When Summon"), 1))
                    .AddPool("MagicUnitPool")
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
                    .CreateUnit("bastiodon", "Bastiodon")
                    .SetStats(12, 4, 6)
                    .SetSprites("bastiodon.png", "bastiodonBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Taunt"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("chingling", "Chingling")
                    .SetStats(6, 3, 0)
                    .SetSprites("chingling.png", "chinglingBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Trigger When Redraw Hit"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("hippowdon", "Hippowdon")
                    .SetStats(8, 3, 5)
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
                    .CreateUnit("croagunk", "Croagunk")
                    .SetStats(5, 2, 4)
                    .SetSprites("croagunk.png", "croagunkBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Hit Equal Shroom To Target"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Evolve Croagunk"), 80))
                    .AddPool("BasicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("toxicroak", "Toxicroak")
                    .SetStats(7, 3, 4)
                    .SetSprites("toxicroak.png", "toxicroakBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Hit Equal Shroom To Target"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("lickilicky", "Lickilicky")
                    .SetStats(8, 3, 3)
                    .SetSprites("lickilicky.png", "lickilickyBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Barrage"), 1), new CardData.TraitStacks(Get<TraitData>("Pull"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("leafeon", "Leafeon")
                    .SetStats(3, 1, 3)
                    .SetSprites("leafeon.png", "leafeonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Turn Apply Shell To AllyInFrontOf"), 2))
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Shroom"), 2))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("glaceon", "Glaceon")
                    .SetStats(3, 3, 3)
                    .SetSprites("glaceon.png", "glaceonBG.png")
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Snow"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Frost"), 1))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("froslass", "Froslass")
                    .SetStats(4, 1, 4)
                    .SetSprites("froslass.png", "froslassBG.png")
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Frost"), 1), new CardData.StatusEffectStacks(Get<StatusEffectData>("Double Negative Effects"), 1))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Aimless"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("crustle", "Crustle")
                    .SetStats(8, 3, 4)
                    .SetSprites("crustle.png", "crustleBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("When Hit Add Scrap Pile To Hand"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("chandelure", "Chandelure")
                    .SetStats(10, 0, 4)
                    .SetSprites("chandelure.png", "chandelureBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Barrage"), 1))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Overload Self"), 3), new CardData.StatusEffectStacks(Get<StatusEffectData>("Apply Overload Equal To Overload"), 1))
                    .AddPool("MagicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("volcarona", "Volcarona")
                    .SetStats(6, 4, 4)
                    .SetSprites("volcarona.png", "volcaronaBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Reduce Counter Row"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("tyrantrum", "Tyrantrum")
                    .SetStats(7, 4, 4)
                    .SetSprites("tyrantrum.png", "tyrantrumBG.png")
                    .SetAttackEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("Apply Wild Trait"), 1))
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("MultiHit"), 1))
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Aimless"), 1), new CardData.TraitStacks(Get<TraitData>("Wild"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("sylveon", "Sylveon")
                    .SetStats(3, 3, 3)
                    .SetSprites("sylveon.png", "sylveonBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Turn Heal & Cleanse Allies"), 3))
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("klefki", "Klefki")
                    .SetStats(6, 2, 2)
                    .SetSprites("klefki.png", "klefkiBG.png")
                    .WithFlavour("I can hold all of your charms for safe keeping ;)")
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("polteageist", "Polteageist")
                    .SetStats(6, null, 5)
                    .SetSprites("polteageist.png", "polteageistBG.png")
                    .SetStartWithEffect(new CardData.StatusEffectStacks(Get<StatusEffectData>("On Card Played Blaze Tea Random Ally"), 1))
                    .AddPool()
                );
            //

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
            /*
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
                    .WithText("Gain <keyword=taunt>\n<+{a}> <keyword=health>")
            );
            
            
            charmlist.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeTyrunt")
                    .WithTier(2)
                    .WithImage("tyruntCharm.png")
                    .WithType(CardUpgradeData.Type.Charm)
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Wild"), 1))
                    .SetAttackEffects(new CardData.StatusEffectStacks(Get<StatusEffectData>("Apply Wild Trait"), 1))
                    .SetConstraints(Get<CardUpgradeData>("CardUpgradeSpark").targetConstraints[1])
                    .WithTitle("Tyrunt Charm")
                    .WithText("Gain <keyword=wild>\nApply <keyword=wild>")
            );*/

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
                if(cardData.name == "websiteofsites.wildfrost.pokefrost.nosepass")
                {
                    Debug.Log("Nosepass's magentic field attrached a charm to it");
                    Debug.Log(cardData.name);
                    List<CardUpgradeData> options = AddressableLoader.GetGroup<CardUpgradeData>("CardUpgradeData").Clone();
                    Debug.Log("Found charm list");
                    for(int i = 0; i < 30;i++)
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
            foreach(CardData card in References.PlayerData.inventory.deck)
            {
                if (card.name.Contains("websiteofsites.wildfrost.pokefrost") && card.cardType.name == "Friendly" && UnityEngine.Random.Range(0, 1f) < shinyrate)
                {
                    string[] splitName = card.name.Split('.');
                    string trueName = splitName[3];
                    Sprite sprite = this.ImagePath("shiny_" + trueName + ".png").ToSprite();
                    sprite.name = "shiny";
                    card.mainSprite = sprite;
                }
            }
        }

        private void GetShiny(Entity entity)
        {
            Debug.Log("[Pokefrost] Offering "+entity.data.name.ToString());
            if (entity.data.name.Contains("websiteofsites.wildfrost.pokefrost") && entity.data.cardType.name == "Friendly" && UnityEngine.Random.Range(0,1f) < shinyrate)
            {
                string[] splitName = entity.data.name.Split('.');
                string trueName = splitName[3];
                if(!System.IO.File.Exists("shiny_" + trueName + ".png"))
                {
                    Debug.Log("[Pokefrost] Oops, shiny file not found. Contact devs.");
                    return;
                }
                Sprite sprite = this.ImagePath("shiny_" + trueName + ".png").ToSprite();
                sprite.name = "shiny";
                entity.data.mainSprite = sprite;
                entity.GetComponent<Card>().mainImage.sprite = sprite;
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
            base.Load();
            Events.OnCardDataCreated += PokemonEdits;
            Events.OnBattleEnd += NosepassAttach;
            Events.OnBattleEnd += CheckEvolve;
            Events.PostBattle += DisplayEvolutions;
            Events.OnEntityOffered += GetShiny;
            Events.OnCampaignStart += ShinyPet;
            References.instance.classes[0] = Get<ClassData>("Basic");
            References.instance.classes[1] = Get<ClassData>("Magic");
            References.instance.classes[2] = Get<ClassData>("Clunk");
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
            Events.OnCardDataCreated -= PokemonEdits;
            Events.OnBattleEnd -= NosepassAttach;
            Events.OnBattleEnd -= CheckEvolve;
            Events.PostBattle -= DisplayEvolutions;
            Events.OnEntityOffered -= GetShiny;
            Events.OnCampaignStart -= ShinyPet;
            //Events.OnCardDataCreated -= Wildparty;

        }

        private void PokemonEdits(CardData card)
        {
            if (card.name == "websiteofsites.wildfrost.pokefrost.klefki") 
            {
                card.charmSlots = 100;
                Debug.Log("Klefkei is :D");
            }
        }

        private void DisplayEvolutions(CampaignNode whatever)
        {
            if (StatusEffectEvolve.evolvedPokemonLastBattle.Count > 0)
            {
                References.instance.StartCoroutine(StatusEffectEvolve.EvolutionPopUp(this));
            }
        }

        public override string GUID => "websiteofsites.wildfrost.pokefrost";
        public override string[] Depends => new string[] { };
        public override string Title => "Pokefrost";
        public override string Description => "Pokemon Companions\r\n\r\n Adds 27 new companions and one new pet.";

        public override List<T> AddAssets<T, Y>()
        {
            var typeName = typeof(Y).Name;
            switch (typeName)
            {
                case "CardData":return list.Cast<T>().ToList();
                case "CardUpgradeData":return charmlist.Cast<T>().ToList();
            }

            return base.AddAssets<T, Y>();
        }
    }

}

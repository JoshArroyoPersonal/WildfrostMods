using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using Random_Junk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tutorial2
{
    public class Tutorial2 : WildfrostMod
    {
        public Tutorial2(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "websiteofsites.wildfrost.contest";

        public override string[] Depends => new string[0];

        public override string Title => "The Unit Contest";

        public override string Description => "Eggie";

        private bool preLoaded = false;

        public static List<object> assets = new List<object>();

        private T TryGet<T>(string name) where T : DataFile
        {
            T data;
            if (typeof(StatusEffectData).IsAssignableFrom(typeof(T)))
                data = base.Get<StatusEffectData>(name) as T;
            else
                data = base.Get<T>(name);

            if (data == null)
                throw new Exception($"TryGet Error: Could not find a [{typeof(T).Name}] with the name [{name}] or [{Extensions.PrefixGUID(name, this)}]");

            return data;
        }

        private StatusEffectDataBuilder StatusCopy(string oldName, string newName)
        {
            StatusEffectData data = TryGet<StatusEffectData>(oldName).InstantiateKeepName();
            data.name = GUID + "." + newName;
            StatusEffectDataBuilder builder = data.Edit<StatusEffectData, StatusEffectDataBuilder>();
            builder.Mod = this;
            return builder;
        }

        private CardData.StatusEffectStacks SStack(string name, int amount) => new CardData.StatusEffectStacks(TryGet<StatusEffectData>(name), amount);

        private void CreateModAssets()
        {

            assets.Add(
                StatusCopy("Summon Fallow", "Summon Eggie")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectSummon)data).summonCard = TryGet<CardData>("eggie");
                })
                );

            assets.Add(
                StatusCopy("Instant Summon Copy", "Instant Summon Copy In Hand")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectInstantSummon)data).summonPosition = StatusEffectInstantSummon.Position.Hand;
                })
                );

            assets.Add(
                StatusCopy("When Deployed Add Junk To Hand", "When Deployed Add Eggie To Hand")
                .WithText("When deployed, summon a copy in your hand")
                .WithTextInsert("<card=websiteofsites.wildfrost.contest.eggie>")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectApplyXWhenDeployed)data).effectToApply = TryGet<StatusEffectData>("Instant Summon Copy In Hand");
                })
                );

            assets.Add(
                new CardDataBuilder(this).CreateUnit("eggie", "Eggie")
                .SetSprites("Eggie.png", "Eggie BG.png")
                .SetStats(1, null, 0)
                .WithCardType("Friendly")
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.startWithEffects = new CardData.StatusEffectStacks[1]
                    {
                        SStack("When Deployed Add Eggie To Hand",1)
                    };
                })
                );

            assets.Add(
                new StatusEffectDataBuilder(this).Create<StatusEffectInstantCombineCard>("Instant Combine Card Test")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectInstantCombineCard)data).cardNames = new string[] { "GuardianGnome", "SunburstDart"};
                    ((StatusEffectInstantCombineCard)data).resultingCardName = "SplitBoss2";
                    ((StatusEffectInstantCombineCard)data).checkDeck = false;
                    ((StatusEffectInstantCombineCard)data).checkBoard = false;
                    ((StatusEffectInstantCombineCard)data).changeDeck = true;
                    ((StatusEffectInstantCombineCard)data).keepUpgrades = true;
                })
                );

            assets.Add(
                new StatusEffectDataBuilder(this).Create<StatusEffectApplyXWhenHit>("When Hit Combine Card Test")
                .WithText("Test Combine")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectApplyXWhenHit)data).effectToApply = TryGet<StatusEffectData>("Instant Combine Card Test");
                    ((StatusEffectApplyXWhenHit)data).applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                    ((StatusEffectApplyXWhenHit)data).targetMustBeAlive = false;
                    data.eventPriority = 999999999;
                    

                })
                );







            preLoaded = true;
        }
        public override void Load()
        {
            if (!preLoaded) { CreateModAssets(); }
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();
            UnloadFromClasses();
        }

        public override List<T> AddAssets<T, Y>()
        {
            if (assets.OfType<T>().Any())
                Debug.LogWarning($"[{Title}] adding {typeof(Y).Name}s: {assets.OfType<T>().Count()}");
            return assets.OfType<T>().ToList();
        }

        public void UnloadFromClasses()
        {
            List<ClassData> tribes = AddressableLoader.GetGroup<ClassData>("ClassData");
            foreach (ClassData tribe in tribes)
            {
                if (tribe == null || tribe.rewardPools == null) { continue; }

                foreach (RewardPool pool in tribe.rewardPools)
                {
                    if (pool == null) { continue; };

                    pool.list.RemoveAllWhere((item) => item == null || item.ModAdded == this);
                }
            }
        }
    }



    //Status Effect Class
    public class StatusEffectTriggerWhenCertainAllyAttacks : StatusEffectTriggerWhenAllyAttacks
    {
        //Cannot change allyInRow or againstTarget without some publicizing. Shade Snake is sad :(

        public CardData ally;

        public override bool RunHitEvent(Hit hit)
        {
            //Debug.Log(hit.attacker?.name);
            if (hit.attacker?.name == ally.name)
            {
                return base.RunHitEvent(hit);
            }
            return false;
        }
    }
}
using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace Robo
{
    internal class Robo : WildfrostMod
    {
        public static WildfrostMod instance;
        public Robo(string modDirectory) : base(modDirectory)
        {
            instance = this;
        }

        public static string[] basicPool = new string[] { "glacia" }; // Put card names here (with GUID prefixes) to get the class flags to show up
        public static string[] magicPool = new string[] { "janvun" };
        public static string[] clunkPool = new string[] { "ingot" };

        private List<CardDataBuilder> list;
        private List<CardUpgradeDataBuilder> charmlist; //Includes all upgrades, so also crowns
        private List<StatusEffectData> statusList;

        private CardData.StatusEffectStacks SStack(string name, int count) => new CardData.StatusEffectStacks(Get<StatusEffectData>(name), count);

        private void CreateModAssets()
        {
            statusList = new List<StatusEffectData>();
            StringTable collection = LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
            StringTable keycollection = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);

            StatusEffectApplyXOnHit quaddemon = ScriptableObject.CreateInstance<StatusEffectApplyXOnHit>();
            quaddemon.name = "On Hit Quad Damage Demonized Target";
            quaddemon.addDamageFactor = 0;
            quaddemon.multiplyDamageFactor = 4f;
            TargetConstraintHasStatus demonconstraint = new TargetConstraintHasStatus();
            demonconstraint.status = Get<StatusEffectData>("Demonize");
            quaddemon.applyConstraints = new TargetConstraint[] { demonconstraint };
            quaddemon.applyToFlags = StatusEffectApplyX.ApplyToFlags.Target;
            quaddemon.canBeBoosted = false;
            quaddemon.type = "";
            collection.SetString(quaddemon.name + "_text", "Deals quadruple damage to <keyword=demonize>'d targets");
            quaddemon.textKey = collection.GetString(quaddemon.name + "_text");
            quaddemon.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", quaddemon);
            statusList.Add(quaddemon);

            StatusEffectApplyXWhenHit junkscrap = ScriptableObject.CreateInstance<StatusEffectApplyXWhenHit>();
            junkscrap.name = "When Hit With Junk Give Random Ally Scrap";
            TargetConstraintHasStatus scrapconstraint = new TargetConstraintHasStatus();
            scrapconstraint.status = Get<StatusEffectData>("Scrap");
            junkscrap.applyConstraints = new TargetConstraint[] { scrapconstraint };
            TargetConstraintIsSpecificCard junkconstraint = new TargetConstraintIsSpecificCard();
            junkconstraint.allowedCards = new CardData[] { Get<CardData>("Junk") };
            junkscrap.attackerConstraints = new TargetConstraint[] { junkconstraint };
            junkscrap.effectToApply = Get<StatusEffectData>("Scrap");
            junkscrap.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomAlly;
            junkscrap.canBeBoosted = true;
            junkscrap.type = "";
            collection.SetString(junkscrap.name + "_text", "When hit with <card=Junk>, apply <{a}> <keyword=scrap> to a random ally");
            junkscrap.textKey = collection.GetString(junkscrap.name + "_text");
            junkscrap.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", junkscrap);
            statusList.Add(junkscrap);

            StatusEffectApplyXWhenAnyoneTakesDamage spiceshroom = ScriptableObject.CreateInstance<StatusEffectApplyXWhenAnyoneTakesDamage>();
            spiceshroom.name = "Gain Spice When Someone Takes Shroom Damage";
            spiceshroom.targetDamageType = "shroom";
            spiceshroom.effectToApply = Get<StatusEffectData>("Spice");
            spiceshroom.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            spiceshroom.type = "";
            spiceshroom.canBeBoosted = true;
            collection.SetString(spiceshroom.name + "_text", "Gain <{a}> <keyword=spice> when <keyword=shroom> damage is dealt");
            spiceshroom.textKey = collection.GetString(spiceshroom.name + "_text");
            spiceshroom.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", spiceshroom);
            statusList.Add(spiceshroom);


        }

        private void CreateModAssetsCards()
        {
            list = new List<CardDataBuilder>();

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("yangler", "Yangler")
                    .SetStats(9, 4, 4)
                    .SetSprites("yangler.png", "yanglerBG.png")
                    .SetTraits(new CardData.TraitStacks(Get<TraitData>("Longshot"), 1), new CardData.TraitStacks(Get<TraitData>("Pull"), 1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("joatoo", "Joatoo")
                    .SetStats(4, 2, 3)
                    .SetSprites("joatoo.png", "joatooBG.png")
                    .SetStartWithEffect(SStack("On Hit Quad Damage Demonized Target", 1), SStack("MultiHit",1))
                    .AddPool()
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("glacia", "Glacia")
                    .SetStats(7, 1, 5)
                    .SetSprites("glacia.png", "glaciaBG.png")
                    .SetStartWithEffect(SStack("When Hit Increase Attack Effects To Self", 1), SStack("ImmuneToSnow", 1))
                    .SetAttackEffect(SStack("Snow", 1))
                    .AddPool("BasicUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("ingot", "Ingot")
                    .SetStats(6, 1, 3)
                    .SetSprites("ingot.png", "ingotBG.png")
                    .SetStartWithEffect(SStack("Weakness", 1), SStack("When Hit With Junk Give Random Ally Scrap", 1))
                    .AddPool("ClunkUnitPool")
                );

            list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("janvun", "Jan Vun")
                    .SetStats(4, null, 4)
                    .SetSprites("janvun.png", "janvunBG.png")
                    .SetStartWithEffect(SStack("Summon Plep", 1))
                    .AddPool("MagicUnitPool")
                );

            /*list.Add(
                new CardDataBuilder(this)
                    .CreateUnit("redcap", "Redcap")
                    .SetStats(6, 0, 3)
                    .SetSprites("redcap.png", "redcapBG.png")
                    .SetStartWithEffect(SStack("Gain Spice When Someone Takes Shroom Damage", 1))
                    .SetAttackEffect(SStack("Shroom", 2))
                    .AddPool("BasicUnitPool")
                );*/
        }

        private void CreateModAssetsCharms()
        {
            charmlist = new List<CardUpgradeDataBuilder>();
        }

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

        public override void Load()
        {
            CreateModAssets();
            CreateModAssetsCards();
            CreateModAssetsCharms();
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();

        }

        public override string GUID => "websiteofsites.wildfrost.robo";
        public override string[] Depends => new string[] { };
        public override string Title => "Mercs";
        public override string Description => "Adds new companions. Art & design done by Robo.";

        [HarmonyPatch(typeof(InspectSystem), "GetClass", new Type[]
        {
            typeof(CardData),
        })]
        internal static class FixTribeFlags
        {
            internal static bool Prefix(ref ClassData __result, CardData cardData)
            {
                string cardName = cardData.name;
                if (cardName.Contains("websiteofsites.wildfrost.robo"))
                {
                    foreach (string cardName2 in Robo.basicPool)
                    {
                        if (cardName.Contains(cardName2))
                        {
                            __result = References.Classes[0];
                            return false;
                        }
                    }
                    foreach (string cardName2 in Robo.magicPool)
                    {
                        if (cardName.Contains(cardName2))
                        {
                            __result = References.Classes[1];
                            return false;
                        }
                    }
                    foreach (string cardName2 in Robo.clunkPool)
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
}

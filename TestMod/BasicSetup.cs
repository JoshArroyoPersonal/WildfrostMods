using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Localization.Tables;
using UnityEngine;

namespace TestMod
{
    internal class BasicSetup : WildfrostMod
    {
        public static WildfrostMod instance;
        public BasicSetup(string modDirectory) : base(modDirectory)
        {
            instance = this;
        }

        public static string[] basicPool = new string[] { }; // Put card names here (with GUID prefixes) to get the class flags to show up
        public static string[] magicPool = new string[] { };
        public static string[] clunkPool = new string[] { };

        private List<CardDataBuilder> list;
        private List<CardUpgradeDataBuilder> charmlist; //Includes all upgrades, so also crowns
        private List<StatusEffectData> statusList;

        private CardData.StatusEffectStacks SStack(string name, int count) => new CardData.StatusEffectStacks(Get<StatusEffectData>(name), count);

        private void CreateModAssets()
        {
            statusList = new List<StatusEffectData>();
            StringTable collection = LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
            StringTable keycollection = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
        }

        private void CreateModAssetsCards()
        {
            list = new List<CardDataBuilder>();
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

        public override string GUID => "websiteofsites.wildfrost.MODNAME";
        public override string[] Depends => new string[] { };
        public override string Title => "MODNAME";
        public override string Description => "Words";

        [HarmonyPatch(typeof(InspectSystem), "GetClass", new Type[]
        {
            typeof(CardData),
        })]
        internal static class FixTribeFlags
        {
            internal static bool Prefix(ref ClassData __result, CardData cardData)
            {
                string cardName = cardData.name;
                if (cardName.Contains("GUID HERE"))
                {
                    foreach (string cardName2 in BasicSetup.basicPool)
                    {
                        if (cardName.Contains(cardName2))
                        {
                            __result = References.Classes[0];
                            return false;
                        }
                    }
                    foreach (string cardName2 in BasicSetup.magicPool)
                    {
                        if (cardName.Contains(cardName2))
                        {
                            __result = References.Classes[1];
                            return false;
                        }
                    }
                    foreach (string cardName2 in BasicSetup.clunkPool)
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
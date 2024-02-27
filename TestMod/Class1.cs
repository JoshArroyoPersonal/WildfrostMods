using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace TestMod
{
    public class TestMod : WildfrostMod
    {

        private List<CardUpgradeDataBuilder> charmlist;

        public TestMod(string modDirectory) : base(modDirectory)
        {

        }

        private void CreateModAssets()
        {
            StringTable collection = LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);

            StatusEffectBecomeOvershroom giveovershroom = ScriptableObject.CreateInstance<StatusEffectBecomeOvershroom>();
            giveovershroom.name = "Turn Overload to Weakness";
            giveovershroom.applyFormat = "";
            giveovershroom.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            giveovershroom.keyword = "";
            giveovershroom.targetConstraints = new TargetConstraint[0];
            collection.SetString(giveovershroom.name + "_text", "<keyword=overload> becomes <keyword=weakness>");
            giveovershroom.textKey = collection.GetString(giveovershroom.name + "_text");
            giveovershroom.textOrder = 0;
            giveovershroom.textInsert = "";
            giveovershroom.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", giveovershroom);
        }

        private void BigBooshu(CardData cardData)
        {
            UnityEngine.Debug.Log("[Tutorial1] New CardData Created: " + cardData.name);
            if (cardData.name == "BerryPet")
            {
                cardData.startWithEffects = CardData.StatusEffectStacks.Stack(cardData.startWithEffects, new CardData.StatusEffectStacks[1]
            {
                new CardData.StatusEffectStacks( Get<StatusEffectData>("Turn Overload to Weakness"), 1)
            });
                UnityEngine.Debug.Log("[Tutorial1] Booshu!");
            }
        }

        public override void Load()
        {
            CreateModAssets();
            Events.OnCardDataCreated += BigBooshu;
            base.Load();

        }
        public override void Unload()
        {
            Events.OnCardDataCreated -= BigBooshu;
            base.Unload();

        }

        public override List<T> AddAssets<T, Y>()
        {
            //var typeName = typeof(Y).Name;
            //switch (typeName)
            //{
            //    case "CardUpgradeData": return charmlist.Cast<T>().ToList();
            //}

            return base.AddAssets<T, Y>();
        }

        public override string GUID => "websiteofsites.wildfrost.testmod";
        public override string[] Depends => new string[] { };
        public override string Title => "Charm Test";
        public override string Description => "Testing 2 charms";
    }

    public class StatusEffectBecomeOvershroom : StatusEffectData
    {
        public override bool HasApplyStatusRoutine => true;

        public override IEnumerator ApplyStatusRoutine(StatusEffectApply apply)
        {
            Debug.Log("[Test] ApplyStatusRoutine");
            if(apply != null && apply?.effectData.name == "Overload")
            {
                Debug.Log("[Test] found overload");
                apply.effectData = AddressableLoader.Get<StatusEffectData>("StatusEffectData","Weakness");
            }

            return null;
        }
    }
}

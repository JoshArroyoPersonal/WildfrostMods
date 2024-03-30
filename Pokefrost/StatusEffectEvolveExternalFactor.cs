using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectEvolveExternalFactor : StatusEffectEvolve
    {

        public static Dictionary<string, string> upgradeMap = new Dictionary<string, string>();
        public int threshold;

        public Action<int, CardData> constraint = ReturnTrueIfMoneyIsAboveThreshold;
        public static bool result = false;

        public static void ReturnTrueIfMoneyIsAboveThreshold(int t, CardData target)
        {
            result = (References.Player.data.inventory.gold >= t);
        }

        public static void ReturnTrueIfEmptyDeck(int t, CardData target)
        {
            result = (References.Player.drawContainer.Count + References.Player.handContainer.Count + References.Player.discardContainer.Count == 0);
        }

        public static void ReturnTrueIfEnoughJunk(int t, CardData target)
        {
            int junk = 0;
            foreach(Entity card in References.Player.drawContainer)
            {
                if(card.name == "Junk")
                {
                    junk += 1;
                }
            }
            foreach (Entity card in References.Player.handContainer)
            {
                if (card.name == "Junk")
                {
                    junk += 1;
                }
            }
            foreach (Entity card in References.Player.discardContainer)
            {
                if (card.name == "Junk")
                {
                    junk += 1;
                }
            }
            result = (junk >= t);
        }

        public static void ReturnTrueIfEnoughOverload(int t, CardData target)
        {
            foreach(Entity entity in Battle.GetAllUnits())
            {
                Debug.Log("[Pokefrost] Found Lampent!");
                if (entity.data.id == target.id && entity.FindStatus("overload").count >= t)
                {
                    result = true;
                    return;
                }
            }
            result = false;
        }

        public void SetConstraint(Action<int, CardData> c)
        {
            constraint = c;
        }

        public override void Init()
        {
            base.Init();
            foreach(CardData.StatusEffectStacks statuses in target.data.startWithEffects)
            {
                if (statuses.data.name == this.name)
                {
                    threshold = ((StatusEffectEvolveExternalFactor)statuses.data).threshold;
                    return;
                }
            }
        }

        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);

            type = "evolve2";
            UnityEngine.Localization.Tables.StringTable collection = LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
            collection.SetString(name + "_text", descrip);
            textKey = collection.GetString(name + "_text");
        }

        public override bool ReadyToEvolve(CardData cardData)
        {
            foreach (CardData.StatusEffectStacks statuses in cardData.startWithEffects)
            {
                if (statuses.data.name == this.name)
                {
                    constraint(statuses.count, target.data);
                    return result;
                }
            }
            return false;
        }
    }
}

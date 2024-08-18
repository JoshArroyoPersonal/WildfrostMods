using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectEvolveKirlia : StatusEffectEvolve
    {
        public bool persist;
        public string faction = "self";
        public static string[] evolutions = { "gardevoir", "gallade" };
        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);

            evolutionCardName = "gardevoir";
            type = "evolve2";
            UnityEngine.Localization.Tables.StringTable collection = LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
            collection.SetString(name + "_text", descrip);
            textKey = collection.GetString(name + "_text");
        }

        public void SetEvolutions(params string[] cardNames)
        {
            evolutionCardName = cardNames[0];
            evolutions = cardNames;
        }

        public override bool RunPostApplyStatusEvent(StatusEffectApply apply)
        {
            base.RunPostApplyStatusEvent(apply);
            //UnityEngine.Debug.Log("[Pokefrost] Post Hit Event");
            bool result2 = false;
            bool result3 = !apply.effectData?.type.IsNullOrEmpty() ?? false;
            switch (faction)
            {
                case "ally":
                    result2 = (apply?.target?.owner == target?.owner);
                    break;
                case "self":
                    result2 = (apply?.target == target);
                    break;
            }
            //UnityEngine.Debug.Log("[Pokefrost] " + result1.ToString() + " " + result2.ToString() + " " + result3.ToString());
            //UnityEngine.Debug.Log(hit.damageType);
            if (result2 && result3 && CheckUniqueEffect(apply.effectData))
            {
                string newType = apply.effectData.type.Replace(", ", ",a");
                UnityEngine.Debug.Log("[Pokefrost] Unique Effect!");
                foreach (StatusEffectData statuses in target.statusEffects)
                {
                    if (statuses.name == this.name && this.count > 0)
                    {
                        AddUniqueEffect(target.data, newType);
                        this.count -= 1;
                        target.display.promptUpdateDescription = true;
                        target.PromptUpdate();
                        //UnityEngine.Debug.Log("[Pokefrost] Updated card on board!");
                    }
                }
                if (this.count != 0)
                {
                    return false;
                }
                FindDeckCopy((card,status) => { AddUniqueEffect(card, newType); status.count = count; });
            }
            return false;
        }

        public bool CheckUniqueEffect(StatusEffectData effectData)
        {
            target.data.TryGetCustomData<string>("Effects Applied", out string val, "");
            string currentType = effectData.type.Replace(", ", ",a");
            string[] usedTypes = val.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            return !usedTypes.Contains(currentType);
        }

        public void AddUniqueEffect(CardData data, string newType)
        {
            data.TryGetCustomData<string>("Effects Applied", out string value, "");
            data.SetCustomData("Effects Applied", value + ", " + newType.Replace(", ", ",a"));
        }

        public override bool ReadyToEvolve(CardData cardData)
        {
            foreach (CardData.StatusEffectStacks statuses in cardData.startWithEffects)
            {
                if (statuses.data.name == this.name)
                {
                    return (statuses.count == 0);
                }
            }
            return false;
        }

        public override void Evolve(WildfrostMod mod, CardData preEvo)
        {
            evolutionCardName = evolutions.RandomItem();
            base.Evolve(mod, preEvo);
        }

        public override CardData[] EvolveForFinalBoss(WildfrostMod mod)
        {
            evolutionCardName = evolutions.RandomItem();
            return base.EvolveForFinalBoss(mod);
        }
    }
}

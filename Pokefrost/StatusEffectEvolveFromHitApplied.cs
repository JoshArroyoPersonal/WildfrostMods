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
    internal class StatusEffectEvolveFromHitApplied : StatusEffectEvolve
    {
        public Func<Hit, bool> constraint = ReturnTrue;
        public string faction;
        public string targetType; //shroom
        public bool persist = true;

        public override bool HasPostHitRoutine => true;

        public override void Init()
        {
            base.Init();
            foreach (CardData.StatusEffectStacks statuses in target.data.startWithEffects)
            {
                if (statuses.data.name == this.name)
                {
                    constraint = ((StatusEffectEvolveFromHitApplied)statuses.data).constraint;
                    return;
                }
            }
        }

        public static bool ReturnTrue(Hit hit)
        {
            return true;
        }

        public virtual void SetConstraints(Func<Hit, bool> f)
        {
            constraint = f;
        }

        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);

            type = "evolve2";
            UnityEngine.Localization.Tables.StringTable collection = LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
            collection.SetString(name + "_text", descrip);
            textKey = collection.GetString(name + "_text");
        }

        public override bool RunPostHitEvent(Hit hit)
        {
            base.RunPostHitEvent(hit);
            //UnityEngine.Debug.Log("[Pokefrost] Post Hit Event");
            bool result1 = constraint(hit);
            bool result2 = false;
            bool result3 = (hit.damageType == targetType);
            if (faction == "ally")
            {
                result2 = (hit?.attacker?.owner == target?.owner);
            }
            //UnityEngine.Debug.Log("[Pokefrost] " + result1.ToString() + " " + result2.ToString() + " " + result3.ToString());
            //UnityEngine.Debug.Log(hit.damageType);
            if (result1 && result2 && result3)
            {
                UnityEngine.Debug.Log("[Pokefrost] Confrimed Hit!");
                foreach (StatusEffectData statuses in target.statusEffects)
                {
                    if (statuses.name == this.name && this.count > 0)
                    {
                        this.count -= Math.Min(this.count, hit.damage);
                        target.display.promptUpdateDescription = true;
                        target.PromptUpdate();
                        //UnityEngine.Debug.Log("[Pokefrost] Updated card on board!");
                    }
                }
                if (!persist && this.count != 0)
                {
                    return false;
                }
                FindDeckCopy();

            }
            return false;
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
    }
}

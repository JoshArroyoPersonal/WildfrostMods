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
    internal class StatusEffectEvolveFromStatusApplied : StatusEffectEvolve
    {

        public static Dictionary<string, string> upgradeMap = new Dictionary<string, string>();
        public Func<StatusEffectEvolveFromStatusApplied, StatusEffectApply, bool> constraint = ReturnTrue;
        public string faction;
        public string targetType; //shroom
        public bool persist = true;

        public bool threshold = false;
        public bool once = false;

        public override void Init()
        {
            base.Init();
            foreach (CardData.StatusEffectStacks statuses in target.data.startWithEffects)
            {
                if (statuses.data.name == this.name)
                {
                    constraint = ((StatusEffectEvolveFromStatusApplied)statuses.data).constraint;
                    return;
                }
            }
        }

        public static bool ReturnTrue(StatusEffectEvolveFromStatusApplied instance, StatusEffectApply apply)
        {
            return true;
        }

        public static bool ReturnTrueIfEnoughAffected(StatusEffectEvolveFromStatusApplied instance, StatusEffectApply apply)
        {
            if (instance.count == 0) { return false; }

            int count = 0;
            List<Entity> entities = Battle.GetCardsOnBoard();
            foreach (Entity entity in entities)
            {
                if (entity.FindStatus(instance.targetType) != null)
                {
                    count++;
                }
            }

            return (count == instance.count);
        }

        public virtual void SetConstraints(Func<StatusEffectEvolveFromStatusApplied, StatusEffectApply, bool> f)
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

        public override bool RunPostApplyStatusEvent(StatusEffectApply apply)
        {
            if (apply?.effectData?.type != targetType) { return false; }

            bool result1 = constraint(this,apply);
            bool result2 = false;
            switch(faction)
            {
                case "ally":
                    result2 = (apply?.applier?.owner == target?.owner);
                    break;
                case "toSelf":
                    result2 = (apply?.target == target);
                    break;
                case "all":
                    result2 = true;
                    break;
            }
            if (result1 && result2)
            {
                UnityEngine.Debug.Log("[Pokefrost] Confirmed Status!");
                foreach (StatusEffectData statuses in target.statusEffects)
                {
                    if (statuses.name == this.name && this.count > 0)
                    {
                        if (threshold)
                        {
                            if(target.FindStatus("overload")?.count >= count)
                            {
                                this.count = 0;
                            }
                        }
                        else if (once)
                        {
                            this.count = 0;
                        }
                        else
                        {
                            this.count -= Math.Min(this.count, apply.count);
                        }
                        target.display.promptUpdateDescription = true;
                        target.PromptUpdate();
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

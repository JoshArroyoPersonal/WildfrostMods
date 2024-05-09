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
    internal class StatusEffectEvolveFromKill : StatusEffectEvolve
    {

        public Action<Entity, DeathType> constraint = ReturnTrue;
        public static bool result = false;
        public bool anyKill = false;
        public bool persist = true;

        public override void Init()
        {
            base.Init();
            foreach (CardData.StatusEffectStacks statuses in target.data.startWithEffects)
            {
                if (statuses.data.name == this.name)
                {
                    //typeConditions = ((StatusEffectEvolveFromKill)statuses.data).typeConditions;
                    constraint = ((StatusEffectEvolveFromKill)statuses.data).constraint;
                    return;
                }
            }
        }

        public static void ReturnTrue(Entity entity, DeathType deathType)
        {
            StatusEffectEvolveFromKill.result = true;
            return;
        }

        public static void ReturnTrueIfCardTypeIsBossOrMiniboss(Entity entity, DeathType deathType)
        {
            switch (entity.data.cardType.name)
            {
                case "Boss":
                case "Miniboss":
                case "BossSmall":
                    StatusEffectEvolveFromKill.result = true;
                    return;
            }
            StatusEffectEvolveFromKill.result = false;
        }

        public static void ReturnTrueIfCardWasConsumed(Entity entity, DeathType deathType)
        {
            if (deathType == DeathType.Consume)
            {
                StatusEffectEvolveFromKill.result = true;
                return;
            }
            else
            {
                StatusEffectEvolveFromKill.result = false;
            }
        }

        public virtual void SetConstraints(Action<Entity, DeathType> c)
        {
            constraint = c;
        }

        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);

            type = "evolve2";
            UnityEngine.Localization.Tables.StringTable collection = LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
            collection.SetString(name + "_text", descrip);
            textKey = collection.GetString(name + "_text");
        }

        public override bool RunEntityDestroyedEvent(Entity entity, DeathType deathType)
        {
            constraint(entity, deathType);
            //if (entity.lastHit != null && entity.lastHit.attacker == target && typeConditions.Contains<string>(entity.data.cardType.name))
            bool deserving = anyKill || (entity.lastHit != null && entity.lastHit.attacker == target);
            if (deserving && result)
            {
                foreach (StatusEffectData statuses in target.statusEffects)
                {
                    if (statuses.name == this.name && this.count > 0)
                    {
                        this.count--;
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

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

        public Func<Entity, DeathType, bool> constraint = ReturnTrue;
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

        public static bool ReturnTrue(Entity entity, DeathType deathType)
        {
            return true;
        }

        public static bool ReturnTrueIfCardTypeIsBossOrMiniboss(Entity entity, DeathType deathType)
        {
            switch (entity.data.cardType.name)
            {
                case "Boss":
                case "Miniboss":
                case "BossSmall":
                    return true;
            }
            return false;
        }

        public static bool ReturnTrueIfCardWasConsumed(Entity entity, DeathType deathType)
        {
            if (deathType == DeathType.Consume)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void SetConstraints(Func<Entity, DeathType, bool> c)
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
            //if (entity.lastHit != null && entity.lastHit.attacker == target && typeConditions.Contains<string>(entity.data.cardType.name))
            bool deserving = anyKill || (entity.lastHit != null && entity.lastHit.attacker == target);
            if (deserving && constraint(entity, deathType))
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

    internal class StatusEffectEvolveCubone : StatusEffectEvolveFromKill
    {
        public string normalEvolution;
        public string sacEvolution;
        public override void Init()
        {
            base.Init();
        }

        public void SetEvolutions(string normal, string sac)
        {
            evolutionCardName = normal;
            normalEvolution = normal;
            sacEvolution = sac;
        }

        public override bool RunBeginEvent()
        {
            anyKill = true;
            constraint = (entity, death) => { return entity == target; };
            return false;
        }

        public override bool RunEntityDestroyedEvent(Entity entity, DeathType deathType)
        {
            if (count > 0)
            {
                base.RunEntityDestroyedEvent(entity, deathType);
                if (count == 0)
                {
                    if (entity?.lastHit?.attacker?.owner == target.owner || deathType == DeathType.Sacrifice)
                    {
                        FindDeckCopy((card, status) => { status.count = 2; });
                    }
                }
            }
            return false;
        }

        public override bool ReadyToEvolve(CardData cardData)
        {
            foreach (CardData.StatusEffectStacks stack in cardData.startWithEffects)
            {
                if (stack.data.name == name)
                {
                    if (stack.count == 0 || stack.count == 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Evolve(WildfrostMod mod, CardData preEvo)
        {
            foreach (CardData.StatusEffectStacks stack in preEvo.startWithEffects)
            {
                if (stack.data.name == name)
                {
                    if (stack.count == 2)
                    {
                        evolutionCardName = sacEvolution;
                        base.Evolve(mod, preEvo);
                        return;
                    }
                }
            }
            evolutionCardName = normalEvolution;
            References.PlayerData.inventory.upgrades.Add(mod.Get<CardUpgradeData>("CardUpgradeThickClub").Clone());
            base.Evolve(mod, preEvo);
        }
    }
}

using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;

namespace Pokefrost
{
    internal class StatusEffectEvolvePlayCards : StatusEffectEvolve
    {
        public string[] cardNames;
        public string[] displayedNames;
        public Func<Entity, Entity[], bool> cardConstraint;
        public string textInsertTemplate = "";


        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);
            type = "evolve2";
        }

        public void SetCardNames(params string[] names)
        {
            cardNames = names;
        }
        public void SetDispalyedNames(params string[] names)
        {
            displayedNames = names;
        }

        public void SetTextTemplate(string text)
        {
            textInsertTemplate = text;
            textInsert = string.Format(textInsertTemplate, displayedNames);
        }

        public void SetCardConstraint(Func<Entity, Entity[], bool> constraint)
        {
            this.cardConstraint = constraint;
        }

        public static bool ReturnTrueIfTrait(string name, Entity entity)
        {
            foreach(Entity.TraitStacks t in entity.traits)
            {
                if (t.data.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Init()
        {
            base.Init();
            foreach (CardData.StatusEffectStacks statuses in target.data.startWithEffects)
            {
                if (statuses.data.name == this.name)
                {
                    cardConstraint = ((StatusEffectEvolvePlayCards)statuses.data).cardConstraint;
                    return;
                }
            }
            UpdateTextInsert();
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            if (cardConstraint != null && cardConstraint(entity,targets))
            {
                return GeneralCardPlayed(entity, targets);
            }
            if (cardNames != null && cardNames.Length > 0)
            {
                return SpecificCardPlayed(entity, targets);
            }
            return false;
        }

        public virtual bool SpecificCardPlayed(Entity entity, Entity[] targets)
        {
            for (int i = 0; i < cardNames.Length; i++)
            {
                if (entity.name == cardNames[i])
                {
                    UnityEngine.Debug.Log("[Pokefrost] Played corresponding card");
                    int amount = (int)Math.Round(Math.Pow(2, i));
                    if ((count / amount) % 2 == 1)
                    {
                        count -= amount;
                        UpdateTextInsert();
                        FindDeckCopy();
                    }
                }
            }
            return false;
        }

        public virtual bool GeneralCardPlayed(Entity entity, Entity[] targets)
        {
            if(count > 0)
            {
                count -= 1;
                target.display.promptUpdateDescription = true;
                target.PromptUpdate();
                FindDeckCopy();
            }
            return false;
        }

        private void UpdateTextInsert()
        {
            string[] trueDisplayedNames = displayedNames.Clone() as string[];
            for(int i=0; i<trueDisplayedNames.Length; i++)
            {
                int amount = (int)Math.Round(Math.Pow(2, i));
                if ((count/amount)%2 == 0)
                {
                    trueDisplayedNames[i] = "<s>" + trueDisplayedNames[i] + "</s>";   
                }
            }
            textInsert = string.Format(textInsertTemplate, trueDisplayedNames);
            target.display.promptUpdateDescription = true;
            target.PromptUpdate();
        }

        public override bool ReadyToEvolve(CardData cardData)
        {
            if (cardConstraint != null)
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
            foreach (CardData.StatusEffectStacks statuses in cardData.startWithEffects)
            {
                if (statuses.data.name == this.name)
                {
                    int amount = (int) Math.Round(Math.Pow(2, cardNames.Length+1));
                    return (statuses.count % amount == 0);
                }
            }
            return false;
        }
    }
}

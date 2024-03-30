using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectEvolvePlayCards : StatusEffectEvolve
    {
        public string[] cardNames;
        public string[] displayedNames;
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

        public override void Init()
        {
            base.Init();
            UpdateTextInsert();
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            for(int i=0; i<cardNames.Length; i++)
            {
                if (entity.name == cardNames[i])
                {
                    UnityEngine.Debug.Log("[Pokefrost] Played corresponding card");
                    int amount = (int)Math.Round(Math.Pow(2, i));
                    if ( (count/amount)%2 == 1)
                    {
                        count -= amount;
                        UpdateTextInsert();
                        foreach (CardData card in References.Player.data.inventory.deck)
                        {
                            if (card.id == target.data.id)
                            {
                                foreach (CardData.StatusEffectStacks statuses in card.startWithEffects)
                                {
                                    if (statuses.data.name == this.name && statuses.count > 0)
                                    {
                                        statuses.count = this.count;
                                        UnityEngine.Debug.Log("[Pokefrost] Updated deck copy!");
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
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

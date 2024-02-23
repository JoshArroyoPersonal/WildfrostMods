using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RandomTextBoxes
{
    public class RandomTextBoxes : WildfrostMod
    {
        public RandomTextBoxes(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "websiteofsites.wildfrost.randomtextboxes";
        public override string[] Depends => new string[] { };
        public override string Title => "Random Text Boxes";
        public override string Description => "All card text boxes are changed to a random different text box.";

        public TargetMode[] targetmodes = { };
        public string[][] allcardnames = { };
        protected override void Load()
        {
            base.Load();

            string[] categories = {"Miniboss", "Enemy", "Clunker", "Item", "Friendly" };
            for (int i = 0; i < categories.Length; i++)
            {
                UnityEngine.Debug.Log("Here");
                CardData[] categoryCardData = Extensions.GetCategoryCardData(categories[i]);
                foreach (CardData cardData in categoryCardData) 
                {
                    if (targetmodes.Contains<TargetMode>(cardData.targetMode) == false)
                    {
                        targetmodes = targetmodes.Append(cardData.targetMode).ToArray();

                        allcardnames = allcardnames.Append(new string[0]).ToArray();

                        allcardnames[allcardnames.Length-1] = allcardnames[allcardnames.Length - 1].Append(cardData.name).ToArray();
                        continue;
                    }

                    else
                    {
                        for (int j = 0; j < targetmodes.Length; j++)
                        {
                            if (targetmodes[j] == cardData.targetMode)
                            {
                                if (allcardnames[j].Contains(cardData.name) == false)
                                {
                                    allcardnames[j] = allcardnames[j].Append(cardData.name).ToArray();
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            Events.OnCardDataCreated += RandomizeTextBoxes;
        }

        protected override void Unload()
        {
            base.Unload();
            Events.OnCardDataCreated -= RandomizeTextBoxes;
        }

        private void RandomizeTextBoxes(CardData cardData)
        {

            for (int j = 0; j < targetmodes.Length; j++)
            {
                if(cardData.targetMode == targetmodes[j])
                {
                    var r = Dead.Random.Range(0, allcardnames[j].Length - 1);
                    cardData.attackEffects = Get<CardData>(allcardnames[j][r]).attackEffects;
                    cardData.startWithEffects = Get<CardData>(allcardnames[j][r]).startWithEffects;
                }
            }
        }

    }

}
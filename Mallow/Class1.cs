using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mallow
{
    public class OopsAllMallow : WildfrostMod
    {
        public OopsAllMallow(string modDirectory) : base(modDirectory) 
        { 
        }

        public override string GUID => "websiteofsites.wildfrost.oopsallmarrow";
        public override string[] Depends => new string[] { };
        public override string Title => "Oops All Marrow";
        public override string Description => "All enemies are replaced with Marrow";

        protected override void Load()
        {
            base.Load();
            Events.OnCardDataCreated += MakeMarrow;
            Events.OnEntityEnabled += NameMarrow;
        }

        protected override void Unload()
        {
            base.Unload();
            Events.OnCardDataCreated -= MakeMarrow;
            Events.OnEntityEnabled -= NameMarrow;
        }

        private void MakeMarrow(CardData cardData)
        {
            switch (cardData.cardType.name)
            {
                case "Enemy":
                case "Miniboss":
                case "Boss":
                case "BossSmall":


                    CardData.StatusEffectStacks[] marroweffect = { new CardData.StatusEffectStacks(Get<StatusEffectData>("While Active Teeth To Allies"), 2) };

                    cardData.startWithEffects = cardData.startWithEffects.Concat(marroweffect).ToArray();

                    //cardData.startWithEffects = cardData.startWithEffects.Concat(Get<CardData>("Spyke").startWithEffects).ToArray();

                    cardData.mainSprite = this.ImagePath("Bonesby.png").ToSprite();

                    switch (cardData.name)
                    {
                        case "SplitBoss":
                            cardData.mainSprite = this.ImagePath("Marroozle.png").ToSprite();
                            break;
                        case "SplitBoss1":
                            cardData.mainSprite = this.ImagePath("Mar.png").ToSprite();
                            break;
                        case "SplitBoss2":
                            cardData.mainSprite = this.ImagePath("Roozle.png").ToSprite();
                            break;
                        case "FrenzyBoss":
                            cardData.mainSprite = this.ImagePath("Infermarroko1.png").ToSprite();
                            break;
                        case "FrenzyBoss2":
                            cardData.mainSprite = this.ImagePath("Infermarroko2.png").ToSprite();
                            break;
                    }


                    break;
            }
        }

        private void NameMarrow(Entity entity)
        {
            UnityEngine.Debug.Log(entity.name);
            UnityEngine.Debug.Log(entity.owner.name);
            if (entity.owner.name == "Enemy")
            {
                Card card = entity.gameObject.GetComponent<Card>();
                switch (card.name)
                {
                    case "SplitBoss":
                        card.SetName("Marroozle");
                        break;
                    case "SplitBoss1":
                        card.SetName("Mar");
                        break;
                    case "SplitBoss2":
                        card.SetName("Roozle");
                        break;
                    case "FrenzyBoss":
                        card.SetName("Infermarroko");
                        break;
                    case "FrenzyBoss2":
                        card.SetName("Infermarroko");
                        break;

                    default:
                        float r = Dead.Random.Range(1, 100);
                        
                        if(r <= 70)
                        {
                            card.SetName(entity.data.title + ": Marrow Mode");
                        }
                        else if(r <= 90)
                        {
                            card.SetName(entity.data.title + ": Bonesby Mode");
                        }
                        else if (r <= 99)
                        {
                            card.SetName(entity.data.title + ": Spyke Mode");
                        }
                        else
                        {
                            card.SetName(entity.data.title + ": Mallow Mode");
                        }
                    break;
                }
            }
        }

    }

}
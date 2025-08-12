using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using Random_Junk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Random_Junk
{
    public class Random_Junk : WildfrostMod
    {

        public static Random_Junk instance;

        public Random_Junk(string modDirectory) : base(modDirectory)
        {
            instance = this;
        }

        public override string GUID => "websiteofsites.wildfrost.contest";

        public override string[] Depends => new string[0];

        public override string Title => "The Unit Contest";

        public override string Description => "Eggie";

        private bool preLoaded = false;

        public static List<object> assets = new List<object>();

        private T TryGet<T>(string name) where T : DataFile
        {
            T data;
            if (typeof(StatusEffectData).IsAssignableFrom(typeof(T)))
                data = base.Get<StatusEffectData>(name) as T;
            else
                data = base.Get<T>(name);

            if (data == null)
                throw new Exception($"TryGet Error: Could not find a [{typeof(T).Name}] with the name [{name}] or [{Extensions.PrefixGUID(name, this)}]");

            return data;
        }

        private StatusEffectDataBuilder StatusCopy(string oldName, string newName)
        {
            StatusEffectData data = TryGet<StatusEffectData>(oldName).InstantiateKeepName();
            data.name = GUID + "." + newName;
            StatusEffectDataBuilder builder = data.Edit<StatusEffectData, StatusEffectDataBuilder>();
            builder.Mod = this;
            return builder;
        }

        private CardData.StatusEffectStacks SStack(string name, int amount) => new CardData.StatusEffectStacks(TryGet<StatusEffectData>(name), amount);

        private void CreateModAssets()
        {

            assets.Add(
                StatusCopy("Summon Fallow", "Summon Eggie")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectSummon)data).summonCard = TryGet<CardData>("eggie");
                })
                );

            assets.Add(
                StatusCopy("Instant Summon Copy", "Instant Summon Copy In Hand")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectInstantSummon)data).summonPosition = StatusEffectInstantSummon.Position.Hand;
                })
                );

            assets.Add(
                StatusCopy("When Deployed Add Junk To Hand", "When Deployed Add Eggie To Hand")
                .WithText("When deployed, summon a copy in your hand")
                .WithTextInsert("<card=websiteofsites.wildfrost.contest.eggie>")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectApplyXWhenDeployed)data).effectToApply = TryGet<StatusEffectData>("Instant Summon Copy In Hand");
                })
                );

            assets.Add(
                new CardDataBuilder(this).CreateUnit("eggie", "Eggie")
                .SetSprites("Eggie.png", "Eggie BG.png")
                .SetStats(1, null, 0)
                .WithCardType("Friendly")
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.startWithEffects = new CardData.StatusEffectStacks[1]
                    {
                        SStack("When Deployed Add Eggie To Hand",1)
                    };
                })
                );

            assets.Add(
                new StatusEffectDataBuilder(this).Create<StatusEffectInstantCombineCard>("Instant Combine Card Test")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectInstantCombineCard)data).cardNames = new string[] { "Tusk", "SunburstDart"};
                    ((StatusEffectInstantCombineCard)data).resultingCardName = "Spyke";
                    ((StatusEffectInstantCombineCard)data).checkDeck = false;
                    ((StatusEffectInstantCombineCard)data).checkBoard = true;
                    ((StatusEffectInstantCombineCard)data).changeDeck = true;
                    ((StatusEffectInstantCombineCard)data).spawnOnBoard = true;
                    ((StatusEffectInstantCombineCard)data).keepUpgrades = true;
                    ((StatusEffectInstantCombineCard)data).keepHealth = true;
                })
                );

            assets.Add(
                new StatusEffectDataBuilder(this).Create<StatusEffectApplyXWhenHit>("When Hit Combine Card Test")
                .WithText("Test Combine")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectApplyXWhenHit)data).effectToApply = TryGet<StatusEffectData>("Instant Combine Card Test");
                    ((StatusEffectApplyXWhenHit)data).applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                    ((StatusEffectApplyXWhenHit)data).targetMustBeAlive = false;
                    data.eventPriority = 999999999;
                    

                })
                );


            assets.Add(
                new StatusEffectDataBuilder(this).Create<StatusEffectApplyXOnCardPlayedWithPet>("When Played Gain Blings")
                .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                {
                    ((StatusEffectApplyX)data).effectToApply = TryGet<StatusEffectData>("Gain Gold");
                    ((StatusEffectApplyX)data).applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                    //Sadly we cannot set the petPrefab here as it gets destroyed, thus we will copy/edit the prefab in the Load method
                    ((StatusEffectApplyXOnCardPlayedWithPet)data).petPrefab = ((StatusEffectFreeAction)TryGet<StatusEffectData>("Free Action")).petPrefab.InstantiateKeepName();

                })
                );






            preLoaded = true;
        }
        public override void Load()
        {
            if (!preLoaded) { CreateModAssets(); }

            base.Load();

            //We need to make our petPrefab here, otherwise the Prefab gets destroyed leading to a crash
            Events.OnSceneChanged += LoadOomlin;


        }

        private void LoadOomlin(Scene scene)
        {
            if (scene.name == "Town")
            {
                ItemHolderPetNoomlin oomlin = ((StatusEffectFreeAction)TryGet<StatusEffectData>("Free Action")).petPrefab.InstantiateKeepName() as ItemHolderPetNoomlin;

                //Different head options, you can have as many/little as you like
                oomlin.headOptions = new Sprite[] { ImagePath("GoomlinBigHat.png").ToSprite(), ImagePath("GoomlinCrossStitch.png").ToSprite(), ImagePath("GoomlinEgg.png").ToSprite(), ImagePath("GoomlinStar.png").ToSprite(), ImagePath("GoomlinSwirl.png").ToSprite(), ImagePath("GoomlinDrill.png").ToSprite(), ImagePath("GoomlinWave.png").ToSprite() };
                foreach (Sprite sprite in oomlin.headOptions)
                {
                    sprite.name = GUID+"Goomlin";
                }

                foreach (Image image in oomlin.showTween.GetComponentsInChildren<Image>()) //Prefab for when oomlin sits on the card
                {
                    //To get the right offsets do the following:
                    //(1) Give a card your effect
                    //(2) Hover over the card and inspect this
                    //(3) Click Inspect GameObject
                    //(4) Click the arrow on Wobber, Flipper, CurveAnimator, Offset, Canvas, Front, FrameOutline, ItemHolderPetCreater, Noomlin(Clone)
                    //Click on Head to adjust head scale/position of head with ears
                    //Click on the arrow by Head and then click Head/EarLeft/EarRight to edit ears/head individuallys
                    switch (image.name)
                    {
                        case "Body": //Body that hangs on top of card
                            image.sprite = ImagePath("GoomlinBody.png").ToSprite(); break;
                        case "EarLeft": //Left ear
                            image.sprite = ImagePath("GoomlinEar_Left.png").ToSprite();
                            image.transform.Translate(new Vector3(0.1f, 0.4f, 0f)); break;
                        case "EarRight": //Right ear
                            image.sprite = ImagePath("GoomlinEar_Right.png").ToSprite();
                            image.transform.Translate(new Vector3(-0.1f, 0.4f, 0f)); break;
                        case "Tail": //Tail for when the -oomlin jumps off
                            image.sprite = ImagePath("GoomlinTail.png").ToSprite(); break;
                        case "Head": //Head, done just to rescale it
                            image.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                            image.transform.Translate(new Vector3(0f, 0.25f, 0f)); break;
                    }

                }

                ((StatusEffectApplyXOnCardPlayedWithPet)TryGet<StatusEffectData>("When Played Gain Blings")).petPrefab = oomlin;

            }

            
        }

        public override void Unload()
        {
            base.Unload();
            Events.OnSceneChanged -= LoadOomlin;
            UnloadFromClasses();
        }

        public override List<T> AddAssets<T, Y>()
        {
            if (assets.OfType<T>().Any())
                Debug.LogWarning($"[{Title}] adding {typeof(Y).Name}s: {assets.OfType<T>().Count()}");
            return assets.OfType<T>().ToList();
        }

        public void UnloadFromClasses()
        {
            List<ClassData> tribes = AddressableLoader.GetGroup<ClassData>("ClassData");
            foreach (ClassData tribe in tribes)
            {
                if (tribe == null || tribe.rewardPools == null) { continue; }

                foreach (RewardPool pool in tribe.rewardPools)
                {
                    if (pool == null) { continue; };

                    pool.list.RemoveAllWhere((item) => item == null || item.ModAdded == this);
                }
            }
        }
    }



    //Status Effect Class
    public class StatusEffectTriggerWhenCertainAllyAttacks : StatusEffectTriggerWhenAllyAttacks
    {
        //Cannot change allyInRow or againstTarget without some publicizing. Shade Snake is sad :(

        public CardData ally;

        public override bool RunHitEvent(Hit hit)
        {
            //Debug.Log(hit.attacker?.name);
            if (hit.attacker?.name == ally.name)
            {
                return base.RunHitEvent(hit);
            }
            return false;
        }
    }
}
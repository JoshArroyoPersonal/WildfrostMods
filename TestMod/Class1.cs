using Deadpan.Enums.Engine.Components.Modding;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;
using WildfrostHopeMod;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace TestMod
{
    public class TestMod : WildfrostMod
    {

        private List<CardUpgradeDataBuilder> charmlist;

        public TestMod(string modDirectory) : base(modDirectory)
        {

        }

        private void updateText()
        {

        }

        private void CreateModAssets()
        {
            GameObject gameObject = new GameObject("OvershroomIcon");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            StatusIcon overshroomicon = gameObject.AddComponent<StatusIcon>();
            Dictionary<string, GameObject> dicty = CardManager.cardIcons;
            GameObject text = dicty["shroom"].GetComponentInChildren<TextMeshProUGUI>().gameObject.InstantiateKeepName();
            text.transform.SetParent(gameObject.transform);
            overshroomicon.textElement = text.GetComponent<TextMeshProUGUI>();
            //overshroomicon.textElement = dicty["shroom"].GetComponent<StatusIcon>().textElement;
            overshroomicon.onCreate = new UnityEngine.Events.UnityEvent();
            overshroomicon.onDestroy = new UnityEngine.Events.UnityEvent();
            overshroomicon.onValueDown = new UnityEventStatStat();
            overshroomicon.onValueUp = new UnityEventStatStat();
            overshroomicon.textColour = dicty["shroom"].GetComponent<StatusIcon>().textColour;
            overshroomicon.textColourAboveMax = overshroomicon.textColour;
            overshroomicon.textColourBelowMax = overshroomicon.textColour;
            UnityEngine.Events.UnityEvent afterupdate = new UnityEngine.Events.UnityEvent();
            overshroomicon.afterUpdate = afterupdate;//dicty["shroom"].GetComponent<StatusIcon>().afterUpdate;
            //overshroomicon.onValueDown.AddListener(overshroomicon.Ping);
            Image image = gameObject.AddComponent<Image>();
            gameObject.SetActive(false);
            image.sprite = this.ImagePath("overshroomicon.png").ToSprite();
            CardHover cardHover = gameObject.AddComponent<CardHover>();
            cardHover.enabled = false;
            cardHover.IsMaster = false;
            CardPopUpTarget cardPopUp = gameObject.AddComponent<CardPopUpTarget>();
            
            cardHover.pop = cardPopUp;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta *= 0.01f;
            gameObject.SetActive(true);
            overshroomicon.type = "overshroom";
            dicty["overshroom"] = gameObject;

            StringTable keycollection = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
            KeywordData overshroomkey = Get<KeywordData>("shroom").InstantiateKeepName();
            overshroomkey.name = "Overshroom";
            keycollection.SetString(overshroomkey.name + "_text", "Overshroom");
            overshroomkey.titleKey = keycollection.GetString(overshroomkey.name + "_text");
            keycollection.SetString(overshroomkey.name + "_desc", "Acts like both <sprite name=overload> and <sprite name=shroom>");
            overshroomkey.descKey = keycollection.GetString(overshroomkey.name + "_desc");
            overshroomkey.ModAdded = this;
            overshroomkey.iconName = "overshroom";
            AddressableLoader.AddToGroup<KeywordData>("KeywordData", overshroomkey);

            cardPopUp.keywords = new KeywordData[1] { overshroomkey };

            /*Dictionary<string, GameObject> dicty = CardManager.cardIcons;
            GameObject gameObject = dicty["shroom"].InstantiateKeepName();
            gameObject.name = "OvershroomIcon";
            Debug.Log(gameObject.name);
            Image image = gameObject.GetComponent<Image>();
            Debug.Log(gameObject.name);
            image.sprite = this.ImagePath("overshroomicon.png").ToSprite();
            Debug.Log(gameObject.name);
            StatusIcon overshroomicon = gameObject.GetComponent<StatusIcon>();
            Debug.Log(gameObject.name);
            overshroomicon.type = "overshroom";
            Debug.Log(gameObject.name);
            dicty["overshroom"] = gameObject;
            Debug.Log(gameObject.name);*/

            dicty["vim"].GetComponent<Image>().sprite = this.ImagePath("amber.png").ToSprite();

            StringTable collection = LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);

            StatusEffectDummy dummyoverload = ScriptableObject.CreateInstance<StatusEffectDummy>();
            dummyoverload.name = "Overload";
            dummyoverload.type = "overload";
            dummyoverload.iconGroupName = "health";
            dummyoverload.visible = false;
            dummyoverload.targetConstraints = new TargetConstraint[0];
            dummyoverload.applyFormat = "";
            dummyoverload.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            dummyoverload.keyword = "";
            dummyoverload.textOrder = 0;
            dummyoverload.textInsert = "";
            dummyoverload.ModAdded = this;

            StatusEffectDummy dummyshroom = ScriptableObject.CreateInstance<StatusEffectDummy>();
            dummyshroom.name = "Shroom";
            dummyshroom.type = "shroom";
            dummyshroom.iconGroupName = "health";
            dummyshroom.visible = false;
            dummyshroom.targetConstraints = new TargetConstraint[0];
            dummyshroom.applyFormat = "";
            dummyshroom.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            dummyshroom.keyword = "";
            dummyshroom.textOrder = 0;
            dummyshroom.textInsert = "";
            dummyshroom.ModAdded = this;

            StatusEffectOvershroom overshroom = ScriptableObject.CreateInstance<StatusEffectOvershroom>();
            overshroom.name = "Overshroom";
            overshroom.type = "overshroom";
            overshroom.dummy1 = dummyoverload;
            overshroom.dummy2 = dummyshroom;
            overshroom.visible = true;
            overshroom.stackable = true;
            overshroom.buildupAnimation = ((StatusEffectOverload) Get<StatusEffectData>("Overload")).buildupAnimation;
            overshroom.iconGroupName = "health";
            overshroom.offensive = true;
            overshroom.applyFormat = "";
            overshroom.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            overshroom.keyword = "";
            overshroom.targetConstraints = new TargetConstraint[1] { new TargetConstraintCanBeHit()};
            overshroom.textOrder = 0;
            overshroom.textInsert = "{a}";
            overshroom.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", overshroom);

            StatusEffectBecomeOvershroom giveovershroom = ScriptableObject.CreateInstance<StatusEffectBecomeOvershroom>();
            giveovershroom.name = "Turn Overload and Shroom to Overshroom";
            giveovershroom.applyFormat = "";
            giveovershroom.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            giveovershroom.keyword = "";
            giveovershroom.targetConstraints = new TargetConstraint[0];
            giveovershroom.textKey = new UnityEngine.Localization.LocalizedString();
            giveovershroom.type = "";
            giveovershroom.textOrder = 0;
            giveovershroom.textInsert = "";
            giveovershroom.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", giveovershroom);

            StatusEffectWhileActiveX activeovershroom = ScriptableObject.CreateInstance<StatusEffectWhileActiveX>();
            activeovershroom.applyConstraints = new TargetConstraint[0];
            activeovershroom.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
            activeovershroom.doPing = true;
            activeovershroom.effectToApply = giveovershroom;
            activeovershroom.pauseAfter = 0;
            activeovershroom.targetMustBeAlive = true;
            activeovershroom.applyFormat = "";
            activeovershroom.applyFormatKey = new UnityEngine.Localization.LocalizedString();
            activeovershroom.keyword = "";
            activeovershroom.hiddenKeywords = new KeywordData[0];
            activeovershroom.targetConstraints = new TargetConstraint[0];
            activeovershroom.textInsert = "";
            activeovershroom.name = "While Active It Is Overshroom";
            collection.SetString(activeovershroom.name + "_text", "While active, your <keyword=overload> and <keyword=shroom> become <keyword=overshroom>");
            activeovershroom.textKey = collection.GetString(activeovershroom.name + "_text");
            activeovershroom.textOrder = 0;
            activeovershroom.type = "";
            activeovershroom.ModAdded = this;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", activeovershroom);
        }

        private void BigBooshu(CardData cardData)
        {
            UnityEngine.Debug.Log("[Tutorial1] New CardData Created: " + cardData.name);
            if (cardData.name == "BerryPet")
            {
                cardData.startWithEffects = CardData.StatusEffectStacks.Stack(cardData.startWithEffects, new CardData.StatusEffectStacks[1]
            {
                new CardData.StatusEffectStacks( Get<StatusEffectData>("While Active It Is Overshroom"), 1)
            });
                UnityEngine.Debug.Log("[Tutorial1] Booshu!");
            }
        }

        public override void Load()
        {
            CreateModAssets();
            Events.OnStatusIconCreated += PatchOvershroom;
            Events.OnCardDataCreated += BigBooshu;
            base.Load();

        }
        public override void Unload()
        {
            Events.OnStatusIconCreated -= PatchOvershroom;
            Events.OnCardDataCreated -= BigBooshu;
            base.Unload();

        }
        private void PatchOvershroom(StatusIcon icon)
        {
            if (icon.type == "overshroom")
            {
                icon.SetText();
                icon.Ping();
                icon.onValueDown.AddListener(delegate { icon.Ping(); });
                icon.onValueUp.AddListener(delegate { icon.Ping(); });
                icon.afterUpdate.AddListener(icon.SetText);
                icon.onValueDown.AddListener(icon.CheckDestroy);
            }
        }

        public override List<T> AddAssets<T, Y>()
        {
            //var typeName = typeof(Y).Name;
            //switch (typeName)
            //{
            //    case "CardUpgradeData": return charmlist.Cast<T>().ToList();
            //}

            return base.AddAssets<T, Y>();
        }

        public override string GUID => "websiteofsites.wildfrost.testmod";
        public override string[] Depends => new string[] { };
        public override string Title => "Charm Test";
        public override string Description => "Testing 2 charms";
    }

    public class StatusEffectBecomeOvershroom : StatusEffectData
    {
        public override bool HasApplyStatusRoutine => true;

        public override IEnumerator ApplyStatusRoutine(StatusEffectApply apply)
        {
            Debug.Log("[Test] ApplyStatusRoutine");
            if(apply != null && apply.applier?.owner == target.owner && apply.effectData?.offensive == true && (apply?.effectData.name == "Overload" || apply?.effectData.name == "Shroom"))
            {
                Debug.Log("[Test] found overload");
                apply.effectData = AddressableLoader.Get<StatusEffectData>("StatusEffectData","Overshroom");
            }

            return null;
        }
    }

    public class StatusEffectOvershroom : StatusEffectData
    {
        [SerializeField]
        public CardAnimation buildupAnimation;

        public bool overloading;

        public bool subbed;

        public bool primed;

        public override bool HasPostApplyStatusRoutine => true;

        public StatusEffectData dummy1;

        public StatusEffectData dummy2;

        public StatusEffectData dummy3;

        public override void Init()
        {
            base.OnStack += Stack;
            Events.OnEntityDisplayUpdated += EntityDisplayUpdated;
            base.OnTurnEnd += DealDamage;
            Events.OnPostProcessUnits += Prime;
            subbed = true;
        }

        public void OnDestroy()
        {
            Events.OnEntityDisplayUpdated -= EntityDisplayUpdated;
            Unsub();
        }

        public void EntityDisplayUpdated(Entity entity)
        {
            if (entity == target && target.enabled)
            {
                Check();
            }
        }

        public IEnumerator Stack(int stacks)
        {

            bool flag = true;
            List<StatusEffectData> effectstoremove = new List<StatusEffectData>();
            foreach (StatusEffectData effect in target.statusEffects)
            {
                if ((effect.name == "Shroom" || effect.name == "Overload") && effect.offensive == true)
                {
                    count += effect.count;
                    effectstoremove.Add(effect);
                }
            }

            foreach (StatusEffectData effect in effectstoremove)
            {
                yield return effect.RemoveStacks(effect.count, false);
            }


            foreach (StatusEffectData effect in target.statusEffects)
            {
                if (effect.name == "Shroom" && effect.offensive == false)
                {
                    flag = false;
                    if (effect.count < count)
                    {
                        Debug.Log("[Overshroom 1] too little shroom");
                        yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy1, count - effect.count));
                        yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy2, count - effect.count));
                        break;
                    }
                }
            }
            if (flag)
            {
                Debug.Log("[Overshroom 2] start apply");
                yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy2, count), true);
                yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy1, count), true);
            }


            Check();
            yield return null;
        }

        public void Check()
        {
            if (count >= target.hp.current && !overloading)
            {
                ActionQueue.Stack(new ActionSequence(DealDamage(count))
                {
                    fixedPosition = true,
                    priority = eventPriority,
                    note = "Overload"
                });
                ActionQueue.Stack(new ActionSequence(Clear())
                {
                    fixedPosition = true,
                    priority = eventPriority,
                    note = "Clear Overload"
                });
                overloading = true;
            }
        }

        public override IEnumerator PostApplyStatusRoutine(StatusEffectApply apply)
        {
            foreach (StatusEffectData effect in target.statusEffects)
            {
                if (effect.name == "Shroom")
                {
                    if (effect.count > count)
                    {
                        Debug.Log("[Overshroom 2] too much shroom");
                        yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy1, effect.count - count));
                        count = effect.count;
                        break;
                    }
                }

                if (effect.name == "Overload")
                {
                    if (effect.count > count)
                    {
                        Debug.Log("[Overshroom 2] too much overload");
                        yield return ActionQueue.Stack(new ActionApplyStatus(target, target, dummy2, effect.count - count));
                        count = effect.count;
                        break;
                    }
                }
            }
        }

        public IEnumerator DealDamage(int damage)
        {
            if (!this || !target || !target.alive)
            {
                yield break;
            }

            HashSet<Entity> targets = new HashSet<Entity>();
            CardContainer[] containers = target.containers;
            foreach (CardContainer collection in containers)
            {
                targets.AddRange(collection);
            }

            if ((bool)buildupAnimation)
            {
                yield return buildupAnimation.Routine(target);
            }

            Entity damager = GetDamager();
            Routine.Clump clump = new Routine.Clump();
            foreach (Entity item in targets)
            {
                Hit hit = new Hit(damager, item, damage)
                {
                    damageType = "overload"
                };
                clump.Add(hit.Process());
            }

            clump.Add(Sequences.Wait(0.5f));
            yield return clump.WaitForEnd();
        }

        public IEnumerator Clear()
        {
            if ((bool)this && (bool)target && target.alive)
            {
                yield return Remove();
                overloading = false;
            }
        }

        public void Unsub()
        {
            if (subbed)
            {
                Events.OnPostProcessUnits -= Prime;
                subbed = false;
            }
        }

        public void Prime(Character character)
        {
            primed = true;
            Unsub();
        }

        public override bool RunTurnEndEvent(Entity entity)
        {
            if (primed && target.enabled)
            {
                return entity == target;
            }

            return false;
        }

        public IEnumerator DealDamage(Entity entity)
        {
            int amount = 1;
            Events.InvokeStatusEffectCountDown(this, ref amount);
            if (amount != 0)
            {
                yield return CountDown(entity, amount);
            }
            Hit hit = new Hit(GetDamager(), target, count+1)
            {
                screenShake = 0.25f,
                damageType = "shroom"
            };
            yield return hit.Process();
            yield return Sequences.Wait(0.2f);
        }

    }

    public class StatusEffectDummy : StatusEffectData
    {
        public bool dummy = true;
        public string truename = string.Empty;

        public override void Init()
        {
            temporary = 99;
            base.OnTurnEnd += Decrease;
        }

        public IEnumerator Decrease(Entity entity) 
        {
            int amount = 1;
            Events.InvokeStatusEffectCountDown(this, ref amount);
            if (amount != 0)
            {
                yield return CountDown(entity, amount);
            }
        }

  
    }


}

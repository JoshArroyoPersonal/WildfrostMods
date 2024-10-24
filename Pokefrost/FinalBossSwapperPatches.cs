using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    //Replace Cards?
    //Remove Injuries
    //Remove Upgrade
    //Process Effects
    //Run Scripts

    [HarmonyPatch(typeof(FinalBossGenerationSettings), "ReplaceCards", new Type[]
        {
            typeof(IList<CardData>)
        })]
    class AppendCardReplacement
    {
        internal static void Prefix(FinalBossGenerationSettings __instance)
        {
            foreach (FinalBossGenerationSettings.ReplaceCard replacement in __instance.replaceCards)
            {
                if (replacement.card.name == "websiteofsites.wildfrost.pokefrost.eevee")
                {
                    return;
                }
            }

            List<FinalBossGenerationSettings.ReplaceCard> replaceCards = new List<FinalBossGenerationSettings.ReplaceCard>();
            foreach (CardDataBuilder builder in Pokefrost.instance.list)
            {
                CardData card = Pokefrost.instance.Get<CardData>(builder._data.name);
                foreach (CardData.StatusEffectStacks stack in card.startWithEffects)
                {
                    if (stack.data is StatusEffectEvolve ev)
                    {
                        Debug.Log($"[Pokefrost] Replacing {card.name}");
                        CardData[] cards = ev.EvolveForFinalBoss(Pokefrost.instance);
                        if (cards != null)
                        {
                            FinalBossGenerationSettings.ReplaceCard replacement = new FinalBossGenerationSettings.ReplaceCard
                            {
                                card = card,
                                options = cards
                            };
                            replaceCards.Add(replacement);
                        }
                    }
                }
            }
            __instance.replaceCards = __instance.replaceCards.AddRangeToArray(replaceCards.ToArray()).ToArray();
        }
    }


    [HarmonyPatch(typeof(FinalBossGenerationSettings), "Process")]
    class AppendFinalBossGeneration
    {
        internal static void Prefix(FinalBossGenerationSettings __instance)
        {
            foreach (FinalBossEffectSwapper swapper in __instance.effectSwappers)
            {
                if (swapper.effect.name.Contains("Buff Card In Deck On Kill")) //If it's already there no need to check further.
                {
                    return;
                }
            }

            AddTraits(__instance);
            AddEffectSwappers(__instance);
            AddScripts(__instance);

        }

        static string[] traitsToRemove = new string[]
        {

        };

        internal static void AddTraits(FinalBossGenerationSettings __instance)
        {
            __instance.ignoreTraits = __instance.ignoreTraits.AddRangeToArray(traitsToRemove.Select(s => Pokefrost.instance.TryGet<TraitData>(s)).ToArray());
        }

        internal static void AddEffectSwappers(FinalBossGenerationSettings __instance)
        {

            List<FinalBossEffectSwapper> swappers = new List<FinalBossEffectSwapper>
            {
                CreateSwapper("While Active Frenzy To Crown Allies", "While Active Frenzy To Allies", minBoost: 0, maxBoost: 0),
                CreateSwapper("Give Thick Club", "On Card Played Buff Marowak", minBoost: 0, maxBoost: 1),
                CreateSwapper("While Active Increase Effects To Hand", "Ongoing Increase Effects", minBoost: 0, maxBoost: 0),
                CreateSwapper("Give Slowking Crown", minBoost: 0, maxBoost: 0),
                CreateSwapper("Give Combo to Card in Hand", "On Turn Apply Attack To Self", minBoost: 0, maxBoost: 1),
                CreateSwapper("Discard Rightmost Button", minBoost: 0, maxBoost: 0),
                CreateSwapper("When Deployed Sketch", attackOption: "Null", minBoost: 0, maxBoost: 0),
                CreateSwapper("Trigger All Button", "When Destroyed Trigger To Allies", minBoost: 0, maxBoost: 0),
                CreateSwapper("Trigger All Listener_1", minBoost: 0, maxBoost: 0),
                CreateSwapper("When Ally Summoned Add Skull To Hand", minBoost: 0, maxBoost: 0),
                CreateSwapper("Trigger When Summon", "Trigger When Card Destroyed", minBoost: 0, maxBoost: 0),
                CreateSwapper("On Hit Snowed Target Double Attack Otherwise Half", attackOption: "Snow", minBoost: 1, maxBoost: 2),
                CreateSwapper("Buff Card In Deck On Kill", "On Turn Apply Attack To Self", minBoost: 1, maxBoost: 3),
                CreateSwapper("Trigger Clunker Ahead", "On Turn Apply Scrap To RandomAlly", minBoost: 0, maxBoost: 0),
                CreateSwapper("On Card Played Increase Attack Of Cards In Hand", "While Active Increase Attack To Allies", minBoost: 0, maxBoost: 0),
                CreateSwapper("When Hit Add Scrap Pile To Hand", "On Turn Apply Scrap To RandomAlly", minBoost: 0, maxBoost: 0),
                CreateSwapper("End of Turn Draw a Card", "When Hit Add Junk To Hand", minBoost: 0, maxBoost: 0),
                CreateSwapper("While Active It Is Overshroom", attackOption: "Overload", minBoost: 4, maxBoost: 4),
                CreateSwapper("Gain Frenzy When Companion Is Killed", "MultiHit", minBoost: 2, maxBoost: 3),
                CreateSwapper("Revive", "Heal Self", minBoost: 3, maxBoost: 5),
                CreateSwapper("Rest Button", "Heal Self", minBoost: 3, maxBoost: 5),
                CreateSwapper("Rest Listener_1", minBoost: 0, maxBoost: 0),
                CreateSwapper("Redraw Cards", "Trigger When Redraw Hit", minBoost: 0, maxBoost: 0),
                CreateSwapper("Add Tar Blade Button", minBoost: 0, maxBoost: 0),
                CreateSwapper("Tar Shot Listener_1", minBoost: 0, maxBoost: 0)
            };
            __instance.effectSwappers = __instance.effectSwappers.AddRangeToArray(swappers.ToArray()).ToArray();
        }

        internal static FinalBossEffectSwapper CreateSwapper(string effect, string replaceOption = null, string attackOption = null, int minBoost = 0, int maxBoost = 0)
        {
            FinalBossEffectSwapper swapper = ScriptableObject.CreateInstance<FinalBossEffectSwapper>();
            swapper.effect = Pokefrost.instance.TryGet<StatusEffectData>(effect);
            swapper.name = swapper.effect.name;
            swapper.replaceWithOptions = new StatusEffectData[0];
            //String s = "";
            if (!replaceOption.IsNullOrEmpty())
            {
                swapper.replaceWithOptions = swapper.replaceWithOptions.Append(Pokefrost.instance.Get<StatusEffectData>(replaceOption)).ToArray();
                //s += swapper.replaceWithOptions[0].name;
            }
            if (!attackOption.IsNullOrEmpty())
            {
                swapper.replaceWithAttackEffect = Pokefrost.instance.TryGet<StatusEffectData>(attackOption);
                //s += swapper.replaceWithAttackEffect.name;
            }
            /*if (s.IsNullOrEmpty())
            {
                s = "Nothing";
            }*/
            swapper.boostRange = new Vector2Int(minBoost, maxBoost);
            //Debug.Log($"[Pokefrost] {swapper.effect.name} => {s} + {swapper.boostRange}");
            return swapper;
        }

        static void AddScripts(FinalBossGenerationSettings __instance)
        {
            FinalBossCardModifier[] modifiers = new FinalBossCardModifier[]
            {

            };
            __instance.cardModifiers = __instance.cardModifiers.AddRangeToArray(modifiers);
        }

        static FinalBossCardModifier CreateCardModifier(string name, params CardScript[] scripts)
        {
            FinalBossCardModifier modifier = ScriptableObject.CreateInstance<FinalBossCardModifier>();
            modifier.name = name;
            modifier.card = Pokefrost.instance.TryGet<CardData>(name);
            modifier.runAll = scripts;
            return modifier;
        }

        static CardScriptAddPassiveEffect PassiveScript(string name, int min, int max)
        {
            CardScriptAddPassiveEffect script = ScriptableObject.CreateInstance<CardScriptAddPassiveEffect>();
            script.name = "Add Passive: " + name;
            script.countRange = new Vector2Int(min, max);
            return script;
        }

        static CardScriptAddAttackEffect AttackScript(string name, int min, int max)
        {
            CardScriptAddAttackEffect script = ScriptableObject.CreateInstance<CardScriptAddAttackEffect>();
            script.name = "Add Attack Effect: " + name;
            script.countRange = new Vector2Int(min, max);
            return script;
        }

        static CardScriptAddTrait TraitScript(string name, int min, int max)
        {
            CardScriptAddTrait script = ScriptableObject.CreateInstance<CardScriptAddTrait>();
            script.name = "Add Trait: " + name;
            script.countRange = new Vector2Int(min, max);
            return script;
        }
    }
}

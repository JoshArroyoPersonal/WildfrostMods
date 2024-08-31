using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public class StatusEffectEvolveFromNode : StatusEffectEvolve
    {
        public string targetNodeName = "Salty Spicelands";
        public bool readyToEvolve = false;

        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);
            type = "evolve3";
        }

        //This method is effectively ReadyToEvolve
        public virtual bool NodeVisit(string nodeName, CardData cardData)
        {
            if (nodeName == targetNodeName)
            {
                return true;
            }
            return false;
        }

        public override bool ReadyToEvolve(CardData cardData)
        {
            return false;
        }

        [HarmonyPatch(typeof(CampaignNode), "SetCleared")]
        internal static class AfterNodeCleared
        {
            internal static void Prefix(CampaignNode __instance)
            {
                CheckEvolve<StatusEffectEvolveFromNode>(References.Player.data.inventory.deck, "evolve3", (s, c) => s.NodeVisit(__instance.name, c));
                CheckEvolve<StatusEffectEvolveFromNode>(References.Player.data.inventory.reserve, "evolve3", (s, c) => s.NodeVisit(__instance.name, c));

                if (EvolutionPopUp.evolvedPokemonLastBattle.Count > 0)
                {
                    References.instance.StartCoroutine(EvolutionPopUp.DelayedRun());
                }

                /*Debug.Log($"[Pokefrost] {__instance.name}");
                CardDataList list = References.Player.data.inventory.deck;
                List<CardData> slateForEvolution = new List<CardData>();
                List<StatusEffectEvolve> evolveEffects = new List<StatusEffectEvolve>();
                foreach (CardData card in list)
                {
                    foreach (CardData.StatusEffectStacks s in card.startWithEffects)
                    {
                        if (s.data is StatusEffectEvolveFromNode s2)
                        {
                            ;
                            if (s2.NodeVisit(__instance.name, card))
                            {
                                Debug.Log("[Pokefrost] Ready for evolution!");
                                slateForEvolution.Add(card);
                                evolveEffects.Add(((StatusEffectEvolve)s.data));
                            }
                        }
                    }
                }
                list = References.Player.data.inventory.reserve;
                foreach (CardData card in list)
                {
                    foreach (CardData.StatusEffectStacks s in card.startWithEffects)
                    {
                        if (s.data is StatusEffectEvolveFromNode s2)
                        {
                            if (s2.NodeVisit(__instance.name, card))
                            {
                                Debug.Log("[Pokefrost] Ready for evolution!");
                                slateForEvolution.Add(card);
                                evolveEffects.Add(((StatusEffectEvolve)s.data));
                            }
                        }
                    }
                }
                int count = slateForEvolution.Count;

                for (int i = 0; i < count; i++)
                {
                    if (References.Player.data.inventory.deck.RemoveWhere((CardData a) => slateForEvolution[i].id == a.id))
                    {
                        Debug.Log("[" + slateForEvolution[i].name + "] Removed From [" + References.Player.name + "] deck");
                        evolveEffects[i].Evolve(Pokefrost.instance, slateForEvolution[i]);
                    }
                    if (References.Player.data.inventory.reserve.RemoveWhere((CardData a) => slateForEvolution[i].id == a.id))
                    {
                        Debug.Log("[" + slateForEvolution[i].name + "] Removed From [" + References.Player.name + "] reserve");
                        evolveEffects[i].Evolve(Pokefrost.instance, slateForEvolution[i]);
                    }
                }
                if (count > 0)
                {
                    References.instance.StartCoroutine(EvolutionPopUp.DelayedRun());
                }*/
            }
        }
    }

    public class StatusEffectEvolveSlowpoke : StatusEffectEvolveFromNode
    {
        public string evolveUncrowned = "websiteofsites.pokefrost.slowbro";
        public string evolveCrowned = "websiteofsites.pokefrost.slowking";
        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);
            evolutionCardName = "websiteofsites.pokefrost.slowbro";
        }

        public override void Evolve(WildfrostMod mod, CardData preEvo)
        {
            if (preEvo.HasCrown)
            {
                evolutionCardName = evolveCrowned;
                References.Player.data.inventory.upgrades.Add(mod.Get<CardUpgradeData>("CrownSlowking"));
            }
            else
            {
                evolutionCardName = evolveUncrowned;
            }
            base.Evolve(mod, preEvo);
        }
    }
}

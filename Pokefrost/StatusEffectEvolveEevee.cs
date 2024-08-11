using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public class StatusEffectEvolveEevee : StatusEffectEvolve
    {
        public static Dictionary<string, string> upgradeMap = new Dictionary<string, string>();

        private string flareon = "flareon";
        private string vaporeon = "vaporeon";
        private string jolteon = "jolteon";
        private string espeon = "espeon";
        private string umbreon = "umbreon";
        private string glaceon = "glaceon";
        private string leafeon = "leafeon";
        private string sylveon = "sylveon";

        public static readonly string[] eeveelutions = new string[8] { "flareon", "vaporeon", "jolteon", "espeon", "umbreon", "glaceon", "leafeon", "sylveon" };
        public enum eeveeEnum
        {
            Flareon = 0,
            Vaporeon = 1,
            Jolteon = 2,
            Espeon = 3,
            Umbreon = 4,
            Glaceon = 5,
            Leafeon = 6,
            Sylveon = 7
        }

        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);
            if(upgradeMap.ContainsKey("CardUpgradeOverload"))
            {
                return;
            }
            upgradeMap.Add("CardUpgradeOverload", "flareon");
            upgradeMap.Add("CardUpgradeSpice", "flareon");
            upgradeMap.Add("CardUpgradeBattle", "flareon");
            upgradeMap.Add("CardUpgradeBombskull", "flareon");
            upgradeMap.Add("CardUpgradeFlameberry", "flareon");

            upgradeMap.Add("CardUpgradeInk", "vaporeon");
            upgradeMap.Add("CardUpgradeFury", "vaporeon");
            upgradeMap.Add("CardUpgradeBlock", "vaporeon");
            upgradeMap.Add("CardUpgradeRemoveCharmLimit", "vaporeon");

            upgradeMap.Add("CardUpgradeSpark", "jolteon");
            upgradeMap.Add("CardUpgradeDraw", "jolteon");
            upgradeMap.Add("CardUpgradeFrenzyReduceAttack", "jolteon");
            upgradeMap.Add("CardUpgradeWildcard", "jolteon");
            upgradeMap.Add("CardUpgradeGlass", "jolteon");
            upgradeMap.Add("websiteofsites.wildfrost.pokefrost.CardUpgradeMagnemite", "jolteon");

            upgradeMap.Add("CardUpgradeBalanced", "espeon");
            upgradeMap.Add("CardUpgradeBom", "espeon");
            upgradeMap.Add("CardUpgradeBoost", "espeon");
            upgradeMap.Add("CardUpgradeSun", "espeon");
            upgradeMap.Add("CardUpgradeBootleg", "espeon");

            upgradeMap.Add("CardUpgradeGreed", "umbreon");
            upgradeMap.Add("CardUpgradeTeethWhenHit", "umbreon");
            upgradeMap.Add("CardUpgradeSpiky", "umbreon");
            upgradeMap.Add("CardUpgradeDemonize", "umbreon");
            upgradeMap.Add("websiteofsites.wildfrost.pokefrost.CardUpgradePluck", "umbreon");

            upgradeMap.Add("CardUpgradeSnowball", "glaceon");
            upgradeMap.Add("CardUpgradeFrosthand", "glaceon");
            upgradeMap.Add("CardUpgradeSnowImmune", "glaceon");
            upgradeMap.Add("CardUpgradeAttackIncreaseCounter", "glaceon");
            upgradeMap.Add("CardUpgradeBlue", "glaceon");

            upgradeMap.Add("CardUpgradeAcorn", "leafeon");
            upgradeMap.Add("CardUpgradeShellOnKill", "leafeon");
            upgradeMap.Add("CardUpgradeShroom", "leafeon");
            upgradeMap.Add("CardUpgradeShroomReduceHealth", "leafeon");
            upgradeMap.Add("CardUpgradePlink", "leafeon");

            upgradeMap.Add("CardUpgradeAttackAndHealth", "sylveon");
            upgradeMap.Add("CardUpgradeHeart", "sylveon");
            upgradeMap.Add("CardUpgradeCloudberry", "sylveon");
            upgradeMap.Add("CardUpgradePig", "sylveon");
            upgradeMap.Add("CardUpgradeHeartmist", "sylveon");

            
            //Abigail Mods
            upgradeMap.Add("absentabigail.wildfrost.blahaj.CardUpgradeShark", vaporeon);

            //Bethanw Mods
            upgradeMap.Add("bethanw10.wildfrost.allcharms.CardUpgradeMakoko", flareon);
            upgradeMap.Add("bethanw10.wildfrost.allcharms.CardUpgradeScales", espeon);
            upgradeMap.Add("bethanw10.wildfrost.allcharms.CardUpgradeShadeHeart", umbreon);

            //Green Ranger Mods
            upgradeMap.Add("greenranger.wildfrost.devilcharm.CardUpgradeDevil", flareon);
            upgradeMap.Add("greenranger.wildfrost.basketcharm.CardUpgradeCleanse", sylveon);

            //Miya Mods
            upgradeMap.Add("kopie.wildfrost.charmscollection.charm_speed", jolteon);

            //Semmiesem Mods
            upgradeMap.Add("semmiesem9.wildfrost.hatsunemiku.BobaCharm", sylveon);

            //Shortcake Mods
            upgradeMap.Add("shortcake.charms.CrystalShardCharm", vaporeon);
            upgradeMap.Add("shortcake.charms.SnowFlowerCharm", glaceon);
            upgradeMap.Add("shortcake.charms.LuminiceCharm", espeon);
            upgradeMap.Add("shortcake.charms.BarrierCharm", vaporeon);
            upgradeMap.Add("shortcake.charms.OrangeCharm", jolteon);

            //Vibe Mods
            upgradeMap.Add("vibeneutral.wildfrost.hollowfrost.focuscharm", espeon);
            upgradeMap.Add("vibeneutral.wildfrost.hollowfrost.grimmcharm", flareon);
            upgradeMap.Add("vibeneutral.wildfrost.hollowfrost.defendercharm", umbreon);

            type = "evolve2";
        }

        public override bool ReadyToEvolve(CardData cardData)
        {
            foreach (CardUpgradeData upgrade in cardData.upgrades)
            {
                if (upgrade.type == CardUpgradeData.Type.Charm)
                {
                    return true;
                }
            }
            return false;
        }

        private void FindEvolution(CardData carddata)
        {
            foreach (CardUpgradeData upgrade in carddata.upgrades)
            {
                if (upgrade.type == CardUpgradeData.Type.Charm)
                {
                    if (upgradeMap.ContainsKey(upgrade.name))
                    {
                        evolutionCardName = upgradeMap[upgrade.name];
                    }
                    else
                    {
                        UnityEngine.Debug.Log("[[Michael]] Unrecognized/neutral charm: randomizing evolution.");
                        int r = UnityEngine.Random.Range(0, 7);
                        evolutionCardName = eeveelutions[r];

                    }
                    UnityEngine.Debug.Log("[Pokefrost] Evolving into " + evolutionCardName);
                }
            }
        }

        public override CardData[] EvolveForFinalBoss(WildfrostMod mod)
        {
            CardData[] cards = eeveelutions.Select((s) => mod.Get<CardData>(s)).ToArray();
            return cards;
        }

        public override void Evolve(WildfrostMod mod, CardData preEvo)
        {
            FindEvolution(preEvo);
            evolutionCardName = Extensions.PrefixGUID(evolutionCardName, mod);
            Debug.Log("[Pokefrost] " + evolutionCardName);
            base.Evolve(mod, preEvo);
        }
    }
}

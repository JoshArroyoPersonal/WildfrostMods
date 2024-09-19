﻿using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BattleEditor;
using Debug = UnityEngine.Debug;
using System.Collections;

namespace Pokefrost
{
    internal class EventBattleManager
    {
        public static EventBattleManager instance;

        public static readonly float minChance = 0.25f;
        public static readonly float maxChance = 1f;

        public bool successfulRoll = false;

        public static Dictionary<string, string> battleList = new Dictionary<string, string>();
        public static string battleChosen = "";

        public EventBattleManager() 
        {
            instance = this;
        }

        public void Enable(Pokefrost mod)
        {
            CreateBattles(mod);
            Events.OnCampaignLoadPreset += RollForEvent;
            Events.OnPreCampaignPopulate += HideNode;
            MoreEvents.OnCampaignGenerated += AddEventItems;
        }

        public void Disable(Pokefrost mod)
        {
            battleList.Clear();
            Events.OnCampaignLoadPreset -= RollForEvent;
            Events.OnPreCampaignPopulate -= HideNode;
            MoreEvents.OnCampaignGenerated -= AddEventItems;
        }

        private void CreateBattles(Pokefrost mod)
        {

            new BattleDataEditor(mod, "Darkrai")
                .SetSprite(mod.ImagePath("darkraiCharm.png").ToSprite())
                .SetNameRef("Dark Crater Pit")
                .EnemyDictionary(('D', "enemy_darkrai"), ('H', "enemy_hypno"), ('M', "enemy_mismagius"), ('G', "enemy_magmortar"), ('S', "enemy_spiritomb"))
                .StartWavePoolData(0, "Curses!")
                .ConstructWaves(4, 0, "SMMS")
                .StartWavePoolData(1, "More curses")
                .ConstructWaves(4, 1, "HMMG", "GMMH", "HSMG", "SSHG")
                .StartWavePoolData(2, "Darkrai is here!")
                .ConstructWaves(3, 9, "DMH", "DGH")
                .GiveMiniBossesCharms(new string[1] { "enemy_darkrai" }, "CardUpgradeDemonize", "CradUpgradeInk")
                .AddBattleToLoader(); //Loads and makes it the mandatory first fight

            new BattleDataEditor(mod, "Lati Twins")
                .SetSprite(mod.ImagePath("smeargleCharm.png").ToSprite())
                .SetNameRef("Southern Island")
                .EnemyDictionary(('P', "enemy_plusle"), ('M', "enemy_minun"), ('V', "enemy_volbeat"), ('I', "enemy_illumise"), ('D', "enemy_dustox"), ('B', "enemy_beautifly"), ('G', "enemy_gorebyss"), ('H', "enemy_huntail"), ('S', "enemy_solrock"), ('L', "enemy_lunatone"), ('A', "enemy_latias"), ('O', "enemy_latios"))
                .StartWavePoolData(0, "Charging up")
                .ConstructWaves(4, 0, "PMDB")
                .StartWavePoolData(1, "Scary")
                .ConstructWaves(4, 1, "VIGH")
                .StartWavePoolData(2, "Lati!")
                .ConstructWaves(4, 9, "SLAO")
                .GiveMiniBossesCharms(new string[1] { "enemy_latias" }, "CardUpgradeHeartmist", "CardUpgradeAcorn")
                .GiveMiniBossesCharms(new string[1] { "enemy_latios" }, "CardUpgradeSpice", "CardUpgradeBattle")
                .AddBattleToLoader(); //Loads and makes it the mandatory first fight

            BattleDataEditor hooh = new BattleDataEditor(mod, "Ho-Oh")
                .SetSprite(mod.ImagePath("darkraiCharm.png").ToSprite())
                .SetNameRef("Mt. Faraway")
                .EnemyDictionary(('H', "enemy_hooh"), ('E', "enemy_entei"), ('R', "enemy_raikou"), ('S', "enemy_suicune"), ('V', "enemy_vaporeon"), ('J', "enemy_jolteon"), ('F', "enemy_flareon"), ('P', "enemy_espeon"), ('U', "enemy_umbreon"), ('L', "enemy_leafeon"), ('G', "enemy_glaceon"), ('Y', "enemy_sylveon"))
                .StartWavePoolData(0, "Mystery")
                .ConstructWaves(1, 0, "H")
                .StartWavePoolData(1, "Beasts")
                .ConstructWaves(2, 1, "EF", "RJ", "SV")
                .StartWavePoolData(2, "Eeveeloutions")
                .ConstructWaves(2, 9, "PU", "GL", "Y")
                .GiveMiniBossesCharms(new string[1] { "enemy_hooh" }, "CardUpgradeCloudberry", "CardUpgradeBlock")
                .GiveMiniBossesCharms(new string[1] { "enemy_suicune" }, "CardUpgradeBlock", "CardUpgradeBoost")
                .GiveMiniBossesCharms(new string[1] { "enemy_raikou" }, "CardUpgradeSun", "CardUpgradeBarrage")
                .GiveMiniBossesCharms(new string[1] { "enemy_entei" }, "CardUpgradeAttackAndHealth", "CardUpgradeTrashBad")
                .SetGenerationScript(ScriptableObject.CreateInstance<BattleGenerationScriptHooh>())
                .AddBattleToLoader(); //Loads and makes it the mandatory first fight

            battleList["Darkrai"] = "darkraiEvent";
            battleList["Lati Twins"] = "latiEvent";
            battleList["Ho-oh"] = "hoohEvent";
        }

        private void RollForEvent(ref string[] lines)
        {
            int width = lines.Length;
            int length = lines[0].Length;
            for (int i = 0; i < length; i++)
            {
                if (lines[0][i] == 'B' && lines[width - 1].Length > i && lines[width - 1][i] == '1')
                {
                    lines[0] = lines[0].Insert(i + 1, "e");
                    lines[width - 2] = lines[width - 2].Insert(i + 1, "6");
                    lines[width - 1] = lines[width - 1].Insert(i + 1, "2");
                    for (int j = 1; j < width - 2; j++)
                    {
                        lines[j] = lines[j].Insert(i + 1, " ");
                    }
                    successfulRoll = true;
                    return;
                }
            }
        }

        private void HideNode()
        {
            if (!successfulRoll) { return; }
            for (int i= 0; i < Campaign.instance.nodes.Count; i++)
            {
            CampaignNode node = Campaign.instance.nodes[i];
                if (node.type.letter == "e")
                {

                    Debug.Log("[Pokefrost] Hiding battle node");
                    CampaignNode prevNode = Campaign.instance.nodes[i - 1];
                    prevNode.connections = node.connections;
                    node.connections = new List<CampaignNode.Connection>();
                    break;
                    //node.connections.Do((n) => Campaign.GetNode(n.otherId).connectedTo = bossNode.id);
                }
            }
        }

        internal void ReturnNode()
        {
            for (int i = 0; i < Campaign.instance.nodes.Count; i++)
            {
                CampaignNode node = Campaign.instance.nodes[i];
                if (node.type.letter == "e")
                {

                    Debug.Log("[Pokefrost] Returning battle node");
                    CampaignNode prevNode = Campaign.instance.nodes[i - 1];
                    if (node.connections.Count > 0) { return; }
                    node.connections = prevNode.connections;
                    prevNode.connections = new List<CampaignNode.Connection>
                    {
                        new CampaignNode.Connection()
                        {
                            otherId = node.id,
                            direction = 1
                        }
                    };
                    break;
                    //node.connections.Do((n) => Campaign.GetNode(n.otherId).connectedTo = bossNode.id);
                }
            }
        }

        private void AddEventItems()
        {
            if (!successfulRoll) { return; }
            Debug.Log("[Pokefrost] Starting...");
            for (int i = 0; i < References.Campaign.nodes.Count; i++)
            {
                CampaignNode node = References.Campaign.nodes[i];
                if (node.type is CampaignNodeTypeBoss)
                {
                    Debug.Log("[Pokefrost] Found Act I Boss node");
                    CampaignNodeTypeBoss.RewardData rewards = node.data.Get<CampaignNodeTypeBoss.RewardData>("rewards");
                    List<BossRewardData.Data> rewardList = rewards.rewards;
                    BossRewardDataModifier.Data data = new BossRewardDataModifier.Data
                    {
                        modifierName = Pokefrost.instance.GUID + "." + battleList[battleChosen]
                    };
                    rewardList.Add(data);
                    Debug.Log("[Pokefrost] Success!");
                    
                    break;
                }
            }
            successfulRoll = false;
        }
    }

    public class ScriptReturnNode : Script
    {
        public override IEnumerator Run()
        {
            EventBattleManager.instance.ReturnNode();
            yield break;
        }
    }
}

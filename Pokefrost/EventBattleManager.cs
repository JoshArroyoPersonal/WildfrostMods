using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BattleEditor;
using Debug = UnityEngine.Debug;
using Data = BossRewardData.Data;
using System.Collections;

namespace Pokefrost
{
    internal class EventBattleManager
    {
        public static EventBattleManager instance;

        public static float minChance = 0.25f;
        public static float maxChance = 1f;

        public bool successfulRoll = false;

        public static Dictionary<string, string> battleList = new Dictionary<string, string>();
        public static string battleChosen = "";
        public static string forceBattle = "";

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

            new BattleDataEditor(mod)
                .Create<BattleDataWithRewards>("Darkrai")
                .SetSprite("MapDarkrai.png", 200)
                .SetNameRef("Dark Crater Pit")
                .EnemyDictionary(('D', "enemy_darkrai"), ('H', "enemy_hypno"), ('M', "enemy_mismagius"), ('G', "enemy_magmortar"), ('S', "enemy_spiritomb"))
                .StartWavePoolData(0, "Curses!")
                .ConstructWaves(4, 0, "SMMS")
                .StartWavePoolData(1, "More curses")
                .ConstructWaves(4, 1, "HMMG", "GMMH", "HSMG", "SSHG")
                .StartWavePoolData(2, "Darkrai is here!")
                .ConstructWaves(3, 9, "DMH", "DGH")
                .GiveMiniBossesCharms(new string[1] { "enemy_darkrai" }, "CardUpgradeDemonize", "CardUpgradeInk")
                .FreeModify<BattleDataWithRewards>(b =>
                {
                    b.dataGroups = new List<Data>[3];
                    b.dataGroups[0] = new List<Data> 
                    { 
                        CreateCard("darkrai"), 
                        CreateCard("cresselia")
                    };
                    b.dataGroups[1] = new List<Data>
                    {
                        CreateCharm("CardUpgradeCurse"),
                        CreateCharm("CardUpgradeDuplicate")
                    };
                    b.dataGroups[2] = new List<Data>
                    {
                        CreateBell("BlessingDarkrai"),
                        CreateBell("BlessingCresselia")
                    };
                })
                .AddBattleToLoader(); //Loads and makes it the mandatory first fight

            new BattleDataEditor(mod)
                .Create<BattleDataWithRewards>("Lati Twins")
                .SetSprite("MapLati.png", 200)
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
                .FreeModify<BattleDataWithRewards>(b =>
                {
                    b.dataGroups = new List<Data>[3];
                    b.dataGroups[0] = new List<Data>
                    {
                        CreateCard("latias"),
                        CreateCard("latios")
                    };
                    b.dataGroups[1] = new List<Data> //CHANGE
                    {
                        CreateCharm("CardUpgradeResist"),
                        CreateCharm("CardUpgradeCharged")
                    };
                    b.dataGroups[2] = new List<Data>
                    {
                        CreateBell("BlessingLatias"),
                        CreateBell("BlessingLatios")
                    };

                })
                .AddBattleToLoader(); //Loads and makes it the mandatory first fight

            BattleDataEditor hooh = new BattleDataEditor(mod)
                .Create<BattleDataWithRewards>("Ho-Oh")
                .SetSprite("MapHooh.png", 200)
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
                .FreeModify<BattleDataWithRewards>(b =>
                {
                    b.bonusPulls = 1;
                    b.bonusProfile = new List<int> { 0, 1, 1 }; //1 bonus charm/bell. Never another leader
                    b.dataGroups = new List<Data>[3];
                    b.dataGroups[0] = new List<Data>
                    {
                        CreateCard("raikou"),
                        CreateCard("entei"),
                        CreateCard("suicune"),
                        CreateCard("hooh")
                    };
                    b.dataGroups[1] = new List<Data> //CHANGE
                    {
                        CreateCharm("CardUpgradeConduit"),
                        CreateCharm("CardUpgradeBackBurn"),
                        CreateCharm("CardUpgradeJuice"),
                        CreateCharm("CardUpgradeSacredAsh")
                    };
                    b.dataGroups[2] = new List<Data>
                    {
                        CreateBell("BlessingSpicune"),
                        CreateBell("BlessingRaikou"),
                        CreateBell("BlessingEntei"),
                        CreateBell("BlessingHooh")
                    };

                })
                .AddBattleToLoader(); //Loads and makes it the mandatory first fight

            battleList["Darkrai"] = "darkraiEvent";
            battleList["Lati Twins"] = "latiEvent";
            battleList["Ho-Oh"] = "hoohEvent";
        }

        private void RollForEvent(ref string[] lines)
        {
            CampaignNodeTypeBattle node = Pokefrost.instance.Get<CampaignNodeType>("specialBattle") as CampaignNodeTypeBattle;
            if (node != null)
            {
                node.isBattle = false;
            }
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

        public static BossRewardDataCard.Data CreateCard(string cardName)
        {
            return new BossRewardDataCard.Data
            {
                type = BossRewardData.Type.Crown,
                cardDataName = cardName
            };
        }

        public static BossRewardDataRandomCharm.Data CreateCharm(string upgradeName)
        {
            return new BossRewardDataRandomCharm.Data
            {
                type = BossRewardData.Type.Charm,
                upgradeName = Pokefrost.instance.GUID + "." + upgradeName
            };
        }

        public static BossRewardDataModifier.Data CreateBell(string modifierName)
        {
            return new BossRewardDataModifier.Data
            {
                type = BossRewardData.Type.Modifier,
                modifierName = Pokefrost.instance.GUID + "." + modifierName
            };
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

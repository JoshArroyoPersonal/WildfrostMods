using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class CampaignNodeTypeSpecialBattle : CampaignNodeTypeBattle
    {
        public override IEnumerator Run(CampaignNode node)
        {
            bool flag = false;
            QuestSystem theQuest = null;
            foreach (QuestSystem quest in Campaign.instance.systems.GetComponents<QuestSystem>())
            {
                if (quest.CheckConditions())
                {
                    flag = true;
                    theQuest = quest;
                    break;
                }
            }
            if (!flag)
            {
                foreach (QuestSystem quest in Campaign.instance.gameObject.GetComponents<QuestSystem>())
                {
                    if (quest.CheckConditions())
                    {
                        flag = true;
                        theQuest = quest;
                        break;
                    }
                }
            }
            if (flag)
            {
                Debug.Log("[Pokefrost] Quest succeeded! Entering bonus battle...");
                theQuest.QuestBattleStart();
                yield return base.Run(node);
                theQuest.QuestBattleFinish();
            }
            else
            {
                Debug.Log("[Pokefrost] Quest failed. Skipping bonus battle...");
                Ext.PopupText(References.Map.FindNode(Campaign.FindCharacterNode(References.Player)).transform, "Conditions Not Met!", false);
                yield return Sequences.Wait(1f);
                node.SetCleared();
                References.Map.Continue();
            }
        }


        public override IEnumerator SetUp(CampaignNode node)
        {
            BattleData battle = null;
            foreach(string battleName in EventBattleManager.battleList.Keys.InRandomOrder())
            {
                battle = Pokefrost.instance.Get<BattleData>(battleName);
                if (battle != null)
                {
                    EventBattleManager.battleChosen = battleName;
                    break;
                }
            }
            node.data = new Dictionary<string, object>
            {
                ["battle"] = battle.name,
                ["waves"] = battle.generationScript.Run(battle, 100)
            };
            isBattle = true;
            
            yield break;
        }
    }
}

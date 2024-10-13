using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EBM = Pokefrost.EventBattleManager;

namespace Pokefrost
{
    internal class CampaignNodeTypeSpecialBattle : CampaignNodeTypeBattle
    {
        public override IEnumerator Run(CampaignNode node)
        {
            string failureText = "Conditions not met!";
            bool flag = false;
            QuestSystem theQuest = null;
            foreach (QuestSystem quest in Campaign.instance.systems.GetComponents<QuestSystem>())
            {
                if (quest.CheckConditions(out failureText))
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
                    if (quest.CheckConditions(out failureText))
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
                Ext.PopupText(References.Map.FindNode(Campaign.FindCharacterNode(References.Player)).transform, failureText, false);
                yield return Sequences.Wait(1f);
                node.SetCleared();
                References.Map.Continue();
            }
        }

        internal virtual BattleData SelectBattle()
        {
            BattleData battle;
            if (EBM.battleList.ContainsKey(EBM.forceBattle))
            {
                EBM.battleChosen = EBM.forceBattle;
                return Pokefrost.instance.Get<BattleData>(EBM.forceBattle);
            }
            else
            {
                foreach (string battleName in EBM.battleList.Keys.InRandomOrder())
                {
                    battle = Pokefrost.instance.Get<BattleData>(battleName);
                    if (battle != null)
                    {
                        EBM.battleChosen = battleName;
                        return battle;
                    }
                }
            }
            throw new Exception("No Event Battle Found");
        }


        public override IEnumerator SetUp(CampaignNode node)
        {
            BattleData battle = SelectBattle();
            
            foreach(string battleName in EBM.battleList.Keys.InRandomOrder())
            {
                battle = Pokefrost.instance.Get<BattleData>(battleName);
                if (battle != null)
                {
                    EBM.battleChosen = battleName;
                    break;
                }
            }
            node.data = new Dictionary<string, object>
            {
                ["battle"] = battle.name,
                ["waves"] = battle.generationScript.Run(battle, 100)
            };
            isBattle = true;

            if (battle is BattleDataWithRewards b)
            {
                b.AddRewards(node);
            }
            
            yield break;
        }
    }
}

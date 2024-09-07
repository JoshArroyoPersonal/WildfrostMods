using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class CampaignNodeTypeSpecialBattle : CampaignNodeTypeBattle
    {
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

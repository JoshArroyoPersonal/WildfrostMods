using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    public class BattleDataWithRewards : BattleData
    {
        public List<BossRewardData.Data>[] dataGroups;
        public int canTake = 2;

        public virtual void AddRewards(CampaignNode node)
        {
            List<BossRewardData.Data> rewards = new List<BossRewardData.Data>();
            foreach (var group in dataGroups)
            {
                rewards.Add(group.RandomItem());
            }

            node.data.Add("rewards", new CampaignNodeTypeBoss.RewardData
            {
                rewards = rewards,
                canTake = canTake
            });
        }
    }
}

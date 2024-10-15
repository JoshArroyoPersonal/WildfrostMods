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
        public int bonusPulls = 0;
        public List<int> minPulls;
        public List<int> bonusProfile;

        public virtual void AddRewards(CampaignNode node)
        {
            List<BossRewardData.Data> rewards = new List<BossRewardData.Data>();

            List<int> profile = DetermineProfile();
            for (int i = 0; i < dataGroups.Length; i++)
            {
                rewards.AddRange(dataGroups[i].InRandomOrder().Take(profile[i]));
            }

            node.data.Add("rewards", new CampaignNodeTypeBoss.RewardData
            {
                rewards = rewards,
                canTake = canTake
            });
        }

        public virtual List<int> DetermineProfile()
        {
            List<int> profile;
            if (minPulls == null)
            {
                profile = DefaultMinPulls();
            }
            else
            {
                profile = minPulls.Clone();
            }
            if (bonusPulls > 0 && bonusProfile != null)
            {
                int sum = 0;
                foreach(int count in bonusProfile)
                {
                    sum += count;
                }

                int temp = bonusPulls;
                while(temp > 0)
                {
                    int rand = Dead.Random.Range(1,sum);
                    for(int i =0; i<bonusProfile.Count;i++)
                    {
                        rand -= bonusProfile[i];
                        if (rand <= 0)
                        {
                            profile[i]++;
                            temp--;
                            break;
                        }
                    }
                }
            }
            return profile;
        }

        public List<int> DefaultMinPulls() => dataGroups.Select(g => 1).ToList();
    }
}

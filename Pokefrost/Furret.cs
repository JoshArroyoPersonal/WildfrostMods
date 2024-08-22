using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class Furret
    {

        [HarmonyPatch(typeof(InjuredCompanionEventSystem), "CampaignLoadPreset")]
        public class FurretCompanionEventSystem1
        {

            internal static bool Prefix()
            {
                return true;
            }

            /*public static void CampaignLoadPreset(ref string[] lines)
            {
                if (Campaign.Data.GameMode.mainGameMode && !Campaign.Data.GameMode.tutorialRun)
                {
                    RunHistory mostRecentRun = GetMostRecentRun();
                    if (mostRecentRun != null && mostRecentRun.result == Campaign.Result.Lose && HasEligibleCompanion(mostRecentRun))
                    {
                        int campaignInsertPosition = GetCampaignInsertPosition(mostRecentRun);
                        lines[0] = lines[0].Insert(campaignInsertPosition, "#");
                        lines[1] = lines[1].Insert(campaignInsertPosition, " ");
                        lines[2] = lines[2].Insert(campaignInsertPosition, lines[2][campaignInsertPosition - 1].ToString());
                        lines[3] = lines[3].Insert(campaignInsertPosition, lines[3][campaignInsertPosition - 1].ToString());
                    }
                }
            }*/
        }


    }
}

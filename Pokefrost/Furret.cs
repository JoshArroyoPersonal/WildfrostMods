using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
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

            internal static bool Prefix(ref string[] lines)
            {
                string fileName = Path.Combine(Pokefrost.instance.ModDirectory, "furret.txt");

                if (Campaign.Data.GameMode.mainGameMode && !Campaign.Data.GameMode.tutorialRun && System.IO.File.Exists(fileName))
                {

                    string[] furretData = System.IO.File.ReadAllLines(fileName);

                    if (furretData.Length > 3)
                    {
                        int num;
                        if(int.TryParse(furretData[3], out int result))
                        {
                            num = result;
                        }
                        else
                        {
                            return true;
                        }
                        int campaignInsertPosition = ((num >= 6) ? 23 : ((num >= 3) ? 11 : 2)); ;
                        lines[0] = lines[0].Insert(campaignInsertPosition, "#");
                        lines[1] = lines[1].Insert(campaignInsertPosition, " ");
                        lines[2] = lines[2].Insert(campaignInsertPosition, lines[2][campaignInsertPosition - 1].ToString());
                        lines[3] = lines[3].Insert(campaignInsertPosition, lines[3][campaignInsertPosition - 1].ToString());
                        return false;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(InjuredCompanionEventSystem), "GetEligibleCompanions")]

        public class FurretCompanionEventSystem2
        {
            internal static bool Prefix(ref List<CardSaveData> __result)
            {
                string fileName = Path.Combine(Pokefrost.instance.ModDirectory, "furret.txt");
                if (System.IO.File.Exists(fileName))
                {
                    UnityEngine.Debug.Log("[Furret] File Exists");
                    string[] furretData = System.IO.File.ReadAllLines(fileName);
                    UnityEngine.Debug.Log("[Furret] Got Lines");
                    if (furretData.Length > 3)
                    {
                        UnityEngine.Debug.Log("[Furret] If is True");
                        CardData furret = Pokefrost.instance.Get<CardData>("furret").Clone();
                        UnityEngine.Debug.Log("[Furret] Got Card");
                        furret.forceTitle = furretData[0].Trim();
                        furret.startWithEffects = new CardData.StatusEffectStacks[0];
                        UnityEngine.Debug.Log("[Furret] Removed Escape");
                        for (int i = 4; i < furretData.Length; i++)
                        {
                            CardUpgradeData upgrade = Pokefrost.instance.Get<CardUpgradeData>(furretData[i].Trim());
                            if (upgrade != null && upgrade.CanAssign(furret))
                            {
                                upgrade.Clone().Assign(furret);
                            }
                        }

                        CardUpgradeData removeCharmLimit = Pokefrost.instance.Get<CardUpgradeData>("CardUpgradeRemoveCharmLimit");
                        if (removeCharmLimit.CanAssign(furret))
                        { 
                            removeCharmLimit.Clone().Assign(furret);
                        }

                        List<CardUpgradeData> options = AddressableLoader.GetGroup<CardUpgradeData>("CardUpgradeData").Clone();
                        int bonus = Dead.Random.Range(3,10);
                        UnityEngine.Debug.Log("[Pokefrost] Furret rolled " + bonus.ToString() + " charms");

                        for (int i = 0; i < bonus; i++)
                        {
                            var r = Dead.Random.Range(0, options.Count);
                            CardUpgradeData charm = options[r].Clone();
                            if (charm.CanAssign(furret) && charm.tier > 0 && charm.name != "CardUpgradeMuncher")
                            {
                                charm.Assign(furret);
                            }
                        }



                        UnityEngine.Debug.Log("[Furret] Right Before Save");
                        __result = new List<CardSaveData> { furret.Save() };
                        System.IO.File.Delete(fileName);
                        return false;


                    }
                    else
                    {
                        return true;
                    }

                }

                return true;

            }

        }


    }
}

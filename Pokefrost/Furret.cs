using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ADD = Pokefrost.AddressableExtMethods;

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
                    Debug.Log("[Pokefrost] Adding Furret To Injured Companion Event");
                    string[] furretData = System.IO.File.ReadAllLines(fileName);

                    if (furretData.Length > 3)
                    {
                        int num;
                        if (int.TryParse(furretData[2], out int result))
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
                    string[] furretData = System.IO.File.ReadAllLines(fileName);

                    if (furretData.Length > 4)
                    {
                        CardData furret = Pokefrost.instance.Get<CardData>("furret").Clone();
                        furret.forceTitle = furretData[0].Trim();
                        furret.startWithEffects = new CardData.StatusEffectStacks[0];

                        for (int i = 5; i < furretData.Length; i++)
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

                        List<CardUpgradeData> options = AddressableLoader.GetGroup<CardUpgradeData>("CardUpgradeData");
                        int bonus = Dead.Random.Range(20, 25);
                        UnityEngine.Debug.Log("[Pokefrost] Furret rolled " + bonus.ToString() + " charms");

                        for (int i = 0; i < bonus; i++)
                        {
                            var r = Dead.Random.Range(0, options.Count-1);
                            CardUpgradeData charm = options[r].Clone();
                            if (charm.CanAssign(furret) && charm.tier > 0 && charm.name != "CardUpgradeMuncher")
                            {
                                charm.Assign(furret);
                            }
                        }

                        if (furretData[4].Trim() == "Hasty")
                        {
                            Pokefrost.instance.Shinify(furret);
                        }

                        __result = new List<CardSaveData> { furret.Save() };
                        
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


        [HarmonyPatch(typeof(EventRoutineInjuredCompanion), "Populate")]

        public class FurretCompanionEventSystem3
        {
            internal static IEnumerator Postfix(IEnumerator __result)
            {
                yield return __result;

                string fileName = Path.Combine(Pokefrost.instance.ModDirectory, "furret.txt");

                if (System.IO.File.Exists(fileName))
                {
                    string[] furretData = System.IO.File.ReadAllLines(fileName);

                    GameObject furretPanel = UICollector.PullPrefab("Box", "FurretBox", GameObject.Find("Canvas/SafeArea/EventManager/Event-InjuredCompanion(Clone)/EnterTweener/Zoomer/Inspect Companion/"));
                    furretPanel.GetComponent<RectTransform>().sizeDelta = new Vector2 (5f, 3f);
                    furretPanel.transform.localPosition = new Vector3 (5.3f, 1.9f, 0);
                    //Sprite sprite = Pokefrost.instance.ImagePath("FurretPanel.png").ToSprite();
                    Sprite sprite = ADD.ASprite("FurretPanel");
                    Debug.Log("[Pokefrost] Made Sprite");
                    Image image = furretPanel.GetComponent<Image>();
                    if (image == null) { Debug.Log("[Pokefrost] Image null"); }
                    Debug.Log("[Pokefrost] Made Image");
                    image.sprite = sprite;
                    
                    GameObject furretText = new GameObject("Paneltext");
                    furretText.transform.SetParent(furretPanel.transform, false);
                    TextMeshProUGUI text = furretText.AddComponent<TextMeshProUGUI>();
                    furretText.GetComponent<RectTransform>().sizeDelta = new Vector2(4.5f, 2);
                    text.alignment = TextAlignmentOptions.Top;
                    text.text = "<color=#FF8>" + furretData[1] + "'s</color> <color=#940>" + furretData[0]+"</color> is back!\n<color=#888><size=0.2>" + furretData[0] + " has been missing since " + furretData[3] + "</size></color>";
                    
                    
                    System.IO.File.Delete(fileName);
                }

            }


        }

    }
}

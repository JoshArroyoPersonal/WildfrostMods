using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using WildfrostHopeMod;

namespace Undiscovered
{
    public class Undiscovered : WildfrostMod
    {
        public Undiscovered(string modDirectory)
          : base(modDirectory)
        {
            instance = this;
        }

        [ConfigManagerTitle("Strength")]
        [ConfigManagerDesc("Higher the Strength the more likely affected cards will appear")]
        [ConfigSlider(0, 100)]
        [ConfigItem(70, "", "Strength")]
        public int strength = 70;

        [ConfigManagerTitle("Type Affected")]
        [ConfigManagerDesc("Determines which cards appear more often")]
        [ConfigOptions("Undiscovered", "Modded (Not Charms)", "Modded", "Not Golden", "Not Chiseled")]
        [ConfigItem("Undiscovered", "", "Type")]
        public string boostoption = "Undiscovered";

        public static Undiscovered instance;

        public override string GUID => "websiteofsites.wildfrost.undiscovered";
        public override string[] Depends => new string[] { };
        public override string Title => "Increased Odds";
        public override string Description => "Increases the odds of you finding undiscovered or modded cards. Use Mod Config Manager to adjust what gets their odds" +
            " increased and by how much. Strength determines the odds with 0 leaving the odds unchanged and 100 guaranteeing you" +
            " see undiscovered/modded cards until you have seen them all.";
    }


    [HarmonyPatch(typeof(CharacterRewards.Pool), "Populate")]
    internal static class ChangeRandom
    {
        static bool Prefix(CharacterRewards.Pool __instance) 
        {
            
            List<string> discovered = SaveSystem.LoadProgressData<List<string>>("cardsDiscovered");

            List<DataFile> rlist = null;

            switch (Undiscovered.instance.boostoption)
            {
                case "Undiscovered": 
                    rlist = __instance.list.OrderBy((a) => FakeRandom(a.name, discovered, 0f, 1f - Undiscovered.instance.strength / 101f, 1f)).ToList();
                    break;

                case "Modded":
                    rlist = __instance.list.OrderBy((a) => ModRandom(a, 0f, 1f - Undiscovered.instance.strength / 101f, 1f)).ToList();
                    break;

                case "Modded (Not Charms)":
                    rlist = __instance.list.OrderBy((a) => ModRandom(a, 0f, 1f - Undiscovered.instance.strength / 101f, 1f, true)).ToList();
                    break;

                case "Not Golden":
                    rlist = __instance.list.OrderBy((a) => GoldRandom(a, 0f, 1f - Undiscovered.instance.strength / 101f, 1f, 2)).ToList();
                    break;

                case "Not Chiseled":
                    rlist = __instance.list.OrderBy((a) => GoldRandom(a, 0f, 1f - Undiscovered.instance.strength / 101f, 1f, 1)).ToList();
                    break;
            }
            __instance.current.AddRange(rlist);
            return false;
        }

        static float FakeRandom(string item, List<string> buried, float min, float mid, float max)
        {
            if (buried.Contains(item))
            {
                return UnityEngine.Random.Range(min, max);
            }
            else
            {
                return UnityEngine.Random.Range(min, mid);
            }
        }

        static float ModRandom(DataFile item, float min, float mid, float max, bool charm=false)
        {
            if (item.ModAdded == null || (charm && item is CardUpgradeData))
            {
                return UnityEngine.Random.Range(min, max);
            }
            else
            {
                return UnityEngine.Random.Range(min, mid);
            }
        }

        static float GoldRandom(DataFile item, float min, float mid, float max, int frame)
        {
            if (CardFramesSystem.GetFrameLevel(item.name) >= frame)
            {
                return UnityEngine.Random.Range(min, max);
            }
            else
            {
                return UnityEngine.Random.Range(min, mid);
            }
        }



    }

}

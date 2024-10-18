using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public static class EyeDataAdder
    {
        private static Dictionary<string, (float, float, float, float, float)[]> eyeDictionary = new Dictionary<string, (float, float, float, float, float)[]>
    {
        {
            "websiteofsites.wildfrost.pokefrost.alolansandslash",
            new(float, float, float, float, float)[2]
            {
                (0.6f, 0.8f, 0.8f, 1.2f, 45f),
                (1.1f, 0.8f, 0.4f, 0.7f, -10f)
            }
        }
    };

        public static void Eyes()
        {
            //WARNING: The EyeData will NOT be removed upon unload. Call Eyes() underneath CreateModAssets() in the Load method. 
            List<EyeData> list = new List<EyeData>()
            {
                //Put the output code here!
                Eyes("websiteofsites.wildfrost.pokefrost.abomasnow", (0.36f,1.90f,1.20f,1.20f,0f), (-0.08f,1.90f,1.20f,1.20f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.absol", (-0.32f,1.47f,0.80f,0.80f,0f), (-0.67f,1.46f,0.80f,0.80f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.alolanmarowak", (0.50f,0.75f,0.70f,1.30f,45f)),
                Eyes("websiteofsites.wildfrost.pokefrost.alolansandslash", (1.09f,0.79f,0.50f,1.30f,340f), (0.62f,0.77f,0.80f,1.50f,40f)),
            };

            AddressableLoader.AddRangeToGroup("EyeData", list);
        }

        public static EyeData Eyes(string cardName, params (float,float,float,float,float)[] data)
        {
            EyeData eyeData = ScriptableObject.CreateInstance<EyeData>();
            eyeData.cardData = cardName;
            eyeData.name = eyeData.cardData + "_EyeData";
            eyeData.eyes = data.Select((e) => new EyeData.Eye
            {
                position = new Vector2(e.Item1, e.Item2),
                scale = new Vector2(e.Item3, e.Item4),
                rotation = e.Item5
            }).ToArray();

            return eyeData;
        }
    }
}

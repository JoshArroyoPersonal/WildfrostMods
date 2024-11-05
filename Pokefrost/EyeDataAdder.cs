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
                Eyes("websiteofsites.wildfrost.pokefrost.aggron", (0.67f,1.94f,0.70f,0.70f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.alolanmarowak", (0.50f,0.75f,0.70f,1.30f,45f)),
                Eyes("websiteofsites.wildfrost.pokefrost.alolansandslash", (1.09f,0.79f,0.50f,1.30f,340f), (0.62f,0.77f,0.80f,1.50f,40f)),
                Eyes("websiteofsites.wildfrost.pokefrost.ambipom", (-0.25f,1.17f,0.70f,0.90f,0f), (0.22f,1.16f,0.70f,0.90f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.bastiodon", (0.28f,0.59f,1.00f,1.00f,55f), (1.06f,0.80f,0.80f,0.80f,55f)),
                Eyes("websiteofsites.wildfrost.pokefrost.chandelure", (-0.03f,0.82f,1.00f,1.00f,0f), (0.58f,0.71f,0.40f,0.80f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.chimecho", (0.22f,1.84f,0.60f,0.60f,0f), (0.57f,1.74f,0.50f,0.50f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.cradily", (0.01f,1.11f,1.00f,1.60f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.crustle", (0.39f,0.41f,0.60f,0.90f,10f), (0.75f,0.39f,0.50f,0.90f,345f)),
                Eyes("websiteofsites.wildfrost.pokefrost.dusclops", (0.44f,1.57f,1.30f,1.30f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.electrode", (0.04f,1.11f,1.00f,1.10f,0f), (0.97f,1.18f,1.00f,1.10f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.empoleon", (-0.21f,1.90f,0.50f,0.50f,0f), (0.06f,1.90f,0.40f,0.40f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.espeon", (0.57f,1.44f,1.00f,1.00f,0f), (1.04f,1.65f,0.80f,0.80f,335f)),
                Eyes("websiteofsites.wildfrost.pokefrost.farfetchd", (-0.17f,1.30f,0.70f,1.00f,0f), (0.20f,1.26f,0.50f,0.90f,350f)),
                Eyes("websiteofsites.wildfrost.pokefrost.flareon", (0.75f,0.57f,0.90f,0.90f,350f), (1.14f,0.65f,0.30f,0.60f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.furret", (-0.19f,1.89f,0.60f,0.60f,0f), (0.18f,1.95f,0.50f,0.50f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.gallade", (-0.08f,1.66f,0.80f,0.80f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.garbodor", (-0.43f,1.49f,0.90f,0.90f,0f), (0.33f,1.57f,0.80f,0.90f,20f)),
                Eyes("websiteofsites.wildfrost.pokefrost.gardevoir", (0.63f,1.92f,0.80f,0.80f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.gengar", (-0.15f,1.13f,1.30f,1.30f,0f), (0.53f,1.31f,1.20f,1.20f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.glaceon", (0.62f,1.13f,0.90f,0.90f,0f), (0.97f,1.22f,0.60f,0.80f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.goomy", (0.36f,0.94f,1.00f,1.00f,0f), (0.69f,0.90f,1.00f,1.00f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.gyarados", (1.04f,1.17f,0.80f,1.00f,0f), (1.31f,1.20f,0.60f,1.00f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.hariyama", (0.08f,1.63f,1.00f,1.20f,0f), (0.55f,1.73f,0.80f,1.10f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.hippowdon", (-0.15f,1.25f,0.70f,1.20f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.hisuiansneasel", (0.22f,1.26f,1.20f,1.20f,0f), (0.78f,1.31f,0.90f,0.90f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.honchkrow", (-0.19f,1.35f,1.00f,1.00f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.jolteon", (0.93f,1.23f,1.20f,1.20f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.kingambit", (0.03f,1.00f,0.60f,0.60f,0f), (0.23f,1.00f,0.60f,0.60f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.kingdra", (0.53f,1.42f,1.20f,1.20f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.klefki", (-0.22f,0.69f,0.60f,0.80f,0f), (0.06f,0.71f,0.60f,0.80f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.leafeon", (0.56f,1.16f,1.00f,1.00f,0f), (0.97f,1.17f,1.00f,1.00f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.lickilicky", (-0.14f,2.02f,1.10f,1.10f,0f), (0.49f,2.03f,1.10f,1.10f,0f)),
                Eyes("websiteofsites.wildfrost.pokefrost.ludicolo", (-0.07f,1.41f,0.90f,1.20f,15f), (0.26f,1.48f,0.90f,1.20f,10f)),
                Eyes("websiteofsites.wildfrost.pokefrost.lumineon", (0.59f,0.57f,1.00f,1.40f,0f)),
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

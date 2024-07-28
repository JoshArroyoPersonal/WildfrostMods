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

        public static List<EyeData> Eyes()
        {
            List<EyeData> list = new List<EyeData>();

            foreach (KeyValuePair<string, (float, float, float, float, float)[]> item in eyeDictionary)
            {
                string key = item.Key;
                EyeData eyeData = ScriptableObject.CreateInstance<EyeData>();
                eyeData.cardData = key;
                eyeData.name = eyeData.cardData + "EyeData";
                List<(float, float, float, float, float)> list2 = item.Value.ToList();
                foreach (var item2 in list2)
                {
                    eyeData.eyes = eyeData.eyes.AddItem(new EyeData.Eye
                    {
                        scale = new Vector2(item2.Item3, item2.Item4),
                        position = new Vector2(item2.Item1, item2.Item2),
                        rotation = item2.Item5
                    }).ToArray();
                }

                list.Add(eyeData);
            }

            return list;
        }
    }
}

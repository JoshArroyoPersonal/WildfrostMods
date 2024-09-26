using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal static class EventSaveSystem
    {
        private static Dictionary<string, int> eventProgress;
        private static bool FileName(CampaignData data, out string fileName)
        {
            fileName = null;    
            string gameMode = data?.GameMode?.name;
            string profile = SaveSystem.GetProfile();
            if (profile == null || gameMode == null)
            {
                return false;
            }
            fileName = Path.Combine(Pokefrost.instance.ModDirectory, $"{profile}_{gameMode}_quest.txt");
            return true;
        }

        private static void LoadProgress(CampaignData data)
        {
            eventProgress = new Dictionary<string, int>();
            try
            {
                if (!FileName(data, out string fileName))
                {
                    return;
                }

                if (File.Exists(fileName))
                {
                    string[] progress = System.IO.File.ReadAllLines(fileName);
                    if (int.TryParse(progress[0], out int value) && value == data.Seed)
                    {
                        for (int i = 1; i < progress.Length; i++)
                        {
                            string[] keyValue = progress[i].Split(' ');
                            eventProgress[keyValue[0]] = int.Parse(keyValue[1]);
                        }
                    }
                    else
                    {
                        File.Delete(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                eventProgress = new Dictionary<string, int>();
                UnityEngine.Debug.Log(ex.Message);
            }
        }

        private static void SaveProgress(CampaignData data)
        {
            if (eventProgress == null  || eventProgress.Count == 0)
            {
                return;
            }

            if (!FileName(data, out string fileName))
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(data.Seed.ToString());
            foreach (string key in eventProgress.Keys)
            {
                sb.AppendLine($"{key} {eventProgress[key]}");
            }
            System.IO.File.WriteAllText(fileName, sb.ToString());
        }

        public static void Add(string key, int value)
        {
            if (eventProgress == null)
            {
                LoadProgress(Campaign.Data);
            }

            eventProgress[key] = value;
            SaveProgress(Campaign.Data);
        }

        public static int Get(string key)
        {
            if (eventProgress == null)
            {
                LoadProgress(Campaign.Data);
            }

            if (eventProgress.TryGetValue(key, out int value))
            {
                return value;
            }
            return -1;
        }
    }
}

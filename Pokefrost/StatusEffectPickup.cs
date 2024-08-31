using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public class StatusEffectPickup : StatusEffectData
    {
        public string[] rewardPoolNames;
        public Func<CardData, bool> constraint;
        public int cap = 10;
        public string text = "{0} Found A Card";
        public string cappedText = "{0} Found A Fortune!";

        public static List<ulong> BlockingQueue = new List<ulong>();

        public override void Init()
        {
            Events.OnBattleEnd += CheckPickup;
            base.Init();
        }

        public void OnDestroy()
        {
            Events.OnBattleEnd -= CheckPickup;
        }

        public virtual void CheckPickup()
        {
            if (target.IsAliveAndExists())
            {
                BlockingQueue.Add(target.data.id);
                Events.PreBattleEnd += Pickup;
                Events.PostBattle += RemovePickup;
            }
        }

        public virtual async Task Pickup()
        {  
            target.StartCoroutine(Run());
        }

        public void RemovePickup(CampaignNode _)
        {
            Events.PreBattleEnd -= Pickup;
            Events.PostBattle -= RemovePickup;
        }

        public virtual IEnumerator Run()
        {
            Debug.Log($"[Pokefrost] Queued {target.data.id}");
            yield return new WaitUntil(() => BlockingQueue[0] == target.data.id);
            Debug.Log($"[Pokefrost] Starting {target.data.id}");
            string titleText = (cap <= GetAmount()) ? cappedText : text;
            PickupRoutine.Create(titleText.Replace("{0}",target.data.title));
            RewardPool[] pools = GetPools();
            constraint = AddressableLoader.Get<StatusEffectPickup>("StatusEffectData", name).constraint;
            yield return PickupRoutine.AddRandomCards(Math.Min(15, GetAmount()), pools, constraint);
            yield return PickupRoutine.Run();
            Debug.Log($"[Pokefrost] Ending {target.data.id}");
            yield return null;
            BlockingQueue.Remove(target.data.id);
            Debug.Log($"[Pokefrost] Ended {target.data.id}");

        }

        protected RewardPool[] ConvertToPools()
        {
            List<RewardPool> pools = new List<RewardPool>();
            foreach (string s in rewardPoolNames)
            {
                RewardPool r = Extensions.GetRewardPool(s);
                if (r != null)
                {
                    pools.Add(r);
                }
            }
            return pools.ToArray();
        }

        protected RewardPool[] GetPools()
        {
            List<ClassData> tribes = AddressableLoader.GetGroup<ClassData>("ClassData");
            ClassData tribe = tribes[0];
            string tribeName = References.Player.name;
            Debug.Log($"[Pokefrost] {tribeName}");
            foreach(ClassData t in tribes)
            {
                if (tribeName.ToLower().Contains(t.name.ToLower()))
                {
                    tribe = t;
                    break;
                }
            }

            return tribe.rewardPools.Where((r) => r != null && r.type == "Items" && !r.isGeneralPool).ToArray();
        }
    }
}

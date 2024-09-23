using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{

    //Also see
    //StatusEffectInstanceSummonLastRecycled in StatusEffectSummonCustom.cs

    internal class StatusEffectAllCardsAreRecycled : StatusEffectData
    {
        public bool added = false;
        public override bool RunBeginEvent()
        {
            Activate();
            return false;
        }

        public void OnDestroy()
        {
            Deactivate();
        }

        public override bool RunEffectBonusChangedEvent()
        {
            if (GetAmount() == 0)
            {
                Deactivate();
            }
            else
            {
                Activate();
            }
            return false;
        }

        protected void Activate()
        {
            if (!added)
            {
                PatchRecycle.active++;
                added = true;
            }
        }

        protected void Deactivate()
        {
            if (added)
            {
                PatchRecycle.active--;
                added = false;
            }
        }

        [HarmonyPatch(typeof(StatusEffectRecycle), "GetTargets")]
        internal class PatchRecycle
        {
            public static int active = 0;
            public static bool Active => active != 0;

            public static CardData lastDestroyed = null;
            public static int node = -1;
            static void Prefix(StatusEffectRecycle __instance, ref int requiredAmount, out List<Entity> __state)
            {
                int count = 0;
                __state = new List<Entity>();

                if (!Active) { return; }

                //for (int i = References.Player.handContainer.Count-1; i >= 0; i--)
                foreach(Entity entity in References.Player.handContainer)
                {
                    if (entity == __instance.target)
                    {
                        continue;
                    }

                    if (entity.name == __instance.cardToRecycle)
                    {
                        count++;
                    }
                    else
                    {
                        count++;
                        __state.Add(entity);
                    }
                }

                if (count >= requiredAmount)
                {
                    int junkCount = count - __state.Count;
                    int itemCount = Math.Max(0, requiredAmount - junkCount);
                    requiredAmount = Math.Min(junkCount, requiredAmount);
                    __state = __state.GetRange(0, itemCount);
                }
            }

            static void Postfix(ref bool __result, StatusEffectRecycle __instance, int requiredAmount, List<Entity> __state)
            {
                if (!__result && requiredAmount == 0) //The original method needs to find a junk in order to return true
                {
                    __result = true;
                }
                __instance.toDestroy.AddRange(__state);
                if (__instance.toDestroy.Count != 0)
                {
                    lastDestroyed = __instance.toDestroy[__instance.toDestroy.Count - 1].data;
                    node = Campaign.FindCharacterNode(References.Player).id;
                }
                
            }
        }

        [HarmonyPatch(typeof(DestroyTargetSystem), "ShowTargets")]
        internal class PatchShowRecycleTargets
        {
            static void Prefix(DestroyTargetSystem __instance, Entity entity)
            {
                if (!PatchRecycle.Active || entity.silenced) { return; }

                foreach(StatusEffectData effect in entity.statusEffects)
                {
                    if (effect is StatusEffectRecycle recycle)
                    {  
                        int junkCount = References.Player.handContainer.Where(h => h.name == recycle.cardToRecycle).Count();
                        int num = recycle.GetAmount() - junkCount;
                        if (num <= 0)
                        {
                            break;
                        }
                        foreach (Entity item in References.Player.handContainer)
                        {
                            if (item.name != recycle.cardToRecycle && item != entity)
                            {
                                __instance.toIndicate.Add(item);
                                if (--num <= 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

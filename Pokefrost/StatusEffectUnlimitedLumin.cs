using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectUnlimitedLumin : StatusEffectData
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
                PatchLumin.active++;
                added = true;
            }
        }

        protected void Deactivate()
        {
            if (added)
            {
                PatchLumin.active--;
                added = false;
            }
        }

        [HarmonyPatch(typeof(StatusEffectLumin), "RunPostApplyStatusEvent")]
        class PatchLumin
        {
            public static int active = 0;
            public static bool Active => active != 0;
            static bool Prefix()
            {
                return !Active;
            }
        }
    }

}

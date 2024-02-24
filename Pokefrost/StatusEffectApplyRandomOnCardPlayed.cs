using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectApplyRandomOnCardPlayed : StatusEffectApplyXOnCardPlayed
    {
        public StatusEffectData[] effectsToapply = new StatusEffectData[0];

        public override void Init()
        {
            base.Init();
            Events.OnActionQueued += DetermineEffect;
        }

        private void DetermineEffect(PlayAction arg)
        {
            int r = UnityEngine.Random.Range(0, effectsToapply.Length);
            effectToApply = effectsToapply[r];
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectInstantSummonCustom : StatusEffectInstantSummon
    {

        public override IEnumerator Process()
        {

            target.data.TryGetCustomData<string>("Future Sight", out string val, "");

            if (!val.IsNullOrEmpty())
            {
                CardData summonCard = Pokefrost.instance.Get<CardData>(val);
                if (summonCard != null)
                {
                    targetSummon.summonCard = summonCard;
                }

            }

            return base.Process();


        }
    }
}

﻿using System;
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


    internal class StatusEffectInstantSummonLuminPart : StatusEffectInstantSummon
    {

        public CardData card1;

        public CardData card2;

        public override IEnumerator Process()
        {

            List<Entity> fulldeck = new List<Entity>();
            fulldeck.AddRange(References.Player.handContainer.ToList());
            fulldeck.AddRange(References.Player.drawContainer.ToList());
            fulldeck.AddRange(References.Player.discardContainer.ToList());

            UnityEngine.Debug.Log("[Pokefrost] Checking deck");

            foreach (Entity entity in fulldeck) 
            {
                if (entity.data.name == card1.name)
                {
                    targetSummon.summonCard = card2;
                    break;
                }
                else if (entity.data.name == card2.name) 
                {
                    targetSummon.summonCard = card1;
                    break;
                }
            }

            return base.Process();


        }
    }



}

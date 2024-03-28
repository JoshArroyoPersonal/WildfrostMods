using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectEvolveNincada : StatusEffectEvolve
    {
        public string shedinjaMask = "websiteofsites.wildfrost.pokefrost.shedinjamask";
        public override void Evolve(WildfrostMod mod, CardData preEvo)
        {
            References.PlayerData.inventory.deck.Add(mod.Get<CardData>(shedinjaMask).Clone());
            base.Evolve(mod, preEvo);
        }
    }

    internal class StatusEffectEvolveCrown : StatusEffectEvolve
    {
        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);
            type = "evolve2";
        }

        public override bool ReadyToEvolve(CardData cardData)
        {
            return cardData.HasCrown;
        }
    }
}

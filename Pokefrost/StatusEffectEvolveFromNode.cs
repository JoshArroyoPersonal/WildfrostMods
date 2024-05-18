using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    public class StatusEffectEvolveFromNode : StatusEffectEvolve
    {
        public string targetNodeName = "Salty Spicelands";
        public bool readyToEvolve = false;

        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);
            type = "evolve3";
        }

        public virtual void NodeVisit(string nodeName)
        {
            if (nodeName == targetNodeName)
            {
                readyToEvolve = true;
            }
        }

        public override bool ReadyToEvolve(CardData cardData)
        {
            return readyToEvolve;
        }
    }

    public class StatusEffectEvolveSlowpoke : StatusEffectEvolveFromNode
    {
        public string evolveUncrowned = "websiteofsites.pokefrost.slowbro";
        public string evolveCrowned = "websiteofsites.pokefrost.slowking";
        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);
            evolutionCardName = "websiteofsites.pokefrost.slowbro";
        }

        public override void Evolve(WildfrostMod mod, CardData preEvo)
        {
            if (preEvo.HasCrown)
            {
                evolutionCardName = evolveCrowned;
                References.Player.data.inventory.upgrades.Add(mod.Get<CardUpgradeData>("CrownSlowking"));
            }
            else
            {
                evolutionCardName = evolveUncrowned;
            }
            base.Evolve(mod, preEvo);
        }
    }
}

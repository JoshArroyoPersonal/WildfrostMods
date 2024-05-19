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

        //This method is effectively ReadyToEvolve
        public virtual bool NodeVisit(string nodeName, CardData cardData)
        {
            if (nodeName == targetNodeName)
            {
                return true;
            }
            return false;
        }

        public override bool ReadyToEvolve(CardData cardData)
        {
            return false;
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

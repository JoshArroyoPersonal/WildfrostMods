using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    public class StatusEffectEvolveFromCardPickup : StatusEffectEvolve
    {
        public virtual string CardName => "";
        public override void Autofill(string n, string descrip, WildfrostMod mod)
        {
            base.Autofill(n, descrip, mod);
            type = "evolve4";
        }

        public virtual bool CardSelected(CardData self, Entity selectedCard)
        {
            return selectedCard.data.name == CardName;
        }

        public static void CheckEvolveFromSelect(Entity entity)
        {
            if (References.Player?.data?.inventory == null) { return; }

            CheckEvolve<StatusEffectEvolveFromCardPickup>(References.PlayerData.inventory.deck, "evolve4", (s, c) => s.CardSelected(c, entity));
            CheckEvolve<StatusEffectEvolveFromCardPickup>(References.PlayerData.inventory.reserve, "evolve4", (s, c) => s.CardSelected(c, entity));

            if (EvolutionPopUp.evolvedPokemonLastBattle.Count > 0)
            {
                References.instance.StartCoroutine(EvolutionPopUp.DelayedRun());
            }
        }
    }

    public class StatusEffectEvolveNatu : StatusEffectEvolveFromCardPickup
    {
        //Does not use CardName

        public override bool CardSelected(CardData self, Entity selectedCard)
        {
            self.TryGetCustomData("Future Sight", out string cardName, CardName);
            return (cardName == selectedCard.data.name);
        }
    }
}

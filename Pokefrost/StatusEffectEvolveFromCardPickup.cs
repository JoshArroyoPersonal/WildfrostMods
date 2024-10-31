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

        public static void CheckEvolveFromSelect(ShopItem item)
        {
            Entity entity = item.GetComponentInChildren<Entity>();
            if (entity != null)
            {
                CheckEvolveFromSelect(entity);
            }
        }

        public static void CheckEvolveFromSelect(Entity entity)
        {
            if (References.Player?.data?.inventory == null) { return; }

            if (entity.data.TryGetCustomData<int>("Future Sight ID", out int value, -1))
            {
                CardScriptForsee.ids.Add(value);
            }

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

        public override void Evolve(WildfrostMod mod, CardData preEvo)
        {
            base.Evolve(mod, preEvo);
            CardData xatu = EvolutionPopUp.pokemonEvolvedIntoLastBattle.Last();
            CardData natu = EvolutionPopUp.evolvedPokemonLastBattle.Last();
            if ( xatu.name.Contains("xatu") && natu.name.Contains("natu"))
            {
                natu.TryGetCustomData("Future Sight", out string cardName, CardName);
                xatu.SetCustomData("Future Sight", cardName);
            }
        }
    }
}

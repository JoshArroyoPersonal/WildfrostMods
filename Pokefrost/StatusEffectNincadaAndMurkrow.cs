﻿using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectEvolveNincada : StatusEffectEvolve
    {
        public string shedinjaMask = "websiteofsites.wildfrost.pokefrost.shedinjamask";

        public override bool ReadyToEvolve(CardData cardData)
        {
            CardData.StatusEffectStacks stack = cardData.startWithEffects.FirstOrDefault((s) => s?.data == this);
            if (stack != null)
            {
                stack.count = Math.Max(0, stack.count - 1);
            }
            return base.ReadyToEvolve(cardData);
        }

        public override void Evolve(WildfrostMod mod, CardData preEvo)
        {
            CardData mask = mod.Get<CardData>(shedinjaMask).Clone();
            References.PlayerData.inventory.deck.Add(mask);
            Card card = CardManager.Get(mask, null, References.Player, false, true);
            Events.InvokeEntityShowUnlocked(card.entity);
            base.Evolve(mod, preEvo);
            EvolutionPopUp.pokemonEvolvedIntoLastBattle.Add(mask);

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

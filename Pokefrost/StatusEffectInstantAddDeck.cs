using Steamworks.Ugc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectInstantAddDeck : StatusEffectInstant
    {
        public CardData card;
        public override IEnumerator Process()
        {
            References.Player.data.inventory.deck.Add(card.Clone());

            yield return base.Process();
        }
    }
}

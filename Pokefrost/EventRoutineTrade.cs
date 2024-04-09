using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class EventRoutineTrade : EventRoutine
    {
        public CardControllerSelectCard cc;
        public CardLane topRow;
        public CardLane bottomRow;

        public bool routineActive = true;

        public override IEnumerator Populate()
        {
            string[] saveCollection = base.data.GetSaveCollection<string>("cards");
            topRow.SetSize(saveCollection.Length, 0.7f);
            bottomRow.SetSize(saveCollection.Length, 0.7f);
            Routine.Clump clump = new Routine.Clump();
            for (int i = 0; i < saveCollection.Length; i++)
            {
                clump.Add(CreateCards(saveCollection[i], cc, bottomRow));
            }

            yield return clump.WaitForEnd();
            bottomRow.SetChildPositions();
        }

        public override IEnumerator Run()
        {
            yield return CreateCards("websiteofsites.wildfrost.pokefrost.nosepass", cc, topRow);
            yield return CreateCards("websiteofsites.wildfrost.pokefrost.goomy", cc, topRow);
            yield return CreateCards("websiteofsites.wildfrost.pokefrost.absol", cc, topRow);
            topRow.SetChildPositions();
            yield return new WaitUntil(() => !routineActive);
        }

        private static IEnumerator CreateCards(string cardName, CardController cardController, CardContainer cardContainer, bool startFlipped = true)
        {
            CardData cardData = AddressableLoader.Get<CardData>("CardData", cardName).Clone();
            Card card = CardManager.Get(cardData, cardController, null, inPlay: false, isPlayerCard: true);
            if (startFlipped)
            {
                card.entity.flipper.FlipDownInstant();
            }

            Debug.Log("Trading!");
            Debug.Log(cardName);
            Debug.Log(cardContainer != null);
            cardContainer.Add(card.entity);
            yield return card.UpdateData();
            if (startFlipped)
            {
                card.entity.flipper.FlipUp(force: true);
            }
        }
    }

    
}

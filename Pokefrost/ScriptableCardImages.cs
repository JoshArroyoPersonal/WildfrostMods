using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ADD = Pokefrost.AddressableExtMethods;

namespace Pokefrost
{
    internal class PalafinHero : ScriptableCardImage
    {
        public Image image;

        bool shiny = false;

        List<Sprite> sprites = new List<Sprite> { ADD.ASprite("palafin"), ADD.ASprite("shiny_palafin"), ADD.ASprite("palafinhero"), ADD.ASprite("shiny_palafinhero") };

        public override void AssignEvent()
        {

            image.sprite = sprites[0];

            foreach (CardData.StatusEffectStacks status in entity.data.startWithEffects)
            {
                if (status.data.type == "shiny")
                {
                    shiny = true;
                    image.sprite = sprites[1];
                    break;
                }
            }

            base.AssignEvent();
        }


        public override void UpdateEvent()
        {

            if (entity.traits.FirstOrDefault(t => t.data.name == "Hero") != null)
            {
                if (shiny) 
                {
                    image.sprite = sprites[3];
                }
                else
                {
                    image.sprite = sprites[2];
                }
            }

            base.UpdateEvent();
        }

    }
}

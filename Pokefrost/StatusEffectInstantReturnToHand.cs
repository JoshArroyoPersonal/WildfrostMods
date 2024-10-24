using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Localization.Tables;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectInstantReturnToHand : StatusEffectInstant
    {

        public static readonly string Key_Leader = "websiteofsites.wildfrost.pokefrost.leader";

        [PokeLocalizer]
        public static void DefineStrings()
        {
            StringTable tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
            tooltips.SetString(Key_Leader, "Leader Cannot Be In Hand!");
        }

        public virtual void PopupText(Entity entity, string s)
        {
            NoTargetTextSystem noText = GameSystem.FindObjectOfType<NoTargetTextSystem>();
            if (noText != null)
            {
                TMP_Text textElement = noText.textElement;
                StringTable tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
                textElement.text = tooltips.GetString(s).GetLocalizedString();
                noText.PopText(entity.transform.position);
            }
        }

        public override IEnumerator Process()
        {
            if (target.owner != References.Player)
            {
                yield return base.Process();
                yield break;
            }

            if (target.data.cardType.name == "Leader")
            {
                PopupText(target, Key_Leader);
            }

            else
            {
                CardContainer hand = References.Player.handContainer;

                if (hand != null)
                {
                    yield return Sequences.CardMove(target, new CardContainer[] { hand });
                }
            }

            yield return base.Process();
        }


    }
}

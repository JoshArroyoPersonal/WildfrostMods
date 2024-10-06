using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Localization.Tables;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectApplyXOnCardPlayedMaybe : StatusEffectApplyXOnCardPlayed
    {

        public static readonly string Key_FailedFlip = "websiteofsites.wildfrost.pokefrost.failedflip";

        [PokeLocalizer]
        public static void DefineStrings()
        {
            StringTable tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
            tooltips.SetString(Key_FailedFlip, "Better Luck Next Time!");
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


        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            if (entity != target)
            {
                return false;
            }

            int chance = GetAmount(entity);

            int roll = Dead.Random.Range(1,100);

            if (chance >= roll)
            {
                return base.RunCardPlayedEvent(entity, targets);
            }

            PopupText(entity, Key_FailedFlip);

            return false;
            
        }

    }
}

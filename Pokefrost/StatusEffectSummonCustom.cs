using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Tables;

namespace Pokefrost
{
    internal class StatusEffectInstantSummonCustom : StatusEffectInstantSummon
    {

        public override IEnumerator Process()
        {

            target.data.TryGetCustomData<string>("Future Sight", out string val, "");

            if (!val.IsNullOrEmpty())
            {
                CardData summonCard = Pokefrost.instance.Get<CardData>(val);
                if (summonCard != null)
                {
                    targetSummon.summonCard = summonCard;
                }

            }

            return base.Process();


        }
    }

    internal class StatusEffectInstantSummonLastRecycled : StatusEffectInstantSummon
    {
        public override IEnumerator Process()
        {
            if (StatusEffectAllCardsAreRecycled.PatchRecycle.lastDestroyed != null)
            {
                targetSummon.summonCard = StatusEffectAllCardsAreRecycled.PatchRecycle.lastDestroyed;
                StatusEffectAllCardsAreRecycled.PatchRecycle.lastDestroyed = null;
                yield return base.Process();
            }
            else
            {
                yield return Remove();
            }
        }
    }


    internal class StatusEffectInstantSummonLuminPart : StatusEffectInstantSummon
    {

        public CardData card1;

        public CardData card2;

        public override IEnumerator Process()
        {

            List<Entity> fulldeck = new List<Entity>();
            fulldeck.AddRange(References.Player.handContainer.ToList());
            fulldeck.AddRange(References.Player.drawContainer.ToList());
            fulldeck.AddRange(References.Player.discardContainer.ToList());

            UnityEngine.Debug.Log("[Pokefrost] Checking deck");

            foreach (Entity entity in fulldeck) 
            {
                if (entity.data.name == card1.name)
                {
                    targetSummon.summonCard = card2;
                    break;
                }
                else if (entity.data.name == card2.name) 
                {
                    targetSummon.summonCard = card1;
                    break;
                }
            }

            return base.Process();


        }
    }

    internal class StatusEffectInstantSummonReserve : StatusEffectInstantSummon
    {
        public static List<CardData> reserve;

        public static int node_id = -1;

        public static readonly string Key_ReserveEmpty = "websiteofsites.wildfrost.pokefrost.reserveEmpty";

        [PokeLocalizer]
        public static void DefineStrings()
        {
            StringTable tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
            tooltips.SetString(Key_ReserveEmpty, "No More in Reserve!");
        }

        public override void Init()
        {
            if (Campaign.FindCharacterNode(References.Player).id != node_id) 
            {
                reserve = References.Player.data.inventory.reserve.Clone();
                node_id = Campaign.FindCharacterNode(References.Player).id;
            }
            base.Init();
        }

        public UnityAction GetReserve()
        {
            CardDataList reserve = References.Player.data.inventory.reserve;
            return null;
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

            Debug.Log("[Pokefrost] Process Reserve");

            if (reserve.Count() > 0)
            {
                int r = Dead.Random.Range(0, reserve.Count()-1);
                CardData summonCard = reserve[r];
                Debug.Log("[Pokefrost] Found card "+summonCard.name);
                if (summonCard != null)
                {
                    targetSummon.summonCard = summonCard;
                    if (CanSummon(out var container, out var shoveData))
                    {
                        reserve.RemoveAt(r);
                    }
                }

                return base.Process();

            }

            else
            {
                Debug.Log("[Pokefrost] No Card Found");
                PopupText(target, Key_ReserveEmpty);
                return Remove();
            }


        }
    }



}

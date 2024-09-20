using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static CampaignNodeTypeBoss;

namespace Pokefrost
{
    public class BossRewardDataCard : BossRewardData
    {
        public static Data Example(string name = "spinda")
        {
            Data card = new Data()
            {
                cardDataName = name,
                type = Type.Crown
            };
            return card;
        }

        public override BossRewardData.Data Pull()
        {
            return new Data
            {
                type = Type.Crown
            };
        }

        public new class Data : BossRewardData.Data
        {
            public string cardDataName;
            public Entity card;

            public CardData GetCardData()
            {
                return Pokefrost.instance.Get<CardData>(cardDataName);
            }

            public override void Select()
            {
                References.PlayerData.inventory.deck.Add(card.data);
                MoveCardToDeck(card);

            }

            private void MoveCardToDeck(Entity entity)
            {
                Events.InvokeEntityEnterBackpack(entity);
                entity.transform.parent = References.Player.entity.display.transform;
                entity.display?.hover?.Disable();
                new Routine(AssetLoader.Lookup<CardAnimation>("CardAnimations", "FlyToBackpack").Routine(entity));
            }
        }

        [HarmonyPatch(typeof(BossRewardSelectCrown), "SetUp")]
        class PatchBossRewardCard
        {
            static void Postfix(BossRewardSelectCrown __instance, BossRewardData.Data rewardData, GainBlessingSequence2 gainBlessingSequence)
            {
                if (rewardData is Data data)
                {
                    __instance.crownImage.color = new Color(1, 1, 1, 0);

                    CardData cardData = data.GetCardData().Clone();
                    GameObject gameObject = __instance.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
                    CardLane lane = gameObject.AddComponent<CardLane>();
                    //Debug.Log("[Pokefrost] Before RectTransform");
                    lane.holder = gameObject.GetComponent<RectTransform>();
                    //Debug.Log("[Pokefrost] Before RectTransform");
                    lane.onAdd = new UnityEventEntity();
                    lane.onRemove = new UnityEventEntity();
                    lane.SetSize(1, 0.7f);

                    //Debug.Log("[Pokefrost] Before CardController");
                    CardControllerSelectCard cc = gameObject.AddComponent<CardControllerSelectCard>();
                    cc.pressEvent = new UnityEventEntity();
                    cc.hoverEvent = new UnityEventEntity();
                    cc.unHoverEvent = new UnityEventEntity();
                    lane.AssignController(cc);
                    //Debug.Log("[Pokefrost] After CardController");

                    __instance.popUpName = "{popUpName}";
                    __instance.title = cardData.title;
                    __instance.body = "Spin-da to Win-da!!";

                    cc.pressEvent.AddListener((d) =>
                    {
                        gainBlessingSequence.Select(data);

                        __instance.StartCoroutine(DelayDestroy(__instance));
                    });

                    __instance.StartCoroutine(SetUpEntity(lane, data, cardData, cc));
                }
            }

            static IEnumerator DelayDestroy(BossRewardSelectCrown __instance)
            {
                yield return new WaitForSeconds(0.05f);
                __instance.UnPop();
                __instance.gameObject.SetActive(false);
            }

            static IEnumerator SetUpEntity(CardLane lane, Data data, CardData cardData, CardControllerSelectCard cc)
            {
                Card card = CardManager.Get(cardData, cc, References.Player, false, true);
                yield return card.UpdateData();
                lane.Add(card.entity);
                data.card = card.entity;
                Events.InvokeEntityOffered(card.entity);

                lane.SetChildPositions();
                card.FlipUp();
            }
    }
    }

    
}

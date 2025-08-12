using Dead;
using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

namespace Pokefrost
{
    internal class StatusEffectLoseAction : StatusEffectData
    {

        public ItemHolderPet petPrefab;

        public bool hasEffect;

        public override void Init()
        {
            base.OnCardPlayed += CardPlayed;
        }

        public override bool RunBeginEvent()
        {
            if (!target.inPlay || target.enabled)
            {
                hasEffect = true;
                if (!GameManager.paused && target.display is Card card)
                {
                    card.itemHolderPet?.Create(petPrefab);
                    Events.InvokeNoomlinShow(target);
                }
            }

            return false;
        }

        public override bool RunCardMoveEvent(Entity entity)
        {
            if (entity == target)
            {
                if (target.InHand())
                {
                    RunBeginEvent();
                }
                else if (hasEffect && !Battle.IsOnBoard(entity.preContainers) && Battle.IsOnBoard(entity))
                {
                    hasEffect = false;
                    if (target.display is Card card)
                    {
                        card.itemHolderPet?.Used();
                    }

                    target.owner.freeAction = true;
                }
            }

            return false;
        }

        public override bool RunEnableEvent(Entity entity)
        {
            if (entity == target)
            {
                RunBeginEvent();
            }

            return false;
        }

        public override bool RunDisableEvent(Entity entity)
        {
            if (entity == target)
            {
                RunEndEvent();
            }

            return false;
        }

        public override bool RunEndEvent()
        {
            hasEffect = false;
            if (target != null && target.display is Card card)
            {
                card.itemHolderPet?.DestroyCurrent();
            }

            return false;
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            if (hasEffect && entity == target)
            {
                return !target.silenced;
            }

            return false;
        }

        public IEnumerator CardPlayed(Entity entity, Entity[] targets)
        {
            hasEffect = false;
            if (target.display is Card card)
            {
                card.itemHolderPet?.Used();
                Events.InvokeNoomlinUsed(target);
                Mover mover = card.gameObject.AddComponent<Mover>();
                mover.velocity = new Vector3(PettyRandom.Range(0f, 1f).WithRandomSign(), -12f, 0f);
                mover.frictMult = 0.8f;
                target.wobbler?.WobbleRandom();
                yield return Sequences.Wait(0.6f);
            }

            FreeActionFlag.flag = true;

        }
    }


    [HarmonyPatch(typeof(ItemHolderPetUsed), "SetUp")]
    class DoomlinSetUp
    {
        public static Sprite fullBody = Pokefrost.instance.ImagePath("Doomlin/DoomlinBodyFull.png").ToSprite();

        public static Sprite earLeft = Pokefrost.instance.ImagePath("Doomlin/DoomlinEar_Left.png").ToSprite();

        public static Sprite earRight = Pokefrost.instance.ImagePath("Doomlin/DoomlinEar_Right.png").ToSprite();

        public static Sprite tail = Pokefrost.instance.ImagePath("Doomlin/DoomlinTail.png").ToSprite();

        static void Prefix(ItemHolderPetUsed __instance, Sprite headSprite)
        {
            if (headSprite.name == Pokefrost.instance.GUID + "Doomlin")
            {
                foreach (Image image in __instance.transform.GetComponentsInChildren<Image>())
                {
                    switch (image.name)
                    {
                        case "Body": //Full body that you see when the -oomlin jumps off
                            image.sprite = fullBody; break;
                        case "EarLeft": //Left ear
                            image.sprite = earLeft;
                            image.transform.Translate(new Vector3(-0.1f, 0f, 0f)); break;
                        case "EarRight": //Right ear
                            image.sprite = earRight;
                            image.transform.Translate(new Vector3(0.1f, 0f, 0f)); break;
                        case "Tail": //Tail for when the -oomlin jumps off
                            image.sprite = tail; break;
                        case "Head": //Head
                            image.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                            image.transform.Translate(new Vector3(0f, 0.25f, 0f)); break;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Battle), "WaitForTurnEnd")]
    class FreeActionFlag
    {
        public static bool flag = false;
        static void Postfix(Character character, CardController cardController)
        {
            if (flag)
            {
                character.endTurn = true;
                flag = false;
            }

        }

        public static void Reset()
        {
            flag = false;
        }

    }


}

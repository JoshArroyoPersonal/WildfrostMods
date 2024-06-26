﻿using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Pokefrost
{
    internal class StatusEffectXActsLikeShell : StatusEffectData
    {

        public override bool HasHitRoutine => true;
        public override bool HasPostApplyStatusRoutine => true;

        public string targetType = "";
        public string imagePath;

        public override IEnumerator PostApplyStatusRoutine(StatusEffectApply apply)
        {
            if (target!=null && apply?.target == target && apply.effectData.type == targetType)
            {
                StatusIcon snowIcon = apply.target.GetComponent<Card>().FindStatusIcon("snow");
                if (snowIcon != null && imagePath != null)
                {
                    snowIcon.GetComponent<Image>().sprite = imagePath.ToSprite();
                    snowIcon.transform.SetParent(snowIcon.transform.parent.parent.Find("HealthLayout"));
                }
                else
                {
                    snowIcon = apply.target.GetComponent<Card>().SetStatusIcon("snow", "health", new Stat(apply.count, 0), true);
                    snowIcon.GetComponent<Image>().sprite = imagePath.ToSprite();
                }
                yield return Sequences.Wait(apply.target.curveAnimator.Ping());
            }
        }

        public override IEnumerator HitRoutine(Hit hit)
        {
            if (hit?.target == target && hit.target.FindStatus(targetType))
            {
                StatusEffectData targetEffect = hit.target.FindStatus(targetType);
                while (targetEffect.count > 0 && hit.damage > 0)
                {

                    targetEffect.count--;
                    hit.damage--;
                    hit.damageBlocked++;
                }

                if (targetEffect.count <= 0)
                {
                    yield return targetEffect.Remove();
                }

                target.PromptUpdate();
            }
        }
    }
}

using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pokefrost
{
    internal class StatusEffectXActsLikeShell : StatusEffectData
    {

        public string targetType = "";
        public Sprite sprite;

        public bool active = true;

        public override void Init()
        {
            base.PostApplyStatus += ChangeIcon;
            base.OnHit += BlockDamage;
            base.OnEffectBonusChanged += SilenceCheck;
        }

        private IEnumerator SilenceCheck()
        {
            StatusIcon snowIcon = target?.GetComponent<Card>()?.FindStatusIcon("snow");
            if (snowIcon == null) { yield break; }

            if (GetAmount() > 0)
            {
                snowIcon.GetComponent<Image>().sprite = sprite;
                snowIcon.transform.SetParent(snowIcon.transform.parent.parent.Find("HealthLayout"));
            }
            else
            {
                snowIcon.GetComponent<Image>().sprite = CardManager.cardIcons["snow"].GetComponent<Image>().sprite;
                snowIcon.transform.SetParent(snowIcon.transform.parent.parent.Find("CounterLayout"));
            }
        }

        public override bool RunPostApplyStatusEvent(StatusEffectApply apply)
        {
            if (target != null && apply?.target == target && GetAmount() > 0)
            {
                return apply.effectData.type == targetType;
            }
            return false;
        }

        public IEnumerator ChangeIcon(StatusEffectApply apply)
        {
            StatusIcon snowIcon = apply.target.GetComponent<Card>().FindStatusIcon("snow");
            if (snowIcon != null && sprite != null)
            {
                snowIcon.GetComponent<Image>().sprite = sprite;
                snowIcon.transform.SetParent(snowIcon.transform.parent.parent.Find("HealthLayout"));
            }
            else
            {
                snowIcon = apply.target.GetComponent<Card>().SetStatusIcon("snow", "health", new Stat(apply.count, 0), true);
                snowIcon.GetComponent<Image>().sprite = sprite;
            }
            yield return Sequences.Wait(apply.target.curveAnimator.Ping());
        }



        public override bool RunHitEvent(Hit hit)
        {
            if (hit?.target == target && hit.target.FindStatus(targetType) && GetAmount() > 0)
            {
                return hit.damage > 0;
            }
            return false;
        }

        public IEnumerator BlockDamage(Hit hit)
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

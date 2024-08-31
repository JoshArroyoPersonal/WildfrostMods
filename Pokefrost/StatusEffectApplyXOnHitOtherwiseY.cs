using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectApplyXOnHitOtherwiseY : StatusEffectApplyXOnHit
    {

        public StatusEffectData mainEffect;
        public StatusEffectData otherEffect;
        public TargetConstraint[] applyConstraints2;

        public override bool RunPreAttackEvent(Hit hit)
        {
            if (hit.attacker == target && target.alive && target.enabled && (bool)hit.target)
            {

                bool flag = true;
                TargetConstraint[] array = applyConstraints2;
                foreach (TargetConstraint targetConstraint in array)
                {
                    if (!targetConstraint.Check(hit.target) && (!(targetConstraint is TargetConstraintHasStatus targetConstraintHasStatus) || !targetConstraintHasStatus.CheckWillApply(hit))
                        && (!(targetConstraint is TargetConstraintHasNegativeStatus targetConstraintHasNegativeStatus) || !targetConstraintHasNegativeStatus.CheckWillApply(hit)) )
                    {
                        flag = false;
                        effectToApply = otherEffect;
                        break;
                    }
                }

                if (flag)
                {
                    effectToApply = mainEffect;
                    int amount = GetAmount();
                    if (addDamageFactor != 0)
                    {
                        hit.damage += amount * addDamageFactor;
                    }

                    if (multiplyDamageFactor != 1f && !target.silenced)
                    {
                        hit.damage = Mathf.RoundToInt((float)hit.damage * multiplyDamageFactor);
                    }
                }

                if (!hit.Offensive && (hit.damage > 0 || ((bool)effectToApply && effectToApply.offensive)))
                {
                    hit.FlagAsOffensive();
                }

                storedHit.Add(hit);
            }

            return false;
        }


    }
}

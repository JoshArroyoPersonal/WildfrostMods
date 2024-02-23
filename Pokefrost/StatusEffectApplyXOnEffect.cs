using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectApplyXOnEffect : StatusEffectApplyX
    {
        [SerializeField]
        private bool postHit;

        public StatusEffectData conditionEffect;

        [Header("Modify Damage")]
        [SerializeField]
        private int addDamageFactor;

        [SerializeField]
        private float multiplyDamageFactor = 1f;

        private readonly List<Hit> storedHit = new List<Hit>();

        public override void Init()
        {
            if (postHit)
            {
                base.PostHit += CheckHit;
            }
            else
            {
                base.OnHit += CheckHit;
            }
        }

        public override bool RunPreAttackEvent(Hit hit)
        {
            if (hit.attacker == target && target.alive && target.enabled && (bool)hit.target)
            {
                if (addDamageFactor != 0 || multiplyDamageFactor != 1f)
                {
                    bool flag = true;
                    TargetConstraint[] array = applyConstraints;
                    foreach (TargetConstraint targetConstraint in array)
                    {
                        if (!targetConstraint.Check(hit.target) && (!(targetConstraint is TargetConstraintHasStatus targetConstraintHasStatus) || !targetConstraintHasStatus.CheckWillApply(hit)))
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        int amount = GetAmount();
                        if (addDamageFactor != 0)
                        {
                            hit.damage += amount * addDamageFactor;
                        }

                        if (multiplyDamageFactor != 1f)
                        {
                            hit.damage = Mathf.RoundToInt((float)hit.damage * multiplyDamageFactor);
                        }
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

        public override bool RunPostHitEvent(Hit hit)
        {
            if (storedHit.Contains(hit))
            {
                return hit.Offensive;
            }

            return false;
        }

        public override bool RunHitEvent(Hit hit)
        {
            if (storedHit.Contains(hit))
            {
                return hit.Offensive;
            }

            return false;
        }

        private IEnumerator CheckHit(Hit hit)
        {
            if ((bool)effectToApply)
            {

                foreach (StatusEffectData status in hit.attacker.statusEffects)
                {
                    if (status.name == conditionEffect.name)
                    {
                        yield return Run(GetTargets(hit), status.count);
                        break;
                    }
                }

                yield return null;
            }

            storedHit.Remove(hit);
        }

    }
}

using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class ScriptableMissingHealth : ScriptableAmount
    {
        public override int Get(Entity entity)
        {
            if (!entity)
            {
                return 0;
            }

            return entity.hp.max - entity.hp.current;

        }

    }

    internal class ScriptableCursesInHand : ScriptableAmount
    {
        public override int Get(Entity entity)
        {
            if (!entity)
            {
                return 0;
            }

            int amount = 0;

            foreach (Entity card in References.Player.handContainer)
            {
                foreach (StatusEffectData status in card.statusEffects)
                {
                    if (status.type == "paracurse" || status.type == "weakcurse" || status.type == "powercurse" || status.type == "frenzycurse")
                    {
                        amount += (1 + entity.effectBonus) * ((int)Math.Round(entity.effectFactor));
                        break;
                    }
                }
            }

            return amount;
        }

    }

    public class ScriptableNegativeHalfAttack : ScriptableAmount 
    {
        public override int Get(Entity entity)
        {
            if (!entity)
            {
                return 0;
            }

            return -Mathf.FloorToInt((entity.damage.current + entity.tempDamage.Value)/2);
        }

    }

    public class TargetConstraintHasLastRecycled : TargetConstraint
    {
        public override bool Check(CardData targetData)
        {
            return (Campaign.FindCharacterNode(References.Player).id == StatusEffectAllCardsAreRecycled.PatchRecycle.node && (bool)StatusEffectAllCardsAreRecycled.PatchRecycle.lastDestroyed);
        }

        public override bool Check(Entity target)
        {
            return Check(target.data);
        }
    }

    public class TargetConstraintHasSlowkingCrown : TargetConstraint
    {
        public override bool Check(Entity target)
        {
            return Check(target.data);
        }

        public override bool Check(CardData targetData)
        {
            if (!(targetData.upgrades.Find((CardUpgradeData a) => a.name == "websiteofsites.wildfrost.pokefrost.CrownSlowking") != null))
            {
                return not;
            }

            return !not;
        }
    }


    public class TargetConstraintHasNegativeStatus : TargetConstraint
    {


        public override bool Check(Entity target)
        {
            if (!target.statusEffects.Any((StatusEffectData a) => a.IsNegativeStatusEffect()))
            {
                return not;
            }

            return !not;
        }

        public override bool Check(CardData targetData)
        {
            bool flag = false;
            CardData.StatusEffectStacks[] startWithEffects = targetData.startWithEffects;
            for (int i = 0; i < startWithEffects.Length; i++)
            {
                if (startWithEffects[i].data.IsNegativeStatusEffect())
                {
                    flag = true;
                    break;
                }
            }

            if (!not)
            {
                return flag;
            }

            return !flag;
        }

        public bool CheckWillApply(Hit hit)
        {
            bool flag = false;
            List<CardData.StatusEffectStacks> statusEffects = hit.statusEffects;
            if (statusEffects != null && statusEffects.Count > 0)
            {
                foreach (CardData.StatusEffectStacks statusEffect in hit.statusEffects)
                {
                    if (statusEffect.data.IsNegativeStatusEffect())
                    {
                        flag = true;
                        break;
                    }
                }
            }

            if (!not)
            {
                return flag;
            }

            return !flag;
        }



    }





}

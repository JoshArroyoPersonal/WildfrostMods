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
                        amount++;
                        break;
                    }
                }
            }

            return amount;
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
}

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

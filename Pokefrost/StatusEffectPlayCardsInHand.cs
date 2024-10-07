using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    public class StatusEffectPlayCardsInHand : StatusEffectInstant
    {
        public TargetConstraint[] applyConstraints;
        public override IEnumerator Process()
        {
            Entity[] entities = target.targetMode.GetPotentialTargets(target, null, null) ?? new Entity[0];
            entities = entities.Where(e => e.isActiveAndEnabled && e.canBeHit).ToArray();
            if (entities.Length > 0)
            {
                Entity entity = entities.RandomItem();
                List<Entity> list = References.Player.handContainer.Where(e => SatisfiesConstraints(e)).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    var action = new ActionTriggerAgainst(list[i], target, entity, null);
                    ActionQueue.Stack(action);
                }
            }
            return base.Process();
        }

        public bool SatisfiesConstraints(Entity entity)
        {
            if (applyConstraints == null) { return true; }
            foreach (var constraint in applyConstraints)
            {
                if (!constraint.Check(entity))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

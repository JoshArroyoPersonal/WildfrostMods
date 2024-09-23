using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectApplyXOnRecall : StatusEffectApplyX
    {

        public bool once = false;

        public override void Init()
        {
            Events.OnActionQueued += ActionQueued;
        }

        public void OnDestroy()
        {
            Events.OnActionQueued -= ActionQueued;
        }

        public void ActionQueued(PlayAction action)
        {
            if (action is ActionMove actionMove && actionMove.entity == target && (bool)target.owner && actionMove.toContainers.Contains(target.owner.discardContainer) && Battle.IsOnBoard(actionMove.entity.containers))
            {
                ActionQueue.Insert(ActionQueue.IndexOf(action), new ActionSequence(Sequence()));
            }
        }

        public IEnumerator Sequence()
        {
            yield return Run(GetTargets());
            if (once)
            {
                yield return Remove();
                target.display.promptUpdateDescription = true;
                target.PromptUpdate();
            }
        }
    }

    internal class StatusEffectApplyXWhenDiscardedFixed : StatusEffectApplyXWhenDiscarded
    {
        public void OnDestroy()
        {
            Events.OnActionQueued -= ActionQueued;
        }
    }


}

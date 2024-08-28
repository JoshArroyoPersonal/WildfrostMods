using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    public class StatusEffectInstantRunScript : StatusEffectInstant
    {
        public PlayAction action;
        public bool stack = true;


        public List<EntityCardScript> scriptList;
        public override IEnumerator Process()
        {
            if (action != null)
            {
                if (stack)
                {
                    ActionQueue.Stack(action);
                }
                else
                {
                    ActionQueue.Add(action);
                }
            }

            if (scriptList != null && scriptList.Count > 0)
            {
                foreach (EntityCardScript script in scriptList)
                {
                    yield return script.Run(target, count);
                    
                }
            }

            target.display.promptUpdateDescription = true;
            target.PromptUpdate();
            yield return base.Process();
        }
    }
}

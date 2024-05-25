using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectWhileInHandXUpdate : StatusEffectWhileInHandX
    {
        public override IEnumerator BeginRoutine()
        {
            yield return base.BeginRoutine();
            target.display.promptUpdateDescription = true;
            target.PromptUpdate();
        }
    }
}

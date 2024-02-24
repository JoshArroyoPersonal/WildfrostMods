using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectTriggerWhenDamageType : StatusEffectData
    {
        private bool isAlreadyOnBoard;

        private string triggerdamagetype;
        public override bool HasHitRoutine => true;

        public override object GetMidBattleData()
        {
            return Battle.IsOnBoard(target);
        }

        public override void RestoreMidBattleData(object data)
        {
            if (data is bool)
            {
                bool flag = (bool)data;
                isAlreadyOnBoard = flag && Battle.IsOnBoard(target);
            }
        }

        public override void Init()
        {
            base.OnHit += Enable;
        }

        public override bool RunHitEvent(Hit hit)
        {
            return Battle.IsOnBoard(target);
        }

        private IEnumerator Enable(Hit hit)
        {
            if (hit.damageType == triggerdamagetype)
            {
                yield return Sequences.Wait(0.2f);
                yield return Activate();
            }
        }

        private IEnumerator Activate()
        {
            if (!target.silenced)
            {
                yield return Sequences.Wait(0.1f);
                target.curveAnimator?.Ping();
                yield return Sequences.Wait(0.5f);
                ActionQueue.Stack(new ActionTrigger(target, null), fixedPosition: true);
            }
        }

    }
}
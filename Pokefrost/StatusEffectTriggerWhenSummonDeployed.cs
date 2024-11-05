using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Pokefrost
{
    internal class StatusEffectTriggerWhenSummonDeployed : StatusEffectData
    {
        private bool isAlreadyOnBoard;
        public override bool HasEnableRoutine => true;

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
            base.OnEnable += Enable;
        }

        public override bool RunEnableEvent(Entity entity)
        {
            return Battle.IsOnBoard(target);
        }

        private IEnumerator Enable(Entity entity)
        {
            if (entity.data.cardType.name == "Summoned")
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
                yield return Sequences.Wait(0.2f);
                ActionQueue.Stack(new ActionTrigger(target, null), fixedPosition: true);
            }
        }
    }

}
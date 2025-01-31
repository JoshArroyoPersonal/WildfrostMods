using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectRiptide : StatusEffectData
    {

        public string lastContainerName;

        public override void Init()
        {
            base.OnCardMove += Damage;
            base.OnCardPlayed += CheckMove;
            base.OnTurnStart += Damage;
        }

        public override bool RunBeginEvent()
        {
            if (target._containers != null && target._containers.Count > 0)
            {
                lastContainerName = target._containers[0].name;
            }

            return base.RunBeginEvent();
        }
        public override bool RunCardMoveEvent(Entity entity)
        {
            if (target._containers != null && target._containers.Count > 0 && target._containers[0].name != lastContainerName)
            {
                lastContainerName = target._containers[0].name;
                return true;
            }

            return false;
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            return RunCardMoveEvent(entity);
        }

        public override bool RunTurnStartEvent(Entity entity)
        {
            return RunCardMoveEvent(entity);
        }

        private IEnumerator CheckMove(Entity entity, Entity[] targets)
        {
            return Damage(entity);
        }

        public IEnumerator Damage(Entity entity)
        {
            Hit hit2 = new Hit(target, target, count)
            {
                canRetaliate = false,
                damageType = "riptide"
            };

            //Pokefrost.fx.TryPlayEffect("jolt", target.transform.position, 0.5f * target.transform.lossyScale);
            //Pokefrost.fx.TryPlaySound("jolt");
            target.curveAnimator.Ping();
            yield return new WaitForSeconds(0.25f);
            yield return hit2.Process();
        }
    }

}

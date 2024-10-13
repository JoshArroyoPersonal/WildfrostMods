using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectWhileExistingX : StatusEffectWhileActiveX
    {

        public override bool CanActivate()
        {
            return true;
        }

        public override bool CheckActivateOnMove(CardContainer[] fromContainers, CardContainer[] toContainers)
        {
            return false;
        }

        public override bool CheckDeactivateOnMove(CardContainer[] fromContainers, CardContainer[] toContainers)
        {
            return false;
        }


    }

    internal class StatusEffectWhileRedrawBellChargedX : StatusEffectWhileActiveX
    {

        RedrawBellSystem bellSystem;

        public override void Init()
        {
            Events.OnRedrawBellHit += Reset;
            Events.OnBattlePreTurnStart += TryActivate;
            base.Init();
        }

        public override void OnDestroy()
        {
            Events.OnRedrawBellHit -= Reset;
            Events.OnBattlePreTurnStart -= TryActivate;
            base.OnDestroy();
        }

        private void Reset(RedrawBellSystem arg0)
        {
            if (active)
            {
                ActionQueue.Stack(new ActionSequence(Deactivate()));
            }
            
        }

        private void TryActivate(int arg0)
        {
            if (CanActivate() && !active)
            {
                ActionQueue.Stack(new ActionSequence(Activate()));
            }
        }

        public override bool CanActivate()
        {
            if (bellSystem == null)
            {
                bellSystem = GameObject.FindObjectOfType<RedrawBellSystem>();
            }

            return (bellSystem.IsCharged && bellSystem.interactable);
        }

        public override bool CheckActivateOnMove(CardContainer[] fromContainers, CardContainer[] toContainers)
        {
            return false;
        }

        public override bool CheckDeactivateOnMove(CardContainer[] fromContainers, CardContainer[] toContainers)
        {
            return false;
        }


    }
}

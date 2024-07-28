using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectIncreaseAttackBasedOnCardsDrawnThisTurn : StatusEffectData
    {
        [SerializeField]
        public StatusEffectData effectToGain;

        public int currentAmount;
        public int amount;
        public override bool HasTurnEndRoutine => true;
        public override bool HasActionPerformedRoutine => true;

        public override IEnumerator ActionPerformedRoutine(PlayAction action)
        {
            Debug.Log(action.Name.ToString());
            if (action.Name == "ActionDraw")
            {
                yield return Activate(Pokefrost.cardsdrawn);
            }
            yield break;
        }

        public override IEnumerator TurnEndRoutine(Entity entity)
        {
            if(entity == target)
            {
                yield return Deactivate();
            }
        }

        public IEnumerator Activate(int arg)
        {
            Debug.Log("[Sneasel] Activate");
            Debug.Log(arg.ToString());
            amount = GetAmount() * arg;
            currentAmount += amount;
            Debug.Log(currentAmount.ToString());
            Debug.Log(amount.ToString());
            Debug.Log(((count + target.effectBonus) * target.effectFactor).ToString());
            Pokefrost.cardsdrawn = 0;
            yield return StatusEffectSystem.Apply(target, target, effectToGain, amount, temporary: true);
        }

        public IEnumerator Deactivate()
        {
            Debug.Log("[Sneasel] Decactivate");
            for (int num = target.statusEffects.Count - 1; num >= 0; num--)
            {
                StatusEffectData statusEffectData = target.statusEffects[num];
                if ((bool)statusEffectData && statusEffectData.name == effectToGain.name)
                {
                    yield return statusEffectData.RemoveStacks(currentAmount, removeTemporary: true);
                    break;
                }
            }

            currentAmount = 0;
        }


    }


}

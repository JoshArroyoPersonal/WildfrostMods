using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectDuplicateEffect : StatusEffectApplyX
    {
        private int chain = 0;
        private int maxChain = 3;
        private Dictionary<string, Vector2Int> amounts = new Dictionary<string, Vector2Int>();

        public ApplyToFlags whenAppliedFlags;
        public bool debuffsOnly = false;
        public bool instantCustom = false;

        public override void Init()
        {
            base.PostApplyStatus += Copy;   
        }

        public override bool RunApplyStatusEvent(StatusEffectApply apply)
        {
            if (!apply.applier || apply.applier == target || !apply.target || !apply.effectData || apply.effectData.type.IsNullOrWhitespace() || target.silenced) { return false; }
            bool debuff = apply.effectData.isStatus && apply.effectData.offensive && apply.effectData.visible;
            if (!debuff && debuffsOnly) { return false; }

            List<Entity> candidates = GetAppliedTargets();
            if (!candidates.Contains(apply.target)) { return false; }

            amounts[apply.effectData.type] = CurrentAmounts(apply.target, apply.effectData.type);
            Debug.Log($"[Pokefrost] {apply.effectData.type}");
            return false;
        }

        public List<Entity> GetAppliedTargets()
        {
            ApplyToFlags apply = applyToFlags;
            applyToFlags = whenAppliedFlags;
            List<Entity> entities = GetTargets();
            applyToFlags = apply;
            return entities;
        }

        private IEnumerator Copy(StatusEffectApply apply)
        {
            chain++;
            if (chain == maxChain) { yield break; }

            
            if (instantCustom && effectToApply is StatusEffectApplyXInstant effect)
            {
                effect.effectToApply = apply.effectData;
            }
            else 
            {
                effectToApply = apply.effectData;
            }
            yield return Run(GetTargets(), apply.count);
            chain = 0;
        }

        public override bool RunPostApplyStatusEvent(StatusEffectApply apply)
        {
            if (!apply.applier || apply.applier == target || !apply.target || !apply.effectData || apply.effectData.type.IsNullOrWhitespace() || target.silenced) { return false; }

            List<Entity> candidates = GetAppliedTargets();
            if (!candidates.Contains(apply.target)) { return false; }

            if (!amounts.TryGetValue(apply.effectData.type, out Vector2Int amount))
            {
                return false;
            }

            Vector2Int newAmount = CurrentAmounts(apply.target, apply.effectData.type);
            if ((newAmount.x - amount.x) - (newAmount.y - amount.y) <= 0 && (newAmount.x - amount.x) != 0) { return false; }

            amounts.Remove(apply.effectData.type);

            return true;
        }

        protected Vector2Int CurrentAmounts(Entity frontAlly, string effectType)
        {
            StatusEffectData effect = frontAlly.statusEffects.FirstOrDefault((s) => s.type == effectType);
            if (effect == default(StatusEffectData))
            {
                return Vector2Int.zero;
            }

            return new Vector2Int(effect.count, effect.temporary);
        }
    }
}

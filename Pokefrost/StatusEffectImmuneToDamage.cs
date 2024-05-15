using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NexPlugin.Ranking;

namespace Pokefrost
{
    internal class StatusEffectImmuneToDamage : StatusEffectData
    {

        public List<string> immuneTypes;
        public bool reverse = false;

        public bool invis = false;
        public float invisFadeIn = 0.2f;
        public float invisFadeOut = 0.8f;
        protected Hit invisHit;

        public bool ignoreReactions;

        public override void Init()
        {
            base.OnHit += Check;
            if (invis)
            {
                base.OnHit += Invisible;
            }
        }

        private IEnumerator Invisible(Hit hit)
        {
            invisHit = hit;
            yield return Fade(1.0f, 0.5f, invisFadeIn);
        }

        public override bool RunHitEvent(Hit hit)
        {
            if (hit.target == target && hit.Offensive && hit.canBeNullified)
            {
                if (reverse ^ immuneTypes.Contains(hit.damageType))
                {
                    return hit.damage > 0;
                }
            }

            if (hit.attacker == target && ignoreReactions)
            {
                hit.canRetaliate = false;
                return invis;
            }

            return false;
        }

        public override bool RunPostHitEvent(Hit hit)
        {
            if (invis && hit == invisHit)
            {
                target.StartCoroutine(Fade(0.5f, 1.0f, invisFadeOut));
            }
            return false;
        }

        private IEnumerator Check(Hit hit)
        {
            SfxSystem.OneShot("event:/sfx/status_icon/block_decrease");
            hit.damageBlocked = hit.damage;
            hit.damage = 0;

            target.PromptUpdate();
            yield break;
        }

        private IEnumerator Fade(float start, float end, float dur)
        {
            LeanTween.value(target.gameObject, start, end, dur).setEase(LeanTweenType.easeOutQuad).setOnUpdate(UpdateFade);
            yield return dur;
        }
        
        private void UpdateFade(float alpha)
        {
            Card card = target.display as Card;
            card.canvasGroup.alpha = alpha;
        }
    }
}

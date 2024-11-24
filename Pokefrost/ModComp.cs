using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pokefrost
{
    public static class ModComp
    {
        private static float tScale = 1f;
        private static float rScale = -30f;
        private static float dur = 1f;
        public static IEnumerator InspectCosmetic(RectTransform parent)
        {
            GameObject holder = new GameObject("Holder", new Type[] { typeof(RectTransform) });
            holder.transform.SetParent(parent.transform, false);

            GameObject icon = new GameObject("Icon", new Type[] { typeof(Image) });
            icon.transform.SetParent(holder.transform, false);
            icon.GetComponent<RectTransform>().sizeDelta = 5 * Vector2.one;
            icon.GetComponent<Image>().sprite = Pokefrost.instance.IconSprite;

            AnimationCurve sineCurve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(0.125f, 0.707f),
                new Keyframe(0.25f, 1f),
                new Keyframe(0.375f, 0.707f),
                new Keyframe(0.5f, 0),
                new Keyframe(0.625f, -0.707f),
                new Keyframe(0.75f, -1),
                new Keyframe(0.875f, -0.707f),
                new Keyframe(1, 0)
                );

            while(true)
            {
                yield return Sequences.Wait(2f);
                LeanTween.moveLocalX(icon, tScale, dur).setEase(sineCurve);
                LeanTween.rotateZ(icon, rScale, dur).setEase(sineCurve);
                yield return Sequences.Wait(dur);
            }
        }
    }
}

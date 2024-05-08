using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public static class Ext
    {
        public static UnityEngine.Localization.Tables.StringTable Collection => LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
        public static T CreateStatus<T>(this T t, string name, string desc = null, string textInsert = null, string type = "", bool boostable = false, bool stackable = true) where T : StatusEffectData
        {
            T status = ScriptableObject.CreateInstance<T>();
            status.name = name;
            status.targetConstraints = new TargetConstraint[0];
            if (!desc.IsNullOrEmpty())
            {
                Collection.SetString(name + "_text", desc);
                status.textKey = Collection.GetString(name + "_text");
                if (!textInsert.IsNullOrEmpty())
                {
                    status.textInsert = textInsert;
                }
            }
            status.type = type;
            status.canBeBoosted = boostable;
            status.stackable = stackable;
            return status;
        }

        public static T SetConstraints<T>(this T t, params TargetConstraint[] ts) where T : StatusEffectData
        {
            t.targetConstraints = ts;
            return t;
        }

        public static T ApplyX<T>(this T t, StatusEffectData effectToApply, StatusEffectApplyX.ApplyToFlags flags) where T:StatusEffectApplyX
        {
            t.effectToApply = effectToApply;
            t.applyToFlags = flags;
            return t;
        }

        public static void Register(this StatusEffectData status, WildfrostMod mod)
        {
            status.ModAdded = mod;
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", status);
        }
    }
}

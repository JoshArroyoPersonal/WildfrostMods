#region Assembly Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\Assembly-CSharp.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StatusEffectMultEffects : StatusEffectData
{
    [SerializeField]
    public List<StatusEffectData> effects = new List<StatusEffectData>();

    public List<Entity.TraitStacks> silenced;

    public Entity.TraitStacks added;

    public List<int> addedAmount;

    public override bool HasStackRoutine => true;

    public override bool Instant => true;

    public override IEnumerator StackRoutine(int stacks)
    {
        foreach(var item in effects)
        {
            item.count = stacks;
            yield return StatusEffectSystem.Apply(target, applier, item, stacks);
        }
        yield return target.UpdateTraits();
        target.display.promptUpdateDescription = true;
        target.PromptUpdate();
    }
}
#if false // Decompilation log
'63' items in cache
------------------
Resolve: 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll'
------------------
Resolve: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\UnityEngine.CoreModule.dll'
------------------
Resolve: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll'
------------------
Resolve: 'Rewired_Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Rewired_Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\Rewired_Core.dll'
------------------
Resolve: 'NaughtyAttributes.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'NaughtyAttributes.Core, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Core.dll'
------------------
Resolve: 'Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\Unity.TextMeshPro.dll'
------------------
Resolve: 'UnityEngine.AIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\UnityEngine.AIModule.dll'
------------------
Resolve: 'Unity.Localization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'Unity.Localization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'Unity.Addressables, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'Unity.Addressables, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEngine.ParticleSystemModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.ParticleSystemModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'Unity.ResourceManager, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'Unity.ResourceManager, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEngine.VideoModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.VideoModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\UnityEngine.VideoModule.dll'
------------------
Resolve: 'UnityEngine.AnimationModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.AnimationModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEngine.TextRenderingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.TextRenderingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'FMODUnity, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'FMODUnity, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\FMODUnity.dll'
------------------
Resolve: 'Steamworks, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Steamworks, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\Steamworks.dll'
------------------
Resolve: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEngine.Physics2DModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.Physics2DModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'DeadSafe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'DeadSafe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\DeadSafe.dll'
------------------
Resolve: 'DeadRandom, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'DeadRandom, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\DeadRandom.dll'
------------------
Resolve: 'Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\Assembly-CSharp-firstpass.dll'
------------------
Resolve: 'MonoMod.Utils, Version=21.9.19.1, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'MonoMod.Utils, Version=21.9.19.1, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\MonoMod.Utils.dll'
------------------
Resolve: 'UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\UnityEngine.AudioModule.dll'
------------------
Resolve: 'UnityEngine.IMGUIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.IMGUIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: '0Harmony, Version=2.5.2.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: '0Harmony, Version=2.5.2.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\0Harmony.dll'
------------------
Resolve: 'DeadExtensions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'DeadExtensions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\DeadExtensions.dll'
------------------
Resolve: 'UnityEngine.JSONSerializeModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.JSONSerializeModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEngine.ImageConversionModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.ImageConversionModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'Rewired_Windows, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Rewired_Windows, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\Rewired_Windows.dll'
------------------
Resolve: 'Rewired_Windows_Functions, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Rewired_Windows_Functions, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Wildfrost_Data\Managed\Rewired_Windows_Functions.dll'
------------------
Resolve: 'Unity.Mathematics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'Unity.Mathematics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEngine.ScreenCaptureModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.ScreenCaptureModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.U2D;
using Deadpan.Enums.Engine.Components.Modding;
using BattleEditor;
using WildfrostHopeMod.VFX;
using WildfrostHopeMod.SFX;
using static WildfrostHopeMod.VFX.GIFLoader;

namespace Pokefrost
{
    public class FXHelper
    {
        public GIFLoader giffy;
        public SFXLoader silly;
        public WildfrostMod mod;
        public FXHelper(WildfrostMod mod, string animLocation, string soundLocation)
        {
            giffy = new GIFLoader(null, mod.ImagePath(animLocation));
            giffy.RegisterAllAsApplyEffect();

            silly = new SFXLoader(mod.ImagePath(soundLocation), initialize: true);
            silly.LoadSoundsFromDir(silly.Directory);
            //silly.RegisterAllSoundsToGlobal();
        }

        public void TryPlaySound(string key, SFXLoader.PlayAs playAs = SFXLoader.PlayAs.SFX)
        {
            silly.TryPlaySound(key, playAs);
        }

        public GameObject TryPlayEffect(string key, Vector3 position = default(Vector3), Vector3 scale = default(Vector3), PlayType playAs = PlayType.applyEffect)
        {
            return giffy.TryPlayEffect(key, position, scale, playAs);
        }
    }

    internal static class AddressableExtMethods
    {
        internal static SpriteAtlas Sprites;

        internal static void LoadAtlas()
        {
            if (!Addressables.ResourceLocators.Any(r => r is ResourceLocationMap map && map.LocatorId == Pokefrost.CatalogPath))
                Addressables.LoadContentCatalogAsync(Pokefrost.CatalogPath).WaitForCompletion();

            Sprites = (SpriteAtlas)Addressables.LoadAssetAsync<UnityEngine.Object>($"Assets/websiteofsites.pokefrost/PokefrostAtlas.spriteatlas").WaitForCompletion();
                                                                                    
        }

        internal static Sprite SaferASprite(string spriteName)
        {
            Sprite spr = ASprite(spriteName);
            if (spr == null || spr.texture.width < 10)
            {
                spr = Pokefrost.instance.ImagePath(spriteName).ToSprite();
            }
            return spr;
        }

        internal static Sprite ASprite(string spriteName)
        {
            return Sprites.GetSprite(spriteName.Replace(".png", ""));
        }

        internal static CardDataBuilder SetASprites(this CardDataBuilder b, string mainImage, string backgroundImage)
        {
            return b.SetSprites(ASprite(mainImage), ASprite(backgroundImage));
        }

        internal static CardUpgradeDataBuilder WithAImage(this CardUpgradeDataBuilder b, string image)
        {
            return b.WithImage(ASprite(image));
        }

        internal static GameModifierDataBuilder ChangeASprites(this GameModifierDataBuilder b, string bell, string dinger)
        {
            b.WithBellSprite(ASprite(bell));
            if (!dinger.IsNullOrEmpty())
            {
                b.WithDingerSprite(ASprite(dinger));
            }
            else
            {
                b.WithDingerSprite(Sprite.Create(b._data.bellSprite.texture, new Rect(0, 0, 0, 0), 0.5f*Vector2.one));
            }
            return b.WithBellSprite(ASprite(bell));
        }

        internal static BattleDataEditor SetASprite(this BattleDataEditor b, string sprite)
        {
            return b.SetSprite(ASprite(sprite));
        }
    }
}

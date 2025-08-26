using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using AssetKits.ParticleImage;

namespace _Project.Scripts.Utils
{
    // Addressable aracı ile Atlas indirme Tool'u
    public static class AddressableAtlasLoader 
    {
        private static Dictionary<AssetReferenceAtlasedSprite, Sprite> _spriteCache = new Dictionary<AssetReferenceAtlasedSprite, Sprite>();
        private static HashSet<AssetReferenceAtlasedSprite> _loadingSprites = new HashSet<AssetReferenceAtlasedSprite>();  
         
        public static void LoadSprite<T>(AssetReferenceAtlasedSprite atlasSprite, T component) where T : Component
        {
            LoadSprite(atlasSprite, sprite => SetSprite(component, sprite));
        } 
        
        private static void SetSprite<T>(T component, Sprite sprite) where T : Component
        {
            if (!component || !sprite) return;
            
            switch (component)
            {
                case UnityEngine.UI.Image image:
                    image.sprite = sprite;
                    break;
                    
                case ParticleImage particleImage:
                    particleImage.sprite = sprite;
                    break;
                    
                case SpriteRenderer spriteRenderer:
                    spriteRenderer.sprite = sprite;
                    break; 
            }
        }
          
        public static void LoadSprite(AssetReferenceAtlasedSprite atlasSprite, System.Action<Sprite> onLoaded = null)
        { 
            if (atlasSprite?.RuntimeKeyIsValid() != true)
            {
                onLoaded?.Invoke(null);
                return;
            } 
            
            if (_spriteCache.TryGetValue(atlasSprite, out var cachedSprite))
            {
                onLoaded?.Invoke(cachedSprite);
                return;
            }
 
            if (_loadingSprites.Contains(atlasSprite))
            {
                if (atlasSprite.OperationHandle.IsValid())
                    atlasSprite.OperationHandle.Completed += _ => onLoaded?.Invoke(_spriteCache.GetValueOrDefault(atlasSprite));
                return;
            }
 
            _loadingSprites.Add(atlasSprite);
            atlasSprite.LoadAssetAsync<Sprite>().Completed += operation =>
            {
                _loadingSprites.Remove(atlasSprite);
                var sprite = operation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded ? operation.Result : null;
                if (sprite) _spriteCache[atlasSprite] = sprite;
                onLoaded?.Invoke(sprite);
            };
        }  
        
        public static void ClearCache()
        {
            foreach (var kvp in _spriteCache)
                if (kvp.Key.IsValid()) kvp.Key.ReleaseAsset();
            
            _spriteCache.Clear();
            _loadingSprites.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _spriteCache?.Clear();
            _loadingSprites?.Clear();
        }
    }
}
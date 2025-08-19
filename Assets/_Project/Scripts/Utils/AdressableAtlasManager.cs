using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

public static class AdressableAtlasManager
{
    private static Dictionary<AssetReferenceAtlasedSprite, Sprite> spriteCache = new Dictionary<AssetReferenceAtlasedSprite, Sprite>();
    private static HashSet<AssetReferenceAtlasedSprite> loadingSprites = new HashSet<AssetReferenceAtlasedSprite>();
 
    public static void LoadSprite(AssetReferenceAtlasedSprite atlasSprite, System.Action<Sprite> onLoaded = null)
    {
        if (atlasSprite == null || !atlasSprite.RuntimeKeyIsValid())
        {
            onLoaded?.Invoke(null);
            return;
        }
 
        if (spriteCache.ContainsKey(atlasSprite))
        {
            onLoaded?.Invoke(spriteCache[atlasSprite]);
            return;
        }
 
        if (loadingSprites.Contains(atlasSprite))
        { 
            var handle = atlasSprite.OperationHandle;
            if (handle.IsValid())
            {
                handle.Completed += (operation) => onLoaded?.Invoke(spriteCache.ContainsKey(atlasSprite) ? spriteCache[atlasSprite] : null);
            }
            return;
        }
 
        loadingSprites.Add(atlasSprite);
        
        var loadHandle = atlasSprite.LoadAssetAsync<Sprite>();
        loadHandle.Completed += (operation) =>
        {
            loadingSprites.Remove(atlasSprite);
            
            if (operation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded && operation.Result != null)
            {
                spriteCache[atlasSprite] = operation.Result;
                onLoaded?.Invoke(operation.Result);
            }
            else
            {
                onLoaded?.Invoke(null);
            }
        };
    } 
 
    public static void LoadSprite(AssetReferenceAtlasedSprite atlasSprite, UnityEngine.UI.Image image)
    {
        LoadSprite(atlasSprite, (sprite) => { if (image != null && sprite != null) image.sprite = sprite; });
    } 
    
    public static void ClearCache()
    {
        foreach (var kvp in spriteCache)
        {
            if (kvp.Key.IsValid())
                kvp.Key.ReleaseAsset();
        }
        spriteCache.Clear();
        loadingSprites.Clear();
    }
}
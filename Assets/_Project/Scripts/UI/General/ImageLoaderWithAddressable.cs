 

using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace _Project.Scripts.UI.General
{
    // Sabit AssetRef Atlas'in sprite'larını yüklemesini sağlıyor.
    public class ImageLoaderWithAddressable : MonoBehaviour
    {
        [SerializeField] private AssetReferenceAtlasedSprite _requireSprite;
        [SerializeField] private Image _sourceImage;

        private void Awake()
        {
            AddressableAtlasLoader.LoadSprite(_requireSprite, _sourceImage); 
        }
    }
}

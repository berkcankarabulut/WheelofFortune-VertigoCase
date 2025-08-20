using _Project.Scripts.Data.Reward;
using GuidSystem.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "RewardItemSO_", menuName = "Project/Reward/RewardItemSO")]
    public class RewardItemSO : ScriptableObject
    {
        [SerializeField] private SerializableGuid _id = SerializableGuid.NewGuid();
        [SerializeField] private RewardType _type;
        [SerializeField] private AssetReferenceAtlasedSprite _icon; 
        [SerializeField] private int _maxLimit = 1;
        public SerializableGuid Id => _id;
        public RewardType Type => _type;
        public AssetReferenceAtlasedSprite Icon => _icon; 
        public string Name => this.name;   
        public int MaxLimit => _maxLimit;
    }
}
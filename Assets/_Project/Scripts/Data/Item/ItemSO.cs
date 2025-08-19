using System;
using _Project.Scripts.Data.Reward;
using GuidSystem.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Data.Item
{
    [CreateAssetMenu(fileName = "ItemSO_", menuName = "Project/Reward/ItemSO")]
    public class ItemSO : ScriptableObject, IEquatable<SerializableGuid>
    {
        private SerializableGuid _id = SerializableGuid.NewGuid();
        [SerializeField] private RewardType _type;
        [SerializeField] private AssetReferenceAtlasedSprite _icon;
        [SerializeField] private string _name;    

        public SerializableGuid Id => _id;
        public RewardType Type => _type;
        public AssetReferenceAtlasedSprite Icon => _icon; 
        public string Name => _name;  

        public bool Equals(SerializableGuid id)
        {
            return Id.ToGuid() == id.ToGuid();
        }
    }
}
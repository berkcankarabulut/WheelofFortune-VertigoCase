using System.Collections.Generic; 
using _Project.Scripts.Data.Item; 
using UnityEngine;

namespace _Project.Scripts.Data.Reward
{
    [CreateAssetMenu(fileName = "ItemDatabaseSO", menuName = "Project/Database/ItemDatabaseSO")]
    public class ItemDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<RewardItemSO> _allItems = new List<RewardItemSO>();
        private Dictionary<string, RewardItemSO> _itemLookup;

        private void OnEnable()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            _itemLookup = new Dictionary<string, RewardItemSO>();
            
            foreach (var item in _allItems)
            {
                if (item != null)
                {
                    string id = item.Id.ToGuid().ToString();
                    _itemLookup[id] = item;
                }
            }
        }

        public RewardItemSO GetItemById(string itemId)
        {
            if (_itemLookup == null) BuildLookup();
            
            _itemLookup.TryGetValue(itemId, out RewardItemSO item);
            return item;
        }

#if UNITY_EDITOR
        [ContextMenu("🔄 Auto Fill Database")]
        private void AutoFillDatabase()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:RewardItemSO");
            _allItems.Clear();
            
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                RewardItemSO item = UnityEditor.AssetDatabase.LoadAssetAtPath<RewardItemSO>(path);
                if (item != null) _allItems.Add(item);
            }
            
            BuildLookup();
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"[ItemDatabase] Found {_allItems.Count} items");
        }
#endif
    }
}
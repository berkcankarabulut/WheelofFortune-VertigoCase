using UnityEngine;

namespace _Project.Scripts.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
 
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        Debug.LogError($"[Singleton] No instance of {typeof(T)} found in scene!");
                    }
                }

                return _instance;
            }
        }
 
        public static bool HasInstance => _instance != null; 
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject); 
            }
            else if (_instance != this)
            {
                Debug.LogWarning(
                    $"[Singleton] Another instance of {typeof(T)} already exists! Destroying: {gameObject.name}");
                Destroy(gameObject);
            }
        } 
 
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
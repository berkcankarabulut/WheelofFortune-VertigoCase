using UnityEngine;
using System.Runtime.CompilerServices;

namespace _Project.Scripts.Utils 
{ 
     public static class LoggerExtensions
    { 
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        [System.Diagnostics.Conditional("ENABLE_LOGGING")]
        public static void Log<T>(this T obj, string message) where T : class
        {
            Debug.Log($"[{typeof(T).Name}] {message}", obj as Object);
        }
         
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        [System.Diagnostics.Conditional("ENABLE_LOGGING")]
        public static void LogWarning<T>(this T obj, string message) where T : class
        {
            Debug.LogWarning($"[{typeof(T).Name}] ⚠️ {message}", obj as Object);
        }
         
        public static void LogError<T>(this T obj, string message) where T : class
        {
            Debug.LogError($"[{typeof(T).Name}] ❌ {message}", obj as Object);
        }
         
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        [System.Diagnostics.Conditional("ENABLE_LOGGING")]
        public static void LogMethod<T>(this T obj, [CallerMemberName] string methodName = "") where T : class
        {
            Debug.Log($"[{typeof(T).Name}::{methodName}] Called", obj as Object);
        }
         
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        [System.Diagnostics.Conditional("ENABLE_LOGGING")]
        public static void LogCategory<T>(this T obj, string category, string message) where T : class
        {
            Debug.Log($"[{typeof(T).Name}][{category}] {message}", obj as Object);
        }
         
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        [System.Diagnostics.Conditional("ENABLE_LOGGING")]
        public static void LogStateChange<T, TValue>(this T obj, string propertyName, TValue oldValue, TValue newValue) where T : class
        {
            Debug.Log($"[{typeof(T).Name}] {propertyName}: {oldValue} → {newValue}", obj as Object);
        }
         
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        [System.Diagnostics.Conditional("ENABLE_LOGGING")]
        public static void LogTiming<T>(this T obj, string operation, System.TimeSpan duration) where T : class
        {
            Debug.Log($"[{typeof(T).Name}] {operation} completed in {duration.TotalMilliseconds:F2}ms", obj as Object);
        }
    } 
} 
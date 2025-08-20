#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using GuidSystem.Runtime;

namespace GuidSystem.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Direct reflection approach - package içinde olduğumuz için daha güvenli
            if (fieldInfo != null)
            {
                var targetObject = property.serializedObject.targetObject;
                var fieldValue = fieldInfo.GetValue(targetObject);
                
                if (fieldValue is SerializableGuid guidValue)
                {
                    // GUID'i string olarak göster
                    EditorGUI.BeginDisabledGroup(true);
                    string guidString = guidValue.ToGuid().ToString();
                    EditorGUI.TextField(position, label.text, guidString);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "Invalid GUID");
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "GUID (No FieldInfo)");
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIKit
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UIKActionObjectReference))]
    public class UIKActionObjectReferencePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = $"({nameof(UIKActionObjectReference)}) {label.text}";
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            UIKActionObjectReflector.RefreshActionObjects();
            List<UIKActionObject> actionObjects = UIKActionObjectReflector.GetAllActionObjects();

            List<string> actionObjectOptions = new();
            foreach (UIKActionObject actionObject in actionObjects)
            {
                actionObjectOptions.Add($"{actionObject.GetType().Name}");
            }
            
            if (actionObjectOptions.Count > 0)
            {
                actionObjects.Insert(0, null);
                actionObjectOptions.Insert(0, "Invalid");
            }
            else
            {
                actionObjects.Insert(0, null);
                actionObjectOptions.Insert(0, "Invalid (No Action Objects Not Found)");
            }
            
            SerializedProperty typeProperty = property.FindPropertyRelative("type");
            
            int indexOfAction = actionObjects.FindIndex(i =>
            {
                return i != null
                    && !string.IsNullOrEmpty(typeProperty.stringValue)
                    && i.GetType().Name == typeProperty.stringValue;
            });
            indexOfAction = Mathf.Max(indexOfAction, 0);
            
            // Display a dropdown of natively defined tags on the next line
            Rect keyFieldRect = new Rect(position.x, position.y, position.width, position.height);
            int clickedOption = EditorGUI.Popup(keyFieldRect, indexOfAction, actionObjectOptions.ToArray());

            if (indexOfAction != clickedOption)
            {
                if (clickedOption > actionObjects.Count) // The value was somehow too large
                {
                    clickedOption = actionObjects.Count;
                }
                else if (clickedOption < 0) // The value was somehow too small
                {
                    clickedOption = 0;
                }
                
                // Set the value to match the selected option
                if (clickedOption == 0)
                {
                    typeProperty.stringValue = string.Empty;
                }
                else
                {
                    typeProperty.stringValue = actionObjects[clickedOption].GetType().Name;
                }
            }

            EditorGUI.EndProperty();
        }
    }
#endif // UNITY_EDITOR
} // UIKit namespace

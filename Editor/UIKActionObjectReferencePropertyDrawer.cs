using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

            List<string> actionObjectOptions = new();
            foreach (Type actionObjectType in UIKActionObjectReflector.GetAllActionObjectTypes())
            {
                actionObjectOptions.Add(actionObjectType.Name);
            }
            
            if (actionObjectOptions.Count > 0)
            {
                actionObjectOptions.Insert(0, "Invalid");
            }
            else
            {
                actionObjectOptions.Insert(0, "Invalid (No Action Object Types Not Found)");
            }
            
            SerializedProperty typeProperty = property.FindPropertyRelative(nameof(UIKActionObjectReference.type));
            
            int indexOfAction = actionObjectOptions.FindIndex(s => s == typeProperty.stringValue);
            indexOfAction = Mathf.Max(indexOfAction, 0);
            
            // Display a dropdown of natively defined tags on the next line
            Rect keyFieldRect = new(position.x, position.y, position.width, position.height);
            int clickedOption = EditorGUI.Popup(keyFieldRect, indexOfAction, actionObjectOptions.ToArray());

            if (indexOfAction != clickedOption)
            {
                if (clickedOption > actionObjectOptions.Count) // The value was somehow too large
                {
                    clickedOption = actionObjectOptions.Count;
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
                    typeProperty.stringValue = actionObjectOptions[clickedOption];
                }
            }

            EditorGUI.EndProperty();
        }
    }
#endif // UNITY_EDITOR
} // UIKit namespace

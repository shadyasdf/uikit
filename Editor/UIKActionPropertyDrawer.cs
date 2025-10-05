using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIKit
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UIKInputAction))]
    public class UIKActionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = $"({nameof(UIKInputAction)}) {label.text}";
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            List<InputAction> inputActions = new();
            List<InputActionAsset> inputActionAssets = UIKActionReflector.GetAllInputActionAssets();
            foreach (InputActionAsset inputActionAsset in inputActionAssets)
            {
                foreach (InputActionMap inputActionMap in inputActionAsset.actionMaps)
                {
                    foreach (InputAction inputAction in inputActionMap.actions)
                    {
                        inputActions.Add(inputAction);
                    }
                }
            }

            List<string> inputActionOptions = new();
            foreach (InputAction inputAction in inputActions)
            {
                inputActionOptions.Add($"{inputAction.actionMap.asset.name}/{inputAction.actionMap.name}/{inputAction.name}");
            }
            
            if (inputActionOptions.Count > 0)
            {
                inputActions.Insert(0, null);
                inputActionOptions.Insert(0, "Invalid");
            }
            else
            {
                inputActions.Insert(0, null);
                inputActionOptions.Insert(0, "Invalid (No Input Actions Not Found)");
            }
            
            SerializedProperty assetProperty = property.FindPropertyRelative("asset");
            SerializedProperty actionMapProperty = property.FindPropertyRelative("actionMap");
            SerializedProperty actionProperty = property.FindPropertyRelative("action");
            
            int indexOfAction = inputActions.FindIndex(i =>
            {
                return i != null
                    && i.name == actionProperty.stringValue
                    && i.actionMap != null
                    && i.actionMap.name == actionMapProperty.stringValue
                    && i.actionMap.asset
                    && i.actionMap.asset.name == assetProperty.stringValue;
            });
            indexOfAction = Mathf.Max(indexOfAction, 0);
            
            // Display a dropdown of natively defined tags on the next line
            Rect keyFieldRect = new Rect(position.x, position.y, position.width, position.height);
            int clickedOption = EditorGUI.Popup(keyFieldRect, indexOfAction, inputActionOptions.ToArray());

            if (indexOfAction != clickedOption)
            {
                if (clickedOption > inputActions.Count) // The value was somehow too large
                {
                    clickedOption = inputActions.Count;
                }
                else if (clickedOption < 0) // The value was somehow too small
                {
                    clickedOption = 0;
                }
                
                // Set the value to match the selected option
                if (clickedOption == 0)
                {
                    assetProperty.stringValue = string.Empty;
                    actionMapProperty.stringValue = string.Empty;
                    actionProperty.stringValue = string.Empty;
                }
                else
                {
                    assetProperty.stringValue = inputActions[clickedOption].actionMap.asset.name;
                    actionMapProperty.stringValue = inputActions[clickedOption].actionMap.name;
                    actionProperty.stringValue = inputActions[clickedOption].name;
                }
            }

            EditorGUI.EndProperty();
        }
    }
#endif // UNITY_EDITOR
} // UIKit namespace

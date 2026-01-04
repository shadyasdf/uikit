using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIKit
{
    [CustomPropertyDrawer(typeof(UIKActionMap))]
    public class UIKActionMapPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = $"({nameof(UIKActionMap)}) {label.text}";
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            List<InputActionMap> actionMaps = new();
            List<InputActionAsset> inputActionAssets = UIKInputActionAssetReflector.GetAllInputActionAssets();
            foreach (InputActionAsset inputActionAsset in inputActionAssets)
            {
                foreach (InputActionMap inputActionMap in inputActionAsset.actionMaps)
                {
                    actionMaps.Add(inputActionMap);
                }
            }

            List<string> actionMapOptions = new();
            foreach (InputActionMap actionMap in actionMaps)
            {
                actionMapOptions.Add($"{actionMap.asset.name}/{actionMap.name}");
            }
            
            if (actionMapOptions.Count > 0)
            {
                actionMaps.Insert(0, null);
                actionMapOptions.Insert(0, "Invalid");
            }
            else
            {
                actionMaps.Insert(0, null);
                actionMapOptions.Insert(0, "Invalid (No Action Maps Not Found)");
            }
            
            SerializedProperty assetProperty = property.FindPropertyRelative("asset");
            SerializedProperty actionMapProperty = property.FindPropertyRelative("name");
            
            int indexOfAction = actionMaps.FindIndex(i =>
            {
                return i != null
                    && i.name == actionMapProperty.stringValue
                    && i.asset
                    && i.asset.name == assetProperty.stringValue;
            });
            indexOfAction = Mathf.Max(indexOfAction, 0);
            
            // Display a dropdown of natively defined tags on the next line
            Rect keyFieldRect = new Rect(position.x, position.y, position.width, position.height);
            int clickedOption = EditorGUI.Popup(keyFieldRect, indexOfAction, actionMapOptions.ToArray());

            if (indexOfAction != clickedOption)
            {
                if (clickedOption > actionMaps.Count) // The value was somehow too large
                {
                    clickedOption = actionMaps.Count;
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
                }
                else
                {
                    assetProperty.stringValue = actionMaps[clickedOption].asset.name;
                    actionMapProperty.stringValue = actionMaps[clickedOption].name;
                }
            }

            EditorGUI.EndProperty();
        }
    }
} // UIKit namespace

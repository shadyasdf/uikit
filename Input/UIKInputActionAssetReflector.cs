using System.Collections.Generic;
using UnityEditor;
using UnityEngine.InputSystem;

namespace UIKit
{
#if UNITY_EDITOR
    public static class UIKInputActionAssetReflector
    {
        private static List<InputActionAsset> inputActionAssets = new();
        
        
        static UIKInputActionAssetReflector()
        {
            // Search for all assets of type InputActionAsset
            string[] guids = AssetDatabase.FindAssets("t:InputActionAsset");
            foreach (string guid in guids)
            {
                if (AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetDatabase.GUIDToAssetPath(guid)) is InputActionAsset inputActionAsset)
                {
                    inputActionAssets.Add(inputActionAsset);
                }
            }
        }

        public static List<InputActionAsset> GetAllInputActionAssets()
        {
            return inputActionAssets;
        }
    }
#endif // UNITY_EDITOR
} // UIKit namespace

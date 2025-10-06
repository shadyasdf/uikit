using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIKit
{
    [Serializable]
    public class UIKInputAction
    {
        public string asset;
        public string actionMap;
        public string action;

        
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(asset)
                && !string.IsNullOrEmpty(actionMap)
                && !string.IsNullOrEmpty(action);
        }
        
        public override string ToString()
        {
            if (IsValid())
            {
                return $"({nameof(UIKInputAction)}) {asset}/{actionMap}/{action}";
            }
            
            return $"({nameof(UIKInputAction)}) null";
        }

        public static implicit operator UIKInputAction(InputAction _inputAction)
        {
            if (_inputAction != null
                && _inputAction.actionMap != null
                && _inputAction.actionMap.asset != null)
            {
                return new UIKInputAction()
                    {
                        asset = _inputAction.actionMap.asset.name,
                        actionMap = _inputAction.actionMap.name,
                        action = _inputAction.name
                    };
            }

            return new UIKInputAction();
        }

        public override bool Equals(object _obj)
        {
            if (_obj == null
                && !IsValid())
            {
                return true;
            }

            if (_obj is UIKInputAction uikInputAction)
            {
                return uikInputAction.asset == asset
                    && uikInputAction.actionMap == actionMap
                    && uikInputAction.action == action;
            }
            
            if (_obj is InputAction inputAction)
            {
                return inputAction.actionMap != null
                    && inputAction.actionMap.asset != null
                    && inputAction.actionMap.asset.name == asset
                    && inputAction.actionMap.name == actionMap
                    && inputAction.name == action;
            }

            return false;
        }
        
        protected bool Equals(UIKInputAction _other)
        {
            return asset == _other.asset
               && actionMap == _other.actionMap
               && action == _other.action;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(asset, actionMap, action);
        }
        
        public static bool operator ==(UIKInputAction _lhs, UIKInputAction _rhs)
        {
            if (_lhs is null && _rhs is null) return true;
            if (_lhs is null || _rhs is null) return false;
            return _lhs.Equals(_rhs);
        }

        public static bool operator !=(UIKInputAction _lhs, UIKInputAction _rhs)
        {
            return !(_lhs == _rhs);
        }

        public static bool operator ==(UIKInputAction _lhs, InputAction _rhs)
        {
            if (_lhs is null && _rhs is null) return true;
            if (_lhs is null || _rhs is null) return false;
            return _lhs.Equals(_rhs); // UIKInputAction does handle .Equals with InputAction, use UIKInputAction as lhs
        }

        public static bool operator !=(UIKInputAction _lhs, InputAction _rhs)
        {
            return !(_lhs == _rhs);
        }
        
        public static bool operator ==(InputAction _lhs, UIKInputAction _rhs)
        {
            if (_lhs is null && _rhs is null) return true;
            if (_lhs is null || _rhs is null) return false;
            return _rhs.Equals(_lhs); // InputAction doesn't handle .Equals with UIKInputAction, use UIKInputAction as lhs
        }

        public static bool operator !=(InputAction _lhs, UIKInputAction _rhs)
        {
            return !(_lhs == _rhs);
        }
    }
    
#if UNITY_EDITOR
    public static class UIKInputActionReflector
    {
        private static List<InputActionAsset> inputActionAssets = new();
        
        
        static UIKInputActionReflector()
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

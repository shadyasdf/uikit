using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIKit
{
    [Serializable]
    public class UIKActionMap
    {
        public string asset;
        public string name;

        
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(asset)
                && !string.IsNullOrEmpty(name);
        }
        
        public override string ToString()
        {
            if (IsValid())
            {
                return $"({nameof(UIKActionMap)}) {asset}/{name}";
            }
            
            return $"({nameof(UIKActionMap)}) null";
        }

        public static implicit operator UIKActionMap(InputActionMap _actionMap)
        {
            if (_actionMap != null
                && _actionMap.asset != null)
            {
                return new UIKActionMap()
                    {
                        asset = _actionMap.asset.name,
                        name = _actionMap.name
                    };
            }

            return new UIKActionMap();
        }

        public override bool Equals(object _obj)
        {
            if (_obj == null
                && !IsValid())
            {
                return true;
            }

            if (_obj is UIKActionMap uikActionMap)
            {
                return uikActionMap.asset == asset
                    && uikActionMap.name == name;
            }
            
            if (_obj is InputActionMap otherActionMap)
            {
                return otherActionMap.asset
                    && otherActionMap.asset.name == asset
                    && otherActionMap.name == name;
            }

            return false;
        }
        
        protected bool Equals(UIKActionMap _other)
        {
            return asset == _other.asset
               && name == _other.name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(asset, name);
        }
        
        public static bool operator ==(UIKActionMap _lhs, UIKActionMap _rhs)
        {
            if (_lhs is null && _rhs is null) return true;
            if (_lhs is null || _rhs is null) return false;
            return _lhs.Equals(_rhs);
        }

        public static bool operator !=(UIKActionMap _lhs, UIKActionMap _rhs)
        {
            return !(_lhs == _rhs);
        }

        public static bool operator ==(UIKActionMap _lhs, InputActionMap _rhs)
        {
            if (_lhs is null && _rhs is null) return true;
            if (_lhs is null || _rhs is null) return false;
            return _lhs.Equals(_rhs); // UIKActionMap does handle .Equals with InputAction, use UIKActionMap as lhs
        }

        public static bool operator !=(UIKActionMap _lhs, InputActionMap _rhs)
        {
            return !(_lhs == _rhs);
        }
        
        public static bool operator ==(InputActionMap _lhs, UIKActionMap _rhs)
        {
            if (_lhs is null && _rhs is null) return true;
            if (_lhs is null || _rhs is null) return false;
            return _rhs.Equals(_lhs); // InputActionMap doesn't handle .Equals with UIKActionMap, use UIKActionMap as lhs
        }

        public static bool operator !=(InputActionMap _lhs, UIKActionMap _rhs)
        {
            return !(_lhs == _rhs);
        }
    }
} // UIKit namespace

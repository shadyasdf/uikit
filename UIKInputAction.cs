using System;
using UnityEngine.InputSystem;

namespace UIKit
{
    [Serializable]
    public class UIKInputAction
    {
        public string asset; // Don't use for comparison, additional local player inputs append "(Clone)" to the end of the asset name
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

        public static explicit operator UIKInputAction(InputAction _inputAction)
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
            if (_obj == null)
            {
                if (!IsValid())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (_obj is UIKInputAction uikInputAction)
            {
                return uikInputAction.actionMap == actionMap
                       && uikInputAction.action == action;
            }

            if (_obj is InputAction inputAction)
            {
                return inputAction.actionMap != null
                       && inputAction.actionMap.name == actionMap
                       && inputAction.name == action;
            }

            return false;
        }

        protected bool Equals(UIKInputAction _other)
        {
            return actionMap == _other.actionMap
                   && action == _other.action;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(actionMap, action);
        }

        public static bool operator ==(UIKInputAction _lhs, object _rhs)
        {
            if (_lhs is null && _rhs is null) return true;
            if (_lhs is null || _rhs is null) return false;
            return _lhs.Equals(_rhs);
        }

        public static bool operator !=(UIKInputAction _lhs, object _rhs)
        {
            return !(_lhs == _rhs);
        }
    }
} // UIKit namespace

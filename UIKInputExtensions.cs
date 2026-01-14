using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIKit
{
    public static class UIKInputExtensions
    {
        public static InputDevice[] GetAllInputDevices()
        {
            return InputSystem.devices.ToArray();
        }
        
        public static InputDevice[] GetFirstInputDevices(this UIKInputDevice _inputDevice)
        {
            List<InputDevice> inputDevices = new();

            foreach (InputDevice inputDevice in InputSystem.devices)
            {
                UIKInputDevice inputDeviceType = inputDevice.GetInputDeviceType();
                if (inputDeviceType != UIKInputDevice.NONE
                    && inputDeviceType.HasAnyFlags(_inputDevice))
                {
                    inputDevices.Add(inputDevice);
                }
            }

            return inputDevices.ToArray();
        }

        public static UIKInputDevice GetInputDeviceFromString(string _string)
        {
            switch (_string)
            {
                case "Keyboard": return UIKInputDevice.Keyboard;
                case "Mouse": return UIKInputDevice.Mouse;
                case "Gamepad": return UIKInputDevice.Gamepad;
            }

            return UIKInputDevice.NONE;
        }

        public static UIKInputDevice GetInputDeviceType(this InputAction.CallbackContext _context)
        {
            return _context.control.device.GetInputDeviceType();
        }

        public static UIKInputDevice GetInputDeviceType(this InputDevice _inputDevice)
        {
            switch (_inputDevice.name)
            {
                case "Mouse":
                    return UIKInputDevice.Mouse;
                case "Keyboard":
                    return UIKInputDevice.Keyboard;
                case "Touchscreen":
                    return UIKInputDevice.Touch;
                case "Pen":
                    goto case null;
                case "Gamepad":
                    goto case default;
                case null:
                    Debug.LogWarning($"Unknown input device displayName: {_inputDevice.name}");
                    return UIKInputDevice.NONE;
                // We can't possibly know all the names of the various gamepads, so we will treat the default non-null device as a Gamepad to be safe
                default:
                    return UIKInputDevice.Gamepad;
            }
        }

        public static bool UsesCursor(this UIKInputDevice _inputDeviceType)
        {
            return _inputDeviceType.HasFlag(UIKInputDevice.Mouse) // This check is usually enough, but the other two are for safety
                || _inputDeviceType == UIKInputDevice.Mouse
                || _inputDeviceType == UIKInputDevice.MouseAndKeyboard;
        }

        public static UIKInputDirection Invert(this UIKInputDirection _direction)
        {
            switch (_direction)
            {
                case UIKInputDirection.Up:
                    return UIKInputDirection.Down;
                case UIKInputDirection.Right:
                    return UIKInputDirection.Left;
                case UIKInputDirection.Down:
                    return UIKInputDirection.Up;
                case UIKInputDirection.Left:
                    return UIKInputDirection.Right;
            }

            return _direction;
        }
        
        public static UIKInputDirection GetInputDirection(this Vector2 _direction)
        {
            // The native Selectable.FindSelectable(Vector3 dir) will ignore explicit navigation rules,
            // so we need to convert our input Vector2 direction into our enum so they don't navigate
            // to places they shouldn't

            float directionAngle = Vector2.SignedAngle(Vector2.right, _direction);

            UIKInputDirection direction = UIKInputDirection.Right;
            if (directionAngle < 45.0f && directionAngle >= -45.0f)
            {
                direction = UIKInputDirection.Right;
            }
            else if (directionAngle < 135.0f && directionAngle >= 45.0f)
            {
                direction = UIKInputDirection.Up;
            }
            else if (directionAngle < -135.0f || directionAngle >= 135.0f)
            {
                direction = UIKInputDirection.Left;
            }
            else if (directionAngle < -45.0f && directionAngle >= -135.0f)
            {
                direction = UIKInputDirection.Down;
            }

            return direction;
        }
    }
} // UIKit namespace

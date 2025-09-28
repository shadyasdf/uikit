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
    }
} // UIKit namespace

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UIKit
{
    public interface UIKPlayer
    {
        public UnityEvent<InputAction> OnInputActionTriggered { get; set; }
        
        public PlayerInput playerInput { get; set; }
        public UIKSelectable selectedUI { get; set; }
        public UIKInputDevice inputDeviceType { get; set; }


        /// <returns>Whether to broadcast this input action</returns>
        public bool OnPreInputActionTriggered(InputAction.CallbackContext _context);
        
        /// <summary>
        /// Have this function return the result of UIKPlayer.GetInputDevices(this)
        /// </summary>
        public InputDevice[] GetInputDevices();

        public static InputDevice[] GetInputDevices(UIKPlayer _player)
        {
            return _player.playerInput.devices.ToArray();
        }
        
        protected void OnSelectedUIChanged(UIKSelectable _oldSelectable, UIKSelectable _newSelectable);

        /// <summary>
        /// Have this function return the result of UIKPlayer.GetSelectedUI(this)
        /// </summary>
        public UIKSelectable GetSelectedUI();

        public static UIKSelectable GetSelectedUI(UIKPlayer _player)
        {
            return _player.selectedUI;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.TrySelectUI(this, _selectable, _force, _executeUIEvent)
        /// </summary>
        public bool TrySelectUI(UIKSelectable _selectable, bool _force = false, bool _executeUIEvent = true);

        public static bool TrySelectUI(UIKPlayer _player, UIKSelectable _selectable, bool _force = false, bool _executeUIEvent = true)
        {
            if (!_selectable)
            {
                return false;
            }

            // We already have this selected
            if (_selectable.selectedByPlayers.Contains(_player))
            {
                return false;
            }
            
            if (!_force)
            {
                if (!_selectable.CanPlayerSelect(_player))
                {
                    return false;
                }

                if (_player.selectedUI != null
                    && !_player.selectedUI.CanPlayerDeselect(_player))
                {
                    return false;
                }
            }

            // Deselect previous selection
            if (_player.selectedUI != null
                && _player.selectedUI.selectedByPlayers.Contains(_player))
            {
                _player.selectedUI.selectedByPlayers.RemoveAll(p => p == _player);

                if (_executeUIEvent)
                {
                    UIKEventSystem.instance.ExecuteUIEvent(_player, _player.selectedUI, ExecuteEvents.deselectHandler);
                }
            }

            // Update the player's selected UI
            UIKSelectable oldSelectable = _player.selectedUI;
            _player.selectedUI = _selectable;
            if (!_player.selectedUI.selectedByPlayers.Contains(_player))
            {
                _player.selectedUI.selectedByPlayers.Add(_player);
            }

            if (_executeUIEvent)
            {
                UIKEventSystem.instance.ExecuteUIEvent(_player, _player.selectedUI, ExecuteEvents.selectHandler);
            }
            _player.OnSelectedUIChanged(oldSelectable, _player.selectedUI);

            return true;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.TryDeselectUI(this, _force, _executeUIEvent)
        /// </summary>
        public bool TryDeselectUI(bool _force = false, bool _executeUIEvent = true);

        public static bool TryDeselectUI(UIKPlayer _player, bool _force = false, bool _executeUIEvent = true)
        {
            if (!_player.selectedUI)
            {
                return true;
            }
            
            if (!_force)
            {
                if (!_player.selectedUI.CanPlayerDeselect(_player))
                {
                    return false;
                }
            }

            // Deselect previous selection
            if (_player.selectedUI.selectedByPlayers.Contains(_player))
            {
                _player.selectedUI.selectedByPlayers.RemoveAll(p => p == _player);

                if (_executeUIEvent)
                {
                    UIKEventSystem.instance.ExecuteUIEvent(_player, _player.selectedUI, ExecuteEvents.deselectHandler);
                }
            }

            // Update the player's selected UI
            UIKSelectable oldSelectable = _player.selectedUI;
            _player.selectedUI = null;
            _player.OnSelectedUIChanged(oldSelectable, _player.selectedUI);

            return true;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.TryNavigateUIByDirection(this, _direction)
        /// </summary>
        public bool TryNavigateUIByDirection(Vector2 _direction);

        public static bool TryNavigateUIByDirection(UIKPlayer _player, Vector2 _direction)
        {
            if (!_player.selectedUI)
            {
                return _player.TrySelectUI(UIKSelectable.GetPlayerFirstSelection(_player));
            }

            UIKSelectable foundUI = _player.selectedUI.FindUI(_direction);
            if (foundUI != null)
            {
                return _player.TrySelectUI(foundUI);
            }

            return false;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.TryNavigateUIByDirection(this, _direction)
        /// </summary>
        public bool TryNavigateUIByDirection(UIKInputDirection _direction);

        public static bool TryNavigateUIByDirection(UIKPlayer _player, UIKInputDirection _direction)
        {
            if (!_player.selectedUI)
            {
                return _player.TrySelectUI(UIKSelectable.GetPlayerFirstSelection(_player));
            }

            UIKSelectable foundUI = _player.selectedUI.FindUI(_direction);
            if (foundUI != null)
            {
                return _player.TrySelectUI(foundUI);
            }

            return false;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.TrySubmitUI(this, _selectable, _force, _executeUIEvent)
        /// </summary>
        public bool TrySubmitUI(UIKSelectable _selectable, bool _force = false, bool _executeUIEvent = true);

        public static bool TrySubmitUI(UIKPlayer _player, UIKSelectable _selectable, bool _force = false, bool _executeUIEvent = true)
        {
            if (!_force)
            {
                if (!_player.selectedUI)
                {
                    return false;
                }

                // If we're using a mouse, don't submit on anything unless we have it selected
                if (_player.inputDeviceType.UsesCursor()
                    && !_player.selectedUI.selectedByPlayers.Contains(_player))
                {
                    return false;
                }

                if (!_player.selectedUI.CanPlayerSubmit(_player))
                {
                    return false;
                }
            }

            if (_executeUIEvent)
            {
                UIKEventSystem.instance.ExecuteUIEvent(_player, _player.selectedUI, ExecuteEvents.submitHandler);
            }
            
            return true;
        }
    }
} // UIKit namespace

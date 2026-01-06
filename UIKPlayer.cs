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
        public UIKCanvas canvas { get; set; }
        public UIKTarget targetUI { get; set; }
        public UIKInputDevice inputDeviceType { get; set; }


        /// <summary>Run any logic that you need done before OnPlayerJoined is called</summary>
        public void OnPrePlayerJoined();
        
        /// <returns>Whether this player is locally controlled</returns>
        public bool GetIsLocal();
        
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
        
        protected void OnTargetUIChanged(UIKTarget _oldTarget, UIKTarget _newTarget);

        /// <summary>
        /// Have this function return the result of UIKPlayer.GetTargetedUI(this)
        /// </summary>
        public UIKTarget GetTargetUI();

        public static UIKTarget GetTargetUI(UIKPlayer _player)
        {
            return _player.targetUI;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.SelectUI(this, _targetable)
        /// </summary>
        public bool SelectUI(UIKTarget _target);

        public static bool SelectUI(UIKPlayer _player, UIKTarget _target)
        {
            UIKEventSystem.instance.ExecuteUIEvent(_player, _target, ExecuteEvents.selectHandler);

            return true;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.DeselectUI(this)
        /// </summary>
        public bool DeselectUI();

        public static bool DeselectUI(UIKPlayer _player)
        {
            UIKEventSystem.instance.ExecuteUIEvent(_player, _player.targetUI, ExecuteEvents.deselectHandler);

            return true;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.TryNavigateUIByDirection(this, _direction)
        /// </summary>
        public bool TryNavigateUIByDirection(Vector2 _direction);

        public static bool TryNavigateUIByDirection(UIKPlayer _player, Vector2 _direction)
        {
            if (!_player.targetUI)
            {
                if (UIKTarget.GetPlayerFirstTarget(_player) is UIKTarget target)
                {
                    return _player.SelectUI(target);
                }
                else
                {
                    return false;
                }
            }

            UIKTarget foundUI = _player.targetUI.FindUI(_direction);
            if (foundUI != null)
            {
                return _player.SelectUI(foundUI);
            }

            return false;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.TryNavigateUIByDirection(this, _direction)
        /// </summary>
        public bool TryNavigateUIByDirection(UIKInputDirection _direction);

        public static bool TryNavigateUIByDirection(UIKPlayer _player, UIKInputDirection _direction)
        {
            if (!_player.targetUI)
            {
                return _player.SelectUI(UIKTarget.GetPlayerFirstTarget(_player));
            }

            UIKTarget foundUI = _player.targetUI.FindUI(_direction);
            if (foundUI != null)
            {
                return _player.SelectUI(foundUI);
            }

            return false;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.TryTargetUI(this, _targetable)
        /// </summary>
        public bool TryTargetUI(UIKTarget _target);

        public static bool TryTargetUI(UIKPlayer _player, UIKTarget _target)
        {
            if (!_target)
            {
                return false;
            }

            // We already have this targeted
            if (_target.targetedByPlayers.Contains(_player))
            {
                return false;
            }
            
            if (!_target.CanPlayerTarget(_player))
            {
                return false;
            }

            if (_player.targetUI != null
                && !_player.targetUI.CanPlayerUntarget(_player))
            {
                return false;
            }

            // Untarget previous target
            if (_player.targetUI != null
                && _player.targetUI.targetedByPlayers.Contains(_player))
            {
                if (_player.targetUI.targetedByPlayers.RemoveAll(p => p == _player) > 0)
                {
                    _player.targetUI.HandleUntargeted(_player);
                }
            }

            // Update the player's selected UI
            UIKTarget oldTarget = _player.targetUI;
            _player.targetUI = _target;
            if (!_player.targetUI.targetedByPlayers.Contains(_player))
            {
                _player.targetUI.targetedByPlayers.Add(_player);
                _player.targetUI.HandleTargeted(_player);
            }
            
            _player.OnTargetUIChanged(oldTarget, _player.targetUI);

            return true;
        }

        /// <summary>
        /// Have this function return the result of UIKPlayer.TryUntargetUI(this, _target)
        /// </summary>
        public bool TryUntargetUI(UIKTarget _target = null);

        public static bool TryUntargetUI(UIKPlayer _player, UIKTarget _target)
        {
            if (!_target
                && _player.targetUI)
            {
                _target = _player.targetUI;
            }
            
            if (!_target)
            {
                return true;
            }
            
            if (!_target.CanPlayerUntarget(_player))
            {
                return false;
            }

            // Untarget previous target
            if (_target.targetedByPlayers.Contains(_player))
            {
                if (_target.targetedByPlayers.RemoveAll(p => p == _player) > 0)
                {
                    _target.HandleUntargeted(_player);
                }
            }

            // Update the player's target UI
            _player.targetUI = null;
            _player.OnTargetUIChanged(_target, _player.targetUI);

            return true;
        }
        
        /// <summary>
        /// Have this function return the result of UIKPlayer.TrySubmitUI(this, _targetable)
        /// </summary>
        public bool TrySubmitUI(UIKTarget _target);

        public static bool TrySubmitUI(UIKPlayer _player, UIKTarget _target)
        {
            if (!_player.targetUI)
            {
                return false;
            }

            if (_player.targetUI != _target)
            {
                return false;
            }

            // If we're using a mouse, don't submit on anything unless we have it selected
            if (_player.inputDeviceType.UsesCursor()
                && !_player.targetUI.targetedByPlayers.Contains(_player))
            {
                return false;
            }

            if (!_player.targetUI.CanPlayerSubmit(_player))
            {
                return false;
            }

            UIKEventSystem.instance.ExecuteUIEvent(_player, _player.targetUI, ExecuteEvents.submitHandler);
            
            return true;
        }
    }
} // UIKit namespace

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UIKit
{
    public interface UIKPlayerManager
    {
        public UnityEvent<UIKPlayer> OnPlayerJoined { get; set; }
        public UnityEvent<UIKPlayer> OnPlayerLeft { get; set; }
        
        public List<UIKPlayer> players { get; set; }
        
        /// <summary>
        /// This must be set manually in your project's code
        /// </summary>
        public static UIKPlayerManager instance;

        
        public List<UIKPlayer> localPlayers => players.Where(p => p.GetIsLocal()).ToList();
        
        /// <summary>
        /// Have this function return the result of UIKPlayerManager.SpawnPlayer(_playerManager, _playerIndex, _splitScreenIndex, _controlScheme, _inputDevices)
        /// </summary>
        public UIKPlayer JoinPlayer(int _playerIndex = -1, int _splitScreenIndex = -1, string _controlScheme = null, InputDevice[] _inputDevices = null);

        public static UIKPlayer JoinPlayer(UIKPlayerManager _playerManager, int _playerIndex, int _splitScreenIndex, string _controlScheme, InputDevice[] _inputDevices = null)
        {
            if (PlayerInputManager.instance == null)
            {
                return null;
            }
            
            PlayerInput playerInput = PlayerInputManager.instance.JoinPlayer(_playerIndex, _splitScreenIndex, _controlScheme, _inputDevices);
            if (playerInput.GetComponent<UIKPlayer>() is not UIKPlayer player)
            {
                return null;
            }
            
            player.playerInput = playerInput;
            
            _playerManager.players.Add(player);
            
            // Initialize the player
            MonoBehaviour.DontDestroyOnLoad(playerInput.gameObject);
            playerInput.onActionTriggered += PlayerInput_OnActionTriggered;
            playerInput.onControlsChanged += PlayerInput_OnControlsChanged;
            PlayerInput_OnControlsChanged(playerInput);
            
            player.OnPrePlayerJoined();
            _playerManager.OnPlayerJoined?.Invoke(player);
            
            return player;
        }
        
        private static void PlayerInput_OnActionTriggered(InputAction.CallbackContext _context)
        {
            // The device used to trigger the action
            // _context.action.activeControl.device

            if (!Application.isFocused)
            {
                return;
            }
            
            // For all players using the device that the input action was triggered on
            foreach (UIKPlayer player in instance.players.Where(p => p.GetInputDevices().Contains(_context.action.activeControl.device)))
            {
                // Canvas gets first dibs on consuming input actions, if we have one
                if (player.canvas)
                {
                    if (!player.canvas.OnPreInputActionTriggered(player, _context))
                    {
                        continue; // If we returned false, don't do any further input handling
                    }
                }
                
                // Player gets second dibs on consuming input actions
                if (!player.OnPreInputActionTriggered(_context))
                {
                    continue; // If we returned false, don't do any further input handling
                }
                
                // Broadcast the input for anyone to listen to
                player.OnInputActionTriggered.Invoke(_context.action);
            }
        }
        
        private static void PlayerInput_OnControlsChanged(PlayerInput _playerInput)
        {
            if (instance.players.FirstOrDefault(p => p.playerInput == _playerInput) is UIKPlayer player)
            {
                if (_playerInput.devices.Count == 1)
                {
                    if (_playerInput.devices[0] == null)
                    {
                        Debug.LogError("Player's playerInput device was null");
                        return;
                    }

                    player.inputDeviceType = _playerInput.devices[0].GetInputDeviceType();
                    return;
                }
                else if (_playerInput.devices.Count == 2)
                {
                    bool hasMouse = false;
                    bool hasKeyboard = false;

                    foreach (InputDevice inputDevice in _playerInput.devices)
                    {
                        if (inputDevice.GetInputDeviceType() == UIKInputDevice.Mouse)
                        {
                            hasMouse = true;
                            continue;
                        }

                        if (inputDevice.GetInputDeviceType() == UIKInputDevice.Keyboard)
                        {
                            hasKeyboard = true;
                            continue;
                        }
                    }

                    if (hasKeyboard && hasMouse)
                    {
                        player.inputDeviceType = UIKInputDevice.MouseAndKeyboard;
                        return;
                    }

                    Debug.LogError("Failed to find valid InputDeviceType for given set of 2 devices");
                    return;
                }
                else
                {
                    Debug.LogError("Failed to find valid playerInput device");
                    return;
                }
            }
        }
    }
} // UIKit namespace

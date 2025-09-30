using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace UIKit
{
    public interface UIKPlayerManager
    {
        public static UIKPlayerManager instance;


        public List<UIKPlayer> GetPlayers();
        
        protected void OnPlayerSpawned(UIKPlayer _player);
        
        /// <summary> Have this function return the result of UIKPlayerManager.SpawnPlayer(_playerManager, _playerIndex, _splitScreenIndex, _controlScheme, _inputDevices) </summary>
        public UIKPlayer SpawnPlayer(int _playerIndex = -1, int _splitScreenIndex = -1, string _controlScheme = null, InputDevice[] _inputDevices = null);

        public static UIKPlayer SpawnPlayer(UIKPlayerManager _playerManager, int _playerIndex, int _splitScreenIndex, string _controlScheme, InputDevice[] _inputDevices = null)
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
            
            _playerManager.OnPlayerSpawned(player);
            
            return player;
        }
    }
} // UIKit namespace

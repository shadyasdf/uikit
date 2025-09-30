using UnityEngine;
using UnityEngine.EventSystems;

namespace UIKit
{
    public interface UIKPlayer
    {
        public UIKSelectable selectedUI { get; set; }

        
        protected void OnSelectedUIChanged(UIKSelectable _oldSelectable, UIKSelectable _newSelectable);

        public UIKInputDevice GetInputDeviceType();

        /// <summary> Have this function return the result of UIKPlayer.GetSelectedUI(this) </summary>
        public UIKSelectable GetSelectedUI();

        public static UIKSelectable GetSelectedUI(UIKPlayer _player)
        {
            return _player.selectedUI;
        }

        /// <summary> Have this function return the result of UIKPlayer.TrySelectUI(this, _selectable) </summary>
        public bool TrySelectUI(UIKSelectable _selectable);

        public static bool TrySelectUI(UIKPlayer _player, UIKSelectable _selectable)
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

            if (!_selectable.CanPlayerSelect(_player))
            {
                return false;
            }

            if (_player.selectedUI != null
                && !_player.selectedUI.CanPlayerDeselect(_player))
            {
                return false;
            }

            // Deselect previous selection
            if (_player.selectedUI != null
                && _player.selectedUI.selectedByPlayers.Contains(_player))
            {
                _player.selectedUI.selectedByPlayers.RemoveAll(p => p == _player);
                UIKEventSystem.instance.ExecuteUIEvent(_player, _player.selectedUI, ExecuteEvents.deselectHandler);
            }

            // Update the player's selected UI
            UIKSelectable oldSelectable = _player.selectedUI;
            _player.selectedUI = _selectable;
            if (!_player.selectedUI.selectedByPlayers.Contains(_player))
            {
                _player.selectedUI.selectedByPlayers.Add(_player);
            }
            UIKEventSystem.instance.ExecuteUIEvent(_player, _player.selectedUI, ExecuteEvents.selectHandler);
            _player.OnSelectedUIChanged(oldSelectable, _player.selectedUI);

            return true;
        }

        /// <summary> Have this function return the result of UIKPlayer.TryDeselectUI(this) </summary>
        public bool TryDeselectUI();

        public static bool TryDeselectUI(UIKPlayer _player)
        {
            if (!_player.selectedUI)
            {
                return true;
            }

            if (!_player.selectedUI.CanPlayerDeselect(_player))
            {
                return false;
            }

            // Deselect previous selection
            if (_player.selectedUI.selectedByPlayers.Contains(_player))
            {
                _player.selectedUI.selectedByPlayers.RemoveAll(p => p == _player);
                UIKEventSystem.instance.ExecuteUIEvent(_player, _player.selectedUI, ExecuteEvents.deselectHandler);
            }

            // Update the player's selected UI
            UIKSelectable oldSelectable = _player.selectedUI;
            _player.selectedUI = null;
            _player.OnSelectedUIChanged(oldSelectable, _player.selectedUI);

            return true;
        }

        /// <summary> Have this function return the result of UIKPlayer.TryNavigateUIByDirection(this, _direction) </summary>
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

        /// <summary> Have this function return the result of UIKPlayer.TryNavigateUIByDirection(this, _direction) </summary>
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

        /// <summary> Have this function return the result of UIKPlayer.TrySubmitUI(this, _selectable) </summary>
        public bool TrySubmitUI(UIKSelectable _selectable);

        public static bool TrySubmitUI(UIKPlayer _player, UIKSelectable _selectable)
        {
            if (!_player.selectedUI)
            {
                return false;
            }

            // If we're using a mouse, don't submit on anything unless we have it selected
            if (_player.GetInputDeviceType().UsesCursor()
                && !_player.selectedUI.selectedByPlayers.Contains(_player))
            {
                return false;
            }

            if (!_player.selectedUI.CanPlayerSubmit(_player))
            {
                return false;
            }

            UIKEventSystem.instance.ExecuteUIEvent(_player, _player.selectedUI, ExecuteEvents.submitHandler);
            return true;
        }
    }
} // UIKit namespace

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIKit
{
    public enum UIKInputDirection
    {
        Up,
        Right,
        Down,
        Left
    }

    public abstract class UIKSelectable : UIKMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected bool allowAsFirstSelection = false; // Not intended to be changed during runtime

        [HideInInspector] public List<UIKPlayer> selectedByPlayers = new();
        public bool hovered { get; private set; } // Hovered is only used for the KeyboardAndMouse InputDeviceType
        public bool selected { get => selectedByPlayers.Count > 0; }

        protected EventTrigger eventTrigger { get; private set; }

        private static List<UIKSelectable> firstSelections = new();
        
        
        protected virtual void OnEnable()
        {
            if (allowAsFirstSelection)
            {
                firstSelections.Add(this);
            }
        }

        protected virtual void OnDisable()
        {
            SetHovered(false);

            if (allowAsFirstSelection
                && firstSelections.Contains(this))
            {
                firstSelections.Remove(this);
            }
        }

        
        public abstract UIKSelectable FindUI(Vector3 _direction);

        public abstract UIKSelectable FindUI(UIKInputDirection _direction);

        public virtual bool CanPlayerSelect(UIKPlayer _player)
        {
            return true;
        }

        public virtual bool CanPlayerDeselect(UIKPlayer _player)
        {
            return true;
        }

        public virtual bool CanPlayerSubmit(UIKPlayer _player)
        {
            return true;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            SetHovered(true);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            SetHovered(false);
        }
        
        protected void SetHovered(bool _hovered)
        {
            if (hovered == _hovered)
            {
                return;
            }
            
            hovered = _hovered;
            
            if (hovered)
            {
                // Since we can't tell which player sent the PointerEnter event, we have to just set all players selectUI to this as long as they have a mouse device
                if (UIKPlayerManager.instance != null)
                {
                    foreach (UIKPlayer _player in UIKPlayerManager.instance.players)
                    {
                        if (_player.inputDeviceType.UsesCursor())
                        {
                            _player.TrySelectUI(this);
                        }
                    }
                }
            }
        }

        protected UIKInputDirection VectorToInputDirection(Vector2 _direction)
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

        public static UIKSelectable GetPlayerFirstSelection(UIKPlayer _player)
        {
            foreach (UIKSelectable firstUISelection in firstSelections)
            {
                if (firstUISelection.CanPlayerSelect(_player))
                {
                    return firstUISelection;
                }
            }

            return null;
        }
    }
} // UIKit namespace

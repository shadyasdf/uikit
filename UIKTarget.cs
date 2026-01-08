using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UIKit
{
    public enum UIKInputDirection
    {
        Up,
        Right,
        Down,
        Left
    }

    public abstract class UIKTarget : UIKMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] public UnityEvent<UIKPlayer> OnTargeted = new();
        [SerializeField] public UnityEvent<UIKPlayer> OnUntargeted = new();
        
        [HideInInspector] public List<UIKPlayer> targetedByPlayers = new();
        public bool hovered { get; private set; } // Hovered is only used for the KeyboardAndMouse InputDeviceType

        
        public abstract UIKTarget FindUI(Vector3 _direction);

        public abstract UIKTarget FindUI(UIKInputDirection _direction);

        public virtual bool CanPlayerInteract(UIKPlayer _player)
        {
            if (GetOwningPlayer() is UIKPlayer owningPlayer // If we have a valid owning player
                && owningPlayer != _player) // and it's not this player
            {
                return false; // Then don't let them interact
            }

            return true;
        }
        
        public virtual bool CanPlayerTarget(UIKPlayer _player)
        {
            if (!CanPlayerInteract(_player))
            {
                return false;
            }
            
            return true;
        }

        public virtual bool CanPlayerUntarget(UIKPlayer _player)
        {
            if (!CanPlayerInteract(_player))
            {
                return false;
            }
            
            return true;
        }

        public virtual bool CanPlayerSubmit(UIKPlayer _player)
        {
            if (!CanPlayerInteract(_player))
            {
                return false;
            }
            
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
                            _player.SelectUI(this);
                        }
                    }
                }
            }
        }

        public void HandleTargeted(UIKPlayer _player)
        {
            OnTargeted?.Invoke(_player);
        }

        public void HandleUntargeted(UIKPlayer _player)
        {
            OnUntargeted?.Invoke(_player);
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
    }
} // UIKit namespace

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UIKit
{
    public enum UIKInputDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public abstract class UIKTarget : UIKElement, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] public UnityEvent<UIKPlayer> OnTargeted = new();
        [SerializeField] public UnityEvent<UIKPlayer> OnUntargeted = new();
        
        [HideInInspector] public List<UIKPlayer> targetedByPlayers = new();
        public bool hovered { get; private set; } // Hovered is only used for the KeyboardAndMouse InputDeviceType
        public bool targeted { get; private set; }
        public bool interactable { get; private set; } = true;
        

        public override UIKTarget GetInnerTarget(UIKInputDirection _direction)
        {
            return this;
        }
        
        public virtual bool CanPlayerInteract(UIKPlayer _player)
        {
            if (!interactable)
            {
                return false;
            }
            
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
            if (!interactable)
            {
                return;
            }
            
            SetHovered(true);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }
            
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
            targeted = true;
            
            OnTargeted?.Invoke(_player);
        }

        public void HandleUntargeted(UIKPlayer _player)
        {
            targeted = false;
            
            OnUntargeted?.Invoke(_player);
        }

        public void SetInteractable(bool _interactable)
        {
            if (interactable == _interactable)
            {
                return;
            }
            
            interactable = _interactable;
            
            OnInteractableChanged();
        }

        protected virtual void OnInteractableChanged()
        {
        }
    }
} // UIKit namespace

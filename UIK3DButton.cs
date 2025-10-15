using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIKit
{
    public class UIK3DButton : UIK3DTarget, UIKButton
    {
        [SerializeField] public UnityEvent<UIKEventData> OnClicked = new();

        [SerializeField] protected UIKInputAction clickInputAction;
        
        
        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);
            
            if (GetComponentInParent<UIKScreen>() is UIKScreen screen)
            {
                screen.RegisterButton(this);
            }
        }

        protected override void OnPreDestroy()
        {
            base.OnPreDestroy();
            
            if (GetComponentInParent<UIKScreen>() is UIKScreen screen)
            {
                screen.UnregisterButton(this);
            }
        }
        
        public virtual void OnSubmit(BaseEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                HandleClick(eventData);
            }
        }

        public virtual void OnSelect(BaseEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                HandleSelected(eventData);
            }
        }

        public virtual void OnDeselect(BaseEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                HandleDeselected(eventData);
            }
        }

        public void HandleClick(UIKEventData _eventData)
        {
            OnClicked?.Invoke(_eventData);
        }

        public void HandleSelected(UIKEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                eventData.pressingPlayer?.TryTargetUI(this);
            }
        }

        public void HandleDeselected(UIKEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                eventData.pressingPlayer?.TryUntargetUI(this);
            }
        }

        public UIKInputAction GetClickAction()
        {
            return clickInputAction;
        }

        public UIKTarget GetSelectable()
        {
            return this;
        }
    }
} // UIKit namespace

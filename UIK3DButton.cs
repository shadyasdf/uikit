using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIKit
{
    public class UIK3DButton : UIK3DSelectable, UIKButton
    {
        [SerializeField] protected UnityEvent<UIKEventData> OnClicked = new();
        [SerializeField] protected UnityEvent<UIKEventData> OnSelected = new();
        [SerializeField] protected UnityEvent<UIKEventData> OnDeselected = new();

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
                OnClicked?.Invoke(eventData);
            }
        }

        public virtual void OnSelect(BaseEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                OnSelected?.Invoke(eventData);
            }
        }

        public virtual void OnDeselect(BaseEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                OnDeselected?.Invoke(eventData);
            }
        }

        public UnityEvent<UIKEventData> GetOnClickedEvent()
        {
            return OnClicked;
        }

        public UnityEvent<UIKEventData> GetOnSelectedEvent()
        {
            return OnSelected;
        }

        public UnityEvent<UIKEventData> GetOnDeselectedEvent()
        {
            return OnDeselected;
        }

        public UIKInputAction GetClickAction()
        {
            return clickInputAction;
        }

        public UIKSelectable GetSelectable()
        {
            return this;
        }
    }
} // UIKit namespace

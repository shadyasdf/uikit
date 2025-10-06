using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIKit
{
    public class UIK2DButton : UIK2DSelectable, UIKButton
    {
        [SerializeField] protected UnityEvent<UIKEventData> OnClicked = new();
        [SerializeField] protected UnityEvent<UIKEventData> OnSelected = new();
        [SerializeField] protected UnityEvent<UIKEventData> OnDeselected = new();

        [SerializeField] protected UIKInputAction clickInputAction;
        
        [SerializeField] protected UIKActionDisplay actionDisplay;
        

        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);
            
            // Clear all OnClick listeners, we do not want to use that event when we're using this UI system
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }

            if (GetComponentInParent<UIKScreen>() is UIKScreen screen)
            {
                screen.RegisterButton(this);
            }

            // If we have an action display, update its visuals
            if (actionDisplay
                && clickInputAction.IsValid())
            {
                actionDisplay.SetInputAction(clickInputAction);
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
            OnSelected?.Invoke(_eventData);
        }

        public void HandleDeselected(UIKEventData _eventData)
        {
            OnDeselected?.Invoke(_eventData);
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

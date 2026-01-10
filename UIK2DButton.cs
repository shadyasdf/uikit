using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(UIK2DButtonStyle))]
    public class UIK2DButton : UIK2DTarget, UIKButton
    {
        [SerializeField] public UnityEvent<UIKEventData> OnClicked = new();

        [SerializeField] protected UIKActionObjectReference clickActionObject;
        [SerializeField] protected UIKInputAction clickInputAction;
        

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
            if (clickActionObject != null
                && clickActionObject.GetActionObject(GetOwningPlayer()) is UIKActionObject actionObject)
            {
                actionObject.TryExecute();
            }
            else
            {
                OnClicked?.Invoke(_eventData);
            }
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

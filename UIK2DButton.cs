using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIKit
{
    public class UIK2DButton : UIK2DSelectable, ISubmitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] public UnityEvent<UIKEventData> OnClicked = new();
        [SerializeField] public UnityEvent<UIKEventData> OnSelected = new();
        [SerializeField]  public UnityEvent<UIKEventData> OnDeselected = new();
        

        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);
            
            // Clear all OnClick listeners, we do not want to use that event when we're using this UI system
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
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
    }
} // UIKit namespace

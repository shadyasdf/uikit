using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIKit
{
    public class UIK3DButton : UIK3DSelectable, ISubmitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] public UnityEvent<UIKEventData> OnClicked = new();
        [SerializeField] public UnityEvent<UIKEventData> OnSelected = new();
        [SerializeField]  public UnityEvent<UIKEventData> OnDeselected = new();
        
        
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

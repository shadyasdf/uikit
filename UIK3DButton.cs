using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIKit
{
    public class UIK3DButton : UIK3DSelectable, ISubmitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] public UnityEvent<UIKEventData> OnClick = new();
        [SerializeField] public UnityEvent<UIKEventData> OnSelected = new();
        [SerializeField]  public UnityEvent<UIKEventData> OnDeselected = new();
        
        
        public void OnSubmit(BaseEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                OnClick?.Invoke(eventData);
            }
        }

        public void OnSelect(BaseEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                OnSelected?.Invoke(eventData);
            }
        }

        public void OnDeselect(BaseEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                OnDeselected?.Invoke(eventData);
            }
        }
    }
} // UIKit namespace

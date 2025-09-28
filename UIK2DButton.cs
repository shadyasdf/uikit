using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIKit
{
    public class UIK2DButton : UIK2DSelectable, ISubmitHandler
    {
        [SerializeField] public UnityEvent<UIKEventData> OnPressed = new();
        

        public void OnSubmit(BaseEventData _eventData)
        {
            if (_eventData is UIKEventData eventData)
            {
                OnPressed?.Invoke(eventData);
            }
        }
    }
} // UIKit namespace

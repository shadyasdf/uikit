using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIKit
{
    public interface UIKButton : ISubmitHandler, ISelectHandler, IDeselectHandler
    {
        public UnityEvent<UIKEventData> GetOnClickedEvent();
        public UnityEvent<UIKEventData> GetOnSelectedEvent();
        public UnityEvent<UIKEventData> GetOnDeselectedEvent();
        
        public UIKInputAction GetClickAction();
        
        public UIKSelectable GetSelectable();
    }
} // UIKit namespace

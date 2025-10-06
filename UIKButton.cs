using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIKit
{
    public interface UIKButton : ISubmitHandler, ISelectHandler, IDeselectHandler
    {
        public void HandleClick(UIKEventData _eventData);
        public void HandleSelected(UIKEventData _eventData);
        public void HandleDeselected(UIKEventData _eventData);
        
        public UIKInputAction GetClickAction();
        
        public UIKSelectable GetSelectable();
    }
} // UIKit namespace

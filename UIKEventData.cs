using UnityEngine.EventSystems;

namespace UIKit
{
    public class UIKEventData : BaseEventData
    {
        public UIKPlayer pressingPlayer;
        public UIKSelectable pressedUISelectable;
        public string specialInputKey;


        public UIKEventData(UIKPlayer _pressingPlayer, UIKSelectable _pressedUISelectable, EventSystem _eventSystem, string _specialInputKey = null) : base(_eventSystem)
        {
            pressingPlayer = _pressingPlayer;
            pressedUISelectable = _pressedUISelectable;
            specialInputKey = _specialInputKey;
        }
    }
} // UIKit namespace

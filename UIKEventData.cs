using UnityEngine.EventSystems;

namespace UIKit
{
    public class UIKEventData : BaseEventData
    {
        public UIKPlayer pressingPlayer;
        public UIKTarget pressedUITarget;
        public string specialInputKey;


        public UIKEventData(UIKPlayer _pressingPlayer, UIKTarget _pressedUITarget, EventSystem _eventSystem, string _specialInputKey = null) : base(_eventSystem)
        {
            pressingPlayer = _pressingPlayer;
            pressedUITarget = _pressedUITarget;
            specialInputKey = _specialInputKey;
        }
    }
} // UIKit namespace

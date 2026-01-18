using UnityEngine.EventSystems;

namespace UIKit
{
    public class UIKEventData : BaseEventData
    {
        public UIKPlayer pressingPlayer;
        public UIKTarget pressedUITarget;


        public UIKEventData(UIKPlayer _pressingPlayer, UIKTarget _pressedUITarget, EventSystem _eventSystem) : base(_eventSystem)
        {
            pressingPlayer = _pressingPlayer;
            pressedUITarget = _pressedUITarget;
        }
    }
} // UIKit namespace

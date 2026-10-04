using UnityEngine;

namespace UIKit
{
    public class UIKWidget_Panel : UIKWidget
    {
        [SerializeField] public UIKElement content;


        public override UIKTarget GetInnerTarget(UIKInputDirection _direction)
        {
            return content?.GetInnerTarget(_direction);
        }
    }
} // UIKit namespace

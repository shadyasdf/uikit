using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class UIKElement_2DLayoutGroup_Horizontal : UIKElement_2DLayoutGroup_Linear
    {
        protected override LayoutGroup GetLayoutGroup()
        {
            return GetComponent<HorizontalLayoutGroup>();
        }

        protected override UIKInputDirection GetForwardDirection()
        {
            return UIKInputDirection.Right;
        }

        protected override UIKInputDirection GetBackwardDirection()
        {
            return UIKInputDirection.Left;
        }
    }
} // UIKit namespace

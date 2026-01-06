using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class UIK2DTargetGroup_Vertical : UIK2DTargetGroup
    {
        protected override LayoutGroup GetLayoutGroup()
        {
            return GetComponent<VerticalLayoutGroup>();
        }

        public override UIKTarget GetInsideTargetFromDirection(UIKInputDirection _direction)
        {
            if (!GetLayoutGroup())
            {
                Debug.LogError($"Failed to get target from input direction because there was no valid {nameof(LayoutGroup)}");
                return null;
            }
            
            if (GetLayoutGroup().transform is Transform layoutGroupTransform
                && layoutGroupTransform.childCount > 0)
            {
                switch (_direction)
                {
                    case UIKInputDirection.Down:
                    case UIKInputDirection.Left:
                    case UIKInputDirection.Right:
                        if (layoutGroupTransform.GetChild(0).GetComponent<UIKTarget>() is UIKTarget topTarget)
                        {
                            return topTarget;
                        }
                        break;
                    case UIKInputDirection.Up:
                        if (layoutGroupTransform.GetChild(layoutGroupTransform.childCount - 1).GetComponent<UIKTarget>() is UIKTarget bottomTarget)
                        {
                            return bottomTarget;
                        }
                        break;
                }
            }
            
            return null;
        }
    }
} // UIKit namespace

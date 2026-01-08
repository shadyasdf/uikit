using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    public interface UIK2DTargetFromSelectable
    {
        public UIKTarget FindTargetFromSelectable(Selectable _selectable, UIKInputDirection _direction)
        {
            if (_selectable == null)
            {
                return null;
            }

            Selectable foundSelectable = null;
            switch (_direction)
            {
                case UIKInputDirection.Left:
                    foundSelectable = _selectable.FindSelectableOnLeft();
                    break;
                case UIKInputDirection.Right:
                    foundSelectable = _selectable.FindSelectableOnRight();
                    break;
                case UIKInputDirection.Up:
                    foundSelectable = _selectable.FindSelectableOnUp();
                    break;
                case UIKInputDirection.Down:
                    foundSelectable = _selectable.FindSelectableOnDown();
                    break;
            }

            if (foundSelectable != null)
            {
                if (foundSelectable.GetComponent<UIKTargetGroup>() is UIKTargetGroup targetGroup)
                {
                    return targetGroup.GetInsideTargetFromDirection(_direction);
                }

                if (foundSelectable.GetComponent<UIKTarget>() is UIKTarget target)
                {
                    return target;
                }
            }
            else
            {
                // If we got this far (no found selectable in direction), and we're the direct child of a target group
                if (_selectable.transform.parent?.GetComponent<UIKTargetGroup>() is UIKTargetGroup parentTargetGroup)
                {
                    // Then we rely on the target group to determine what we select that is outside the group
                    return parentTargetGroup.GetOutsideTargetFromDirection(_direction);
                }
            }

            return null;
        }
    }
} // UIKit namespace

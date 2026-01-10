using System.Collections.Generic;
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

        protected override void RefreshNavigation()
        {
            List<UIK2DTarget> targets = new();
            foreach (Transform child in GetLayoutGroup().transform)
            {
                if (child == null
                    || child.IsPendingDestroy()
                    || child.GetComponent<UIK2DTarget>() is not UIK2DTarget target)
                {
                    continue;
                }
                
                targets.Add(target);
            }

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                {
                    continue;
                }

                Navigation navigation = targets[i].selectable.navigation;
                
                if (targets.Count - 1 >= i + 1
                    && targets[i + 1] is UIK2DTarget nextTarget)
                {
                    navigation.selectOnDown = nextTarget.selectable;
                }

                if (i - 1 >= 0
                    && targets[i - 1] is UIK2DTarget previousTarget)
                {
                    navigation.selectOnUp = previousTarget.selectable;
                }
                
                targets[i].selectable.navigation = navigation;
            }
        }
    }
} // UIKit namespace

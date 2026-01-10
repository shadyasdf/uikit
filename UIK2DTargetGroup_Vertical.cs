using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class UIK2DTargetGroup_Vertical : UIK2DTargetGroup
    {
        [SerializeField] protected bool navigationWraps;
        
        
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

            if (targets.Count > 1) // Only handle internal navigation if we have more than 1 target
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (targets[i] == null)
                    {
                        continue;
                    }

                    Navigation navigation = targets[i].selectable.navigation;

                    // Determine next selection (downwards)
                    switch (navigationWraps)
                    {
                        case true:
                            if (i == targets.Count - 1)
                            {
                                if (targets[0] is UIK2DTarget nextTargetWrapped)
                                {
                                    navigation.selectOnDown = nextTargetWrapped.selectable;
                                }
                                else
                                {
                                    navigation.selectOnDown = null;
                                }
                            }
                            else
                            {
                                // If we're not on the last selectable, ignore wrapping behaviour
                                goto case false;
                            }
                            break;
                        case false:
                            if (targets.Count - 1 >= i + 1
                                && targets[i + 1] is UIK2DTarget nextTarget)
                            {
                                navigation.selectOnDown = nextTarget.selectable;
                            }
                            else
                            {
                                navigation.selectOnDown = null;
                            }
                            break;
                    }

                    // Determine previous selection (upwards)
                    switch (navigationWraps)
                    {
                        case true:
                            if (i == 0)
                            {
                                if (targets[^1] is UIK2DTarget previousTargetWrapped)
                                {
                                    navigation.selectOnUp = previousTargetWrapped.selectable;
                                }
                                else
                                {
                                    navigation.selectOnUp = null;
                                }
                            }
                            else
                            {
                                // If we're not on the first selectable, ignore wrapping behaviour
                                goto case false;
                            }
                            break;
                        case false:
                            if (i - 1 >= 0
                                && targets[i - 1] is UIK2DTarget previousTarget)
                            {
                                navigation.selectOnUp = previousTarget.selectable;
                            }
                            else
                            {
                                navigation.selectOnUp = null;
                            }
                            break;
                    }
                    
                    targets[i].selectable.navigation = navigation;
                }
            }
            else
            {
                // Clear out the up and down navigation for our targets
                foreach (UIK2DTarget target in targets)
                {
                    Navigation navigation = target.selectable.navigation;
                    navigation.selectOnDown = null;
                    navigation.selectOnUp = null;
                    target.selectable.navigation = navigation;
                }
            }
        }
    }
} // UIKit namespace

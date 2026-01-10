using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class UIKElement_2DLayoutGroup_Vertical : UIKElement_2DLayoutGroup
    {
        [SerializeField] protected bool navigationWraps;
        
        
        protected override LayoutGroup GetLayoutGroup()
        {
            return GetComponent<VerticalLayoutGroup>();
        }

        public override UIKTarget GetInnerTarget(UIKInputDirection _direction)
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
                        if (layoutGroupTransform.GetChild(0).GetComponent<UIKElement>() is UIKElement topElement
                            && topElement.GetInnerTarget(_direction) is UIKTarget topTarget)
                        {
                            return topTarget;
                        }
                        break;
                    case UIKInputDirection.Up:
                        if (layoutGroupTransform.GetChild(layoutGroupTransform.childCount - 1).GetComponent<UIKElement>() is UIKElement bottomElement
                            && bottomElement.GetInnerTarget(_direction) is UIKTarget bottomTarget)
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
            List<UIKElement> elements = new();
            foreach (Transform child in GetLayoutGroup().transform)
            {
                if (child == null
                    || child.IsPendingDestroy()
                    || child.GetComponent<UIKElement>() is not UIKElement element)
                {
                    continue;
                }
                
                elements.Add(element);
            }

            if (elements.Count > 1) // Only handle internal navigation if we have more than 1 element
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i] == null)
                    {
                        continue;
                    }

                    // Determine next selection (downwards)
                    switch (navigationWraps)
                    {
                        case true:
                            if (i == elements.Count - 1)
                            {
                                if (elements[0] is UIKElement nextElementWrapped)
                                {
                                    elements[i].navigation.down = nextElementWrapped;
                                }
                                else
                                {
                                    elements[i].navigation.down = null;
                                }
                            }
                            else
                            {
                                // If we're not on the last selectable, ignore wrapping behaviour
                                goto case false;
                            }
                            break;
                        case false:
                            if (elements.Count - 1 >= i + 1
                                && elements[i + 1] is UIKElement nextElement)
                            {
                                elements[i].navigation.down = nextElement;
                            }
                            else
                            {
                                elements[i].navigation.down = null;
                            }
                            break;
                    }

                    // Determine previous selection (upwards)
                    switch (navigationWraps)
                    {
                        case true:
                            if (i == 0)
                            {
                                if (elements[^1] is UIKElement previousElementWrapped)
                                {
                                    elements[i].navigation.up = previousElementWrapped;
                                }
                                else
                                {
                                    elements[i].navigation.up = null;
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
                                && elements[i - 1] is UIKElement previousElement)
                            {
                                elements[i].navigation.up = previousElement;
                            }
                            else
                            {
                                elements[i].navigation.up = null;
                            }
                            break;
                    }
                }
            }
            else
            {
                // Clear out the up and down navigation for our elements
                foreach (UIKElement element in elements)
                {
                    element.navigation.down = null;
                    element.navigation.up = null;
                }
            }
        }
    }
} // UIKit namespace

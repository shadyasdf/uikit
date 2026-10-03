using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UIKit
{
    public abstract class UIKElement_2DLayoutGroup_Linear : UIKElement_2DLayoutGroup
    {
        [SerializeField] protected bool navigationWraps;


        protected abstract UIKInputDirection GetForwardDirection();

        protected abstract UIKInputDirection GetBackwardDirection();

        public override UIKTarget GetInnerTarget(UIKInputDirection _direction)
        {
            if (!GetLayoutGroup())
            {
                Debug.LogError($"Failed to get target from input direction because there was no valid {nameof(UnityEngine.UI.LayoutGroup)}");
                return null;
            }

            // Entering the group while moving backward lands on its last element; any other direction lands on its first
            IEnumerable<UIKElement> childElements = GetLayoutGroup().transform.GetComponentsInChildren<UIKElement>();
            if (_direction == GetBackwardDirection())
            {
                childElements = childElements.Reverse();
            }

            foreach (UIKElement childElement in childElements)
            {
                if (childElement == this
                    || childElement.gameObject.IsPendingDestroy())
                {
                    continue;
                }

                if (childElement.GetInnerTarget(_direction) is UIKTarget target
                    && target.interactable)
                {
                    return target;
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
                    || child.gameObject.IsPendingDestroy()
                    || child.GetComponent<UIKElement>() is not UIKElement element)
                {
                    continue;
                }

                elements.Add(element);
            }

            bool wraps = navigationWraps
                && elements.Count > 1;
            for (int i = 0; i < elements.Count; i++)
            {
                UIKElement next = i + 1 < elements.Count ? elements[i + 1] : wraps ? elements[0] : null;
                UIKElement previous = i > 0 ? elements[i - 1] : wraps ? elements[^1] : null;

                elements[i].navigation.Set(GetForwardDirection(), next);
                elements[i].navigation.Set(GetBackwardDirection(), previous);
            }
        }
    }
} // UIKit namespace

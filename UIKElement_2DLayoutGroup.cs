using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    public abstract class UIKElement_2DLayoutGroup : UIKElement
    {
        protected abstract LayoutGroup GetLayoutGroup();
        
        public virtual void AddTarget(UIKElement _element)
        {
            if (_element == null
                || !GetLayoutGroup())
            {
                return;
            }
            
            _element.transform.SetParent(GetLayoutGroup().transform, false);
            
            RefreshNavigation();
        }

        public virtual void RemoveTarget(UIKElement _element)
        {
            if (_element == null)
            {
                return;
            }
            
            _element.gameObject.SafeDestroy();

            RefreshNavigation();
        }

        public virtual List<UIKElement> GetTargets()
        {
            List<UIKElement> targets = new();
            foreach (Transform child in GetLayoutGroup().transform)
            {
                if (child.GetComponent<UIKElement>() is UIKElement element)
                {
                    targets.Add(element);
                }
            }

            return targets;
        }

        public virtual void ClearTargets()
        {
            foreach (UIKElement target in GetTargets())
            {
                RemoveTarget(target);
            }
        }

        protected abstract void RefreshNavigation();
    }
} // UIKit namespace

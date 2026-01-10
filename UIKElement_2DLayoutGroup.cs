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

        protected abstract void RefreshNavigation();
    }
} // UIKit namespace

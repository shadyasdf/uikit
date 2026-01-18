using UnityEngine;

namespace UIKit
{
    public interface UIKActionTrigger
    {
        public UIKActionObject GetTriggerActionObject();
        public UIKInputAction GetTriggerInputAction();
    }
}

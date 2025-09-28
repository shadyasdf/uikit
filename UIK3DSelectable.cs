using System.Collections.Generic;
using UnityEngine;

namespace UIKit
{
    public class UIK3DSelectable : UIKSelectable
    {
        public Dictionary<UIKInputDirection, UIKSelectable> explicitNavigateSelectables = new(); // Eventually we should find a better solution for 3D selectables
        
        
        public override UIKSelectable FindUI(Vector3 _direction)
        {
            return FindUI(VectorToInputDirection(_direction));
        }

        public override UIKSelectable FindUI(UIKInputDirection _direction)
        {
            if (explicitNavigateSelectables.ContainsKey(_direction)
                && explicitNavigateSelectables[_direction] != null)
            {
                return explicitNavigateSelectables[_direction];
            }

            return null;
        }
    }
} // UIKit namespace

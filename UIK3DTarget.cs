using System.Collections.Generic;
using UnityEngine;

namespace UIKit
{
    public class UIK3DTarget : UIKTarget
    {
        public Dictionary<UIKInputDirection, UIKTarget> explicitNavigateSelectables = new(); // Eventually we should find a better solution for 3D selectables
        
        
        public override UIKTarget FindUI(Vector3 _direction)
        {
            return FindUI(((Vector2)_direction).GetInputDirection());
        }

        public override UIKTarget FindUI(UIKInputDirection _direction)
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

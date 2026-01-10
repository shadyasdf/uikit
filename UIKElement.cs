using System;
using UnityEngine;

namespace UIKit
{
    [Serializable]
    public class UIKNavigation
    {
        public UIKElement up;
        public UIKElement down;
        public UIKElement right;
        public UIKElement left;
    }
    
    public abstract class UIKElement : UIKMonoBehaviour
    {
        [SerializeField] public UIKNavigation navigation;


        public virtual UIKTarget GetOuterTarget(UIKInputDirection _direction)
        {
            switch (_direction)
            {
                case UIKInputDirection.Up:
                    if (navigation?.up?.GetInnerTarget(_direction) is UIKTarget upTarget)
                    {
                        return upTarget;
                    }

                    break;
                case UIKInputDirection.Down:
                    if (navigation?.down?.GetInnerTarget(_direction) is UIKTarget downTarget)
                    {
                        return downTarget;
                    }

                    break;
                case UIKInputDirection.Left:
                    if (navigation?.left?.GetInnerTarget(_direction) is UIKTarget leftTarget)
                    {
                        return leftTarget;
                    }

                    break;
                case UIKInputDirection.Right:
                    if (navigation?.right?.GetInnerTarget(_direction) is UIKTarget rightTarget)
                    {
                        return rightTarget;
                    }

                    break;
            }
            
            // If we made it this far, this element didn't have navigation set up for that direction
            // We should check for a container element we're inside and use their outer target
            if (transform.parent?.GetComponentInParent<UIKElement>() is UIKElement parentElement) // .parent because GetComponentInParent includes self 
            {
                return parentElement.GetOuterTarget(_direction);
            }

            return null;
        }
        
        public abstract UIKTarget GetInnerTarget(UIKInputDirection _direction);
    }
} // UIKit namespace


namespace UIKit
{
    public abstract class UIKTargetGroup : UIKMonoBehaviour, UIKTargetFinder
    {
        public abstract UIKTarget GetInsideTargetFromDirection(UIKInputDirection _direction);
        public abstract UIKTarget GetOutsideTargetFromDirection(UIKInputDirection _direction);
        public abstract void AddTarget(UIKTarget _target);
    }
}

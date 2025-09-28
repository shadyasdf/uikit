using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(Selectable))]
    public class UIK2DSelectable : UIKSelectable
    {
        public Selectable selectable { get; private set; }


        protected override void Awake()
        {
            base.Awake();
            
            selectable = GetComponent<Selectable>();

            // Set the navigation mode to always be explicit
            Navigation navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            selectable.navigation = navigation;
        }
        

        public override UIKSelectable FindUI(Vector3 _direction)
        {
            if (selectable == null)
            {
                return null;
            }

            return FindUI(VectorToInputDirection(_direction));
        }

        public override UIKSelectable FindUI(UIKInputDirection _direction)
        {
            if (selectable == null)
            {
                return null;
            }

            Selectable foundSelectable = null;
            switch (_direction)
            {
                case UIKInputDirection.Left:
                    foundSelectable = selectable.FindSelectableOnLeft();
                    break;
                case UIKInputDirection.Right:
                    foundSelectable = selectable.FindSelectableOnRight();
                    break;
                case UIKInputDirection.Up:
                    foundSelectable = selectable.FindSelectableOnUp();
                    break;
                case UIKInputDirection.Down:
                    foundSelectable = selectable.FindSelectableOnDown();
                    break;
            }

            if (foundSelectable != null)
            {
                UIKSelectable ui = foundSelectable.GetComponent<UIKSelectable>();
                if (ui != null)
                {
                    return ui;
                }
            }

            return null;
        }

        public void LinkNavigationTo(UIKInputDirection _direction, UIK2DSelectable _selectable)
        {
            Navigation navigation = selectable.navigation;
            switch (_direction)
            {
                case UIKInputDirection.Up:
                    navigation.selectOnUp = _selectable.selectable;
                    break;
                case UIKInputDirection.Right:
                    navigation.selectOnRight = _selectable.selectable;
                    break;
                case UIKInputDirection.Down:
                    navigation.selectOnDown = _selectable.selectable;
                    break;
                case UIKInputDirection.Left:
                    navigation.selectOnLeft = _selectable.selectable;
                    break;
            }
            selectable.navigation = navigation;
        }
    }
} // UIKit namespace

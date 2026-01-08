using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(Selectable))]
    public abstract class UIK2DTargetGroup : UIKTargetGroup, UIK2DTargetFromSelectable
    {
        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            if (GetComponent<Selectable>() is Selectable selectable)
            {
                selectable.interactable = false;
                selectable.transition = Selectable.Transition.None;
                Navigation navigation = selectable.navigation;
                navigation.mode = Navigation.Mode.Explicit;
                selectable.navigation = navigation;
            }
        }


        protected abstract LayoutGroup GetLayoutGroup(); 
        
        public override UIKTarget GetOutsideTargetFromDirection(UIKInputDirection _direction)
        {
            if (GetComponent<Selectable>() is Selectable selectable)
            {
                return ((UIK2DTargetFromSelectable)this).FindTargetFromSelectable(selectable, _direction);
            }
            
            return null;
        }

        public override void AddTarget(UIKTarget _target)
        {
            if (_target == null
                || !GetLayoutGroup())
            {
                return;
            }
            
            _target.transform.SetParent(GetLayoutGroup().transform, false);
        }
    }
} // UIKit namespace

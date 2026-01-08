using UnityEngine;
using UnityEngine.EventSystems;

namespace UIKit
{
    public abstract class UIKTargetGroup : UIKTarget
    {
        public abstract UIKTarget GetInsideTargetFromDirection(UIKInputDirection _direction);
        public abstract UIKTarget GetOutsideTargetFromDirection(UIKInputDirection _direction);
        public abstract void AddTarget(UIKTarget _target);
        
        
        public override UIKTarget FindUI(Vector3 _direction)
        {
            throw new System.NotImplementedException();
        }

        public override UIKTarget FindUI(UIKInputDirection _direction)
        {
            throw new System.NotImplementedException();
        }
        
        public override bool CanPlayerInteract(UIKPlayer _player)
        {
            return false;
        }
        
        public override bool CanPlayerTarget(UIKPlayer _player)
        {
            return false;
        }

        public override bool CanPlayerUntarget(UIKPlayer _player)
        {
            return false;
        }

        public override bool CanPlayerSubmit(UIKPlayer _player)
        {
            return false;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}

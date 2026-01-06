using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIKit
{
    public class UIKActionDisplay : UIKMonoBehaviour
    {
        protected UIKInputAction inputAction;


        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            UpdateDisplay();
        }


        public virtual void SetInputAction(UIKInputAction _inputAction)
        {
            inputAction = _inputAction;
            
            UpdateDisplay();
        }

        protected virtual void UpdateDisplay()
        {
            if (!inputAction.IsValid())
            {
                UpdateDisplayWithInvalid();
                return;
            }

            if (GetOwningPlayer() is UIKPlayer player
                && player.playerInput is PlayerInput playerInput
                && playerInput.currentActionMap.actions.FirstOrDefault(a => inputAction == a) is InputAction playerInputAction)
            {
                UpdateDisplayWithText(playerInputAction.GetBindingDisplayString());
                return;
            }
            
            UpdateDisplayWithInvalid();
        }

        protected virtual void UpdateDisplayWithInvalid()
        {
        }

        protected virtual void UpdateDisplayWithText(string _text)
        {
        }
    }
} // UIKit namespace

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UIKit
{
    public class UIK2DInputIcon : UIKMonoBehaviour
    {
        [SerializeField] protected Image image;


        public void SetIcon(Sprite _icon)
        {
            if (image == null)
            {
                return;
            }

            image.sprite = _icon;
            image.enabled = image.sprite;
        }

        public void SetInputAction(InputAction _inputAction)
        {
            SetIcon(_inputAction != null ? GetCanvas()?.GetInputActionIcon(_inputAction) : null);
        }

        public void SetInputBinding(string _bindingPath)
        {
            SetIcon(GetCanvas()?.GetInputBindingIcon(_bindingPath));
        }
    }
} // UIKit namespace

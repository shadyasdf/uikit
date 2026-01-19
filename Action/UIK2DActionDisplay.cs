using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UIKit
{
    public class UIK2DActionDisplay : UIKMonoBehaviour
    {
        [SerializeField] protected TMP_Text text;
        [SerializeField] protected Image image;
        
        [Space(10)]
        [Header("Editor Preview Only")]
        [SerializeField] protected string editorPreviewText;
        [SerializeField] protected Sprite editorPreviewIcon;

        private InputAction displayInputAction;
        private string displayActionText;
        

        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);
            
            if (_isOnValidate)
            {
                if (text != null)
                {
                    text.SetText(editorPreviewText);
                }

                if (image != null)
                {
                    image.sprite = editorPreviewIcon;
                }
            }
        }


        public void SetInputAction(UIKInputAction _inputAction, bool _refresh = true)
        {
            if (GetOwningPlayer()?.playerInput is PlayerInput playerInput
                && playerInput.currentActionMap.actions.FirstOrDefault(a => _inputAction == a) is InputAction inputAction)
            {
                displayInputAction = inputAction;
            }
            else
            {
                displayInputAction = null;
            }

            if (_refresh)
            {
                RefreshDisplay();
            }
        }

        public void SetActionText(string _text, bool _refresh = true)
        {
            displayActionText = _text;

            if (_refresh)
            {
                RefreshDisplay();
            }
        }
        
        public virtual void RefreshDisplay()
        {
            // Update text
            text.SetText(displayActionText);
            
            // Update icon
            if (image != null
                && displayInputAction != null)
            {
                image.sprite = GetCanvas().GetInputActionIcon(displayInputAction);
            }
            else
            {
                image.sprite = null;
            }
        }
    }
} // UIKit namespace

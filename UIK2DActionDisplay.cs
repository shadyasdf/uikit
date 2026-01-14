using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UIKit
{
    public class UIK2DActionDisplay : UIKMonoBehaviour, UIKActionTrigger
    {
        [SerializeField] protected TMP_Text text;
        [SerializeField] protected Image image;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected UIKActionObjectReference actionObject;
        [SerializeField] protected UIKInputAction inputAction;
        
        [Space(10)]
        [Header("Editor Preview Only")]
        [SerializeField] protected string editorPreviewText;
        [SerializeField] protected Sprite editorPreviewIcon;
        

        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            if (!_isOnValidate)
            {
                if (GetComponentInParent<UIKScreen>() is UIKScreen screen)
                {
                    screen.RegisterActionTrigger(this);
                }
                
                if (GetOwningPlayer() is UIKPlayer player)
                {
                    player.OnInputActionMapChanged.AddListener(Player_OnInputActionMapChanged);
                    player.OnInputDeviceTypeChanged.AddListener(Player_OnInputDeviceTypeChanged);

                    if (actionObject != null
                        && actionObject.GetActionObject(player) is UIKActionObject actionObjectInstance)
                    {
                        actionObjectInstance.OnCanExecuteChanged.AddListener(ActionObject_OnCanExecuteChanged);
                        ActionObject_OnCanExecuteChanged(actionObjectInstance.CanExecute());
                    }
                }
            }
            
            if (_isOnValidate)
            {
                text.SetText(editorPreviewText);
                image.sprite = editorPreviewIcon;
            }
            else
            {
                UpdateDisplay();
            }
        }

        protected override void OnPreDestroy()
        {
            base.OnPreDestroy();
            
            if (GetComponentInParent<UIKScreen>() is UIKScreen screen)
            {
                screen.UnregisterActionTrigger(this);
            }
        }
        
        
        public UIKActionObject GetTriggerActionObject()
        {
            if (actionObject != null
                && GetOwningPlayer() is UIKPlayer player)
            {
                return actionObject.GetActionObject(player);
            }

            return null;
        }

        public UIKInputAction GetTriggerInputAction()
        {
            return inputAction;
        }
        
        protected virtual void UpdateDisplay()
        {
            if (inputAction == null
                || !inputAction.IsValid())
            {
                UpdateDisplayInputAction(null);
            
                return;
            }

            if (GetOwningPlayer() is UIKPlayer player
                && player.playerInput is PlayerInput playerInput
                && playerInput.currentActionMap.actions.FirstOrDefault(a => inputAction == a) is InputAction nativeInputAction)
            {
                UpdateDisplayInputAction(nativeInputAction);
            
                return;
            }

            UpdateDisplayInputAction(null);
        }

        protected virtual void UpdateDisplayInputAction(InputAction _inputAction)
        {
            if (text == null
                || image == null)
            {
                return;
            }
        
            if (GetOwningPlayer() is UIKPlayer player
                && player.canvas is UIKCanvas canvas)
            {
                text.SetText(actionObject.GetActionText(player));
                image.sprite = canvas.GetInputActionIcon(_inputAction);
            }
            else
            {
                text.SetText(string.Empty);
                image.sprite = null;
            }
        }

        protected virtual void UpdateCanInteract(bool _canExecute)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = _canExecute ? 1f : 0.5f;
            }
        }

        private void Player_OnInputActionMapChanged(string _actionMap)
        {
            UpdateDisplay();
        }

        private void Player_OnInputDeviceTypeChanged(UIKInputDevice _inputDeviceType)
        {
            UpdateDisplay();
        }

        private void ActionObject_OnCanExecuteChanged(bool _canExecute)
        {
            UpdateCanInteract(_canExecute);
        }
    }
}

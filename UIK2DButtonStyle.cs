using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [Serializable]
    public enum UIKStyleTransition
    {
        None,
        Color
    }
    
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(UIK2DButton))]
    public class UIK2DButtonStyle : UIKMonoBehaviour
    {
        [SerializeField] protected UIKStyleTransition transition = UIKStyleTransition.None;
        [SerializeField] protected Graphic transitionGraphic;
        [SerializeField] protected Color normalColor;
        [SerializeField] protected Color targetedColor;

        protected Button button;
        protected UIK2DButton uik2DButton;
        
        
        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            if (!button)
            {
                button = GetComponent<Button>();
            }

            if (!uik2DButton)
            {
                uik2DButton = GetComponent<UIK2DButton>();
            }

            if (_isOnValidate)
            {
                if (button.transition != Selectable.Transition.None)
                {
                    button.transition = Selectable.Transition.None;
                }
            }

            if (!_isOnValidate)
            {
                uik2DButton.OnTargeted.AddListener(OnTargeted);
                uik2DButton.OnUntargeted.AddListener(OnUntargeted);
            }
        }


        protected virtual void OnTargeted(UIKPlayer _player)
        {
            if (transitionGraphic)
            {
                transitionGraphic.color = targetedColor;
            }
        }
        
        protected virtual void OnUntargeted(UIKPlayer _player)
        {
            if (transitionGraphic)
            {
                transitionGraphic.color = normalColor;
            }
        }
    }
} // UIKit namespace

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
    public class UIK2DButtonStyle : UIK2DButtonPart
    {
        [SerializeField] protected UIKStyleTransition transition = UIKStyleTransition.None;
        [SerializeField] protected Graphic transitionGraphic;
        [SerializeField] protected Color normalColor;
        [SerializeField] protected Color targetedColor;
        [SerializeField] protected bool useCheckedColor;
        [SerializeField] protected Color checkedColor;

        protected Button button;
        protected UIK2DButtonCheckbox checkbox;
        
        
        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            if (!button)
            {
                button = GetComponent<Button>();
            }

            if (!checkbox)
            {
                checkbox = GetComponent<UIK2DButtonCheckbox>();
            }

            if (_isOnValidate)
            {
                if (button.transition != Selectable.Transition.None)
                {
                    button.transition = Selectable.Transition.None;
                }
            }

            UpdateTransitionGraphic();
        }


        protected override void Button_OnTargeted(UIKPlayer _player)
        {
            base.Button_OnTargeted(_player);

            UpdateTransitionGraphic();
        }
        
        protected override void Button_OnUntargeted(UIKPlayer _player)
        {
            base.Button_OnUntargeted(_player);

            UpdateTransitionGraphic();
        }

        public void SetNormalColor(Color _color)
        {
            normalColor = _color;
            UpdateTransitionGraphic();
        }
        
        public void SetTargetedColor(Color _color)
        {
            targetedColor = _color;
            UpdateTransitionGraphic();
        }

        public void SetCheckedColor(Color _color)
        {
            checkedColor = _color;
            UpdateTransitionGraphic();
        }

        public void UpdateTransitionGraphic()
        {
            if (transitionGraphic
                && uik2DButton)
            {
                if (uik2DButton.targeted)
                {
                    transitionGraphic.color = targetedColor;
                }
                else if (useCheckedColor
                    && checkbox
                    && checkbox.isChecked)
                {
                    transitionGraphic.color = checkedColor;
                }
                else
                {
                    transitionGraphic.color = normalColor;
                }
            }
        }

        public void SetGraphicVisible(bool _visible)
        {
            transitionGraphic.enabled = _visible;
        }
    }
} // UIKit namespace

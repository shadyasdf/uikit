using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIKit
{
    public class UIK2DButtonCheckbox : UIK2DButtonPart
    {
        [SerializeField] public UnityEvent<bool> OnCheckedChanged = new();

        [SerializeField] protected Graphic checkmark;
        [SerializeField] protected bool checkedByDefault;
        [SerializeField] protected bool toggleOnClick = true;

        public bool isChecked { get; private set; }


        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            if (!_isOnValidate
                || !Application.isPlaying)
            {
                isChecked = checkedByDefault;
            }
            RefreshCheckmark();
        }

        protected override void Button_OnClickHandled(UIKEventData _eventData)
        {
            base.Button_OnClickHandled(_eventData);

            if (toggleOnClick)
            {
                SetChecked(!isChecked);
            }
        }


        public virtual void SetChecked(bool _checked, bool _notify = true)
        {
            if (isChecked == _checked)
            {
                return;
            }

            isChecked = _checked;
            RefreshCheckmark();

            if (_notify)
            {
                OnCheckedChanged.Invoke(isChecked);
            }
        }

        protected virtual void RefreshCheckmark()
        {
            if (checkmark)
            {
                checkmark.enabled = isChecked;
            }

            GetComponent<UIK2DButtonStyle>()?.UpdateTransitionGraphic();
        }
    }
} // UIKit namespace

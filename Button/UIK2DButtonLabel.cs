using TMPro;
using UnityEngine;

namespace UIKit
{
    public class UIK2DButtonLabel : UIK2DButtonPart
    {
        [SerializeField] protected TMP_Text text;
        [SerializeField] protected string labelText = "Button";


        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            if (TMP_Settings.instance == null)
            {
                return;
            }

            RefreshText();
        }


        public virtual void SetText(string _text)
        {
            labelText = _text;
            RefreshText();
        }

        public string GetText()
        {
            return labelText;
        }

        protected virtual void RefreshText()
        {
            if (!text)
            {
                return;
            }

            if (GetOwningPlayer() is UIKPlayer player
                && uik2DButton
                && uik2DButton.GetClickActionObject()?.GetActionText(player) is string actionText
                && !string.IsNullOrEmpty(actionText))
            {
                text.SetText(actionText);
            }
            else
            {
                text.SetText(labelText);
            }
        }
    }
} // UIKit namespace

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIKit
{
    public class UIK2DButtonPicker : UIK2DButtonPart
    {
        [SerializeField] public UnityEvent<int> OnSelectedIndexChanged = new();

        [SerializeField] protected TMP_Text valueText;
        [SerializeField] protected UIK2DButton previousButton;
        [SerializeField] protected UIK2DButton nextButton;
        [SerializeField] protected bool wraps = true;
        [SerializeField] protected List<string> options = new();

        public int selectedIndex { get; private set; }


        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            if (!_isOnValidate
                || !Application.isPlaying)
            {
                selectedIndex = 0;
            }

            if (!_isOnValidate)
            {
                previousButton?.OnClickHandled.AddListener(PreviousButton_OnClickHandled);
                nextButton?.OnClickHandled.AddListener(NextButton_OnClickHandled);
            }

            if (TMP_Settings.instance == null)
            {
                return;
            }

            RefreshVisuals();
        }

        protected override void OnPreDestroy()
        {
            base.OnPreDestroy();

            previousButton?.OnClickHandled.RemoveListener(PreviousButton_OnClickHandled);
            nextButton?.OnClickHandled.RemoveListener(NextButton_OnClickHandled);
        }

        protected override void Button_OnClickHandled(UIKEventData _eventData)
        {
            base.Button_OnClickHandled(_eventData);

            SelectRelative(1);
        }


        private void PreviousButton_OnClickHandled(UIKEventData _eventData)
        {
            SelectRelative(-1);
        }

        private void NextButton_OnClickHandled(UIKEventData _eventData)
        {
            SelectRelative(1);
        }

        protected override void Button_OnLockedChanged(bool _locked)
        {
            base.Button_OnLockedChanged(_locked);

            previousButton?.SetLocked(_locked);
            nextButton?.SetLocked(_locked);
        }

        public override bool HandleButtonNavigation(UIKPlayer _player, UIKInputDirection _direction)
        {
            switch (_direction)
            {
                case UIKInputDirection.Left:
                    if (!uik2DButton.locked)
                    {
                        SelectRelative(-1);
                    }
                    return true;
                case UIKInputDirection.Right:
                    if (!uik2DButton.locked)
                    {
                        SelectRelative(1);
                    }
                    return true;
            }

            return false;
        }

        public virtual void SetOptions(List<string> _options)
        {
            options = new List<string>(_options);
            selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(options.Count - 1, 0));
            RefreshVisuals();
        }

        public IReadOnlyList<string> GetOptions()
        {
            return options;
        }

        public virtual void SetSelectedIndex(int _index, bool _notify = true)
        {
            if (options.Count == 0)
            {
                return;
            }

            _index = Mathf.Clamp(_index, 0, options.Count - 1);
            if (selectedIndex == _index)
            {
                return;
            }

            selectedIndex = _index;
            RefreshVisuals();

            if (_notify)
            {
                OnSelectedIndexChanged.Invoke(selectedIndex);
            }
        }

        public void SelectRelative(int _offset)
        {
            if (options.Count == 0)
            {
                return;
            }

            int index = selectedIndex + _offset;
            if (wraps)
            {
                index = ((index % options.Count) + options.Count) % options.Count;
            }

            SetSelectedIndex(index);
        }

        protected virtual void RefreshVisuals()
        {
            if (valueText)
            {
                valueText.SetText(selectedIndex < options.Count ? options[selectedIndex] : string.Empty);
            }

            SetArrowAvailable(previousButton, wraps || selectedIndex > 0);
            SetArrowAvailable(nextButton, wraps || selectedIndex < options.Count - 1);
        }

        protected void SetArrowAvailable(UIK2DButton _arrow, bool _available)
        {
            if (!_arrow)
            {
                return;
            }

            _arrow.SetInteractable(_available);
            foreach (Graphic graphic in _arrow.GetComponentsInChildren<Graphic>(true))
            {
                graphic.enabled = _available;
            }
        }
    }
} // UIKit namespace

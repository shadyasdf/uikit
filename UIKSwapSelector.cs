using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIKit
{
    public class UIKSwapSelector : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<int, int> OnSwapped = new();
        [HideInInspector] public UnityEvent<int> OnHeldIndexChanged = new();

        private readonly List<UIK2DButton> buttons = new();

        public int heldIndex { get; private set; } = -1;
        public bool isHolding => heldIndex >= 0;


        protected virtual void OnDestroy()
        {
            Clear();
        }


        public void Add(UIK2DButton _button)
        {
            if (!_button
                || buttons.Contains(_button))
            {
                return;
            }

            buttons.Add(_button);
            _button.OnClickHandled.AddListener(Button_OnClickHandled);
        }

        public void Clear()
        {
            CancelHold();

            foreach (UIK2DButton button in buttons)
            {
                if (button)
                {
                    button.OnClickHandled.RemoveListener(Button_OnClickHandled);
                }
            }
            buttons.Clear();
        }

        public bool CancelHold()
        {
            if (!isHolding)
            {
                return false;
            }

            SetHeldIndex(-1);
            return true;
        }

        private void Button_OnClickHandled(UIKEventData _eventData)
        {
            int index = buttons.IndexOf(_eventData.pressedUITarget as UIK2DButton);
            if (index < 0)
            {
                return;
            }

            if (!isHolding)
            {
                SetHeldIndex(index);
                UIKFeedback.Raise(UIKFeedbackKind.SwapPickUp, buttons[index]);
                return;
            }

            int swappedIndex = heldIndex;
            SetHeldIndex(-1);
            UIKFeedback.Raise(UIKFeedbackKind.SwapDrop, buttons[index]);
            if (swappedIndex != index)
            {
                OnSwapped.Invoke(swappedIndex, index);
            }
        }

        private void SetHeldIndex(int _index)
        {
            if (heldIndex == _index)
            {
                return;
            }

            heldIndex = _index;
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i]?.GetComponent<UIK2DButtonCheckbox>()?.SetChecked(i == heldIndex, false);
            }

            OnHeldIndexChanged.Invoke(heldIndex);
        }
    }
} // UIKit namespace

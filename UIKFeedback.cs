using UnityEngine;
using UnityEngine.Events;

namespace UIKit
{
    public enum UIKFeedbackKind
    {
        Focus,
        Unfocus,
        Submit,
        Denied,
        Back,
        NavigationBump,
        TabChanged,
        CheckboxOn,
        CheckboxOff,
        PickerStep,
        SliderStep,
        SliderRelease,
        SwapPickUp,
        SwapDrop,
        HoldStart,
        HoldCancel,
        HoldComplete
    }

    public static class UIKFeedback
    {
        public static readonly UnityEvent<UIKFeedbackKind, UIKElement> OnFeedback = new();


        public static void Raise(UIKFeedbackKind _kind, UIKElement _source)
        {
            OnFeedback.Invoke(_kind, _source);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            OnFeedback.RemoveAllListeners();
        }
    }
} // UIKit namespace

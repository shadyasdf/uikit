using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIKit
{
    public class UIKWidgetSwitcher : UIKElement
    {
        [HideInInspector] public UnityEvent<UIKWidget> OnCurrentWidgetChanged = new();

        [SerializeField] private int currentWidgetIndex;
        [SerializeField] protected bool manageActivation = true;


        protected override void Awake()
        {
            base.Awake();

            Refresh();
        }


        public void SetCurrentWidget(int _index)
        {
            if (_index < 0
                || _index >= GetWidgets().Count)
            {
                Debug.LogError("Index is out of range in switcher");

                return;
            }

            if (currentWidgetIndex == _index)
            {
                return;
            }

            currentWidgetIndex = _index;

            Refresh();
        }

        public void SetCurrentWidget(UIKWidget _widget)
        {
            int index = GetWidgets().IndexOf(_widget);
            if (index < 0)
            {
                Debug.LogError("Widget is not a child of this switcher");

                return;
            }

            SetCurrentWidget(index);
        }

        public int GetCurrentWidgetIndex()
        {
            return currentWidgetIndex;
        }

        public UIKWidget GetCurrentWidget()
        {
            List<UIKWidget> widgets = GetWidgets();
            return currentWidgetIndex >= 0 && currentWidgetIndex < widgets.Count ? widgets[currentWidgetIndex] : null;
        }

        public override UIKTarget GetInnerTarget(UIKInputDirection _direction)
        {
            return GetCurrentWidget()?.GetInnerTarget(_direction);
        }

        public virtual void Refresh()
        {
            List<UIKWidget> widgets = GetWidgets();

            // Deactivate the inactive ones first
            for (int i = 0; i < widgets.Count; i++)
            {
                if (i != currentWidgetIndex)
                {
                    widgets[i].gameObject.SetActive(false);
                    if (manageActivation)
                    {
                        widgets[i].Deactivate();
                    }
                }
            }

            if (currentWidgetIndex >= 0
                && currentWidgetIndex < widgets.Count)
            {
                widgets[currentWidgetIndex].gameObject.SetActive(true);
                if (manageActivation)
                {
                    widgets[currentWidgetIndex].Activate();
                }

                OnCurrentWidgetChanged?.Invoke(widgets[currentWidgetIndex]);
            }
            else
            {
                OnCurrentWidgetChanged?.Invoke(null);
            }
        }

        protected List<UIKWidget> GetWidgets()
        {
            List<UIKWidget> widgets = new();
            foreach (Transform child in transform)
            {
                if (!child.gameObject.IsPendingDestroy()
                    && child.GetComponent<UIKWidget>() is UIKWidget widget)
                {
                    widgets.Add(widget);
                }
            }

            return widgets;
        }
    }
} // UIKit namespace

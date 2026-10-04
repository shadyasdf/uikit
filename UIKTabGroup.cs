using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UIKit
{
    [Serializable]
    public struct UIKTab
    {
        public UIK2DButton button;
        public UIKWidget page;
    }

    public class UIKTabGroup : UIKElement, UIKInputActionHandler
    {
        [HideInInspector] public UnityEvent<int> OnCurrentTabChanged = new();

        [SerializeField] protected UIKElement_2DLayoutGroup tabButtonGroup;
        [SerializeField] protected UIKWidgetSwitcher pageSwitcher;
        [SerializeField] protected UIKInputDirection pagesDirectionFromTabs = UIKInputDirection.Down;
        [SerializeField] protected UIKInputAction nextTabInputAction;
        [SerializeField] protected UIKInputAction previousTabInputAction;
        [SerializeField] protected bool tabSwitchingWraps = true;
        [SerializeField] protected List<UIKTab> tabs = new();

        public int currentTabIndex { get; private set; }


        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            if (_isOnValidate)
            {
                return;
            }

            if (tabButtonGroup
                && pageSwitcher)
            {
                tabButtonGroup.navigation.Set(pagesDirectionFromTabs, pageSwitcher);
            }

            foreach (UIKTab tab in tabs)
            {
                BindTab(tab);
            }
            RefreshTabButtons();
        }


        public void AddTab(UIK2DButton _button, UIKWidget _page)
        {
            if (!_button
                || !_page
                || !tabButtonGroup
                || !pageSwitcher)
            {
                return;
            }

            tabButtonGroup.AddTarget(_button);
            _page.transform.SetParent(pageSwitcher.transform, false);

            UIKTab tab = new() { button = _button, page = _page };
            tabs.Add(tab);
            BindTab(tab);

            pageSwitcher.Refresh();
            if (tabs.Count == 1)
            {
                currentTabIndex = 0;
                pageSwitcher.SetCurrentWidget(_page);
            }
            RefreshTabButtons();
        }

        public int GetNumTabs()
        {
            return tabs.Count;
        }

        public void SelectTab(int _index)
        {
            if (_index < 0
                || _index >= tabs.Count)
            {
                return;
            }

            if (_index == currentTabIndex)
            {
                RefreshTabButtons();
                return;
            }

            // Captured before the switch, because hiding the old page untargets anything inside it
            UIKPlayer player = GetOwningPlayer();
            UIKTarget previousTarget = player?.targetUI;
            bool targetWasOnTabButton = previousTarget
                && tabs.Exists(t => t.button == previousTarget);
            bool targetWasOnPage = previousTarget
                && tabs[currentTabIndex].page
                && previousTarget.transform.IsChildOf(tabs[currentTabIndex].page.transform);

            currentTabIndex = _index;
            pageSwitcher.SetCurrentWidget(tabs[_index].page);
            RefreshTabButtons();

            if (player != null
                && !player.inputDeviceType.UsesCursor())
            {
                if (targetWasOnTabButton)
                {
                    player.TryTargetUI(tabs[_index].button);
                }
                else if (targetWasOnPage
                    && tabs[_index].page.GetInnerTarget(pagesDirectionFromTabs) is UIKTarget pageTarget)
                {
                    player.TryTargetUI(pageTarget);
                }
            }

            OnCurrentTabChanged.Invoke(currentTabIndex);
        }

        public void SelectTabRelative(int _offset)
        {
            if (tabs.Count == 0)
            {
                return;
            }

            int index = currentTabIndex + _offset;
            if (tabSwitchingWraps)
            {
                index = ((index % tabs.Count) + tabs.Count) % tabs.Count;
            }

            SelectTab(Mathf.Clamp(index, 0, tabs.Count - 1));
        }

        public bool HandleInputAction(InputAction.CallbackContext _context)
        {
            if (!_context.action.WasPressedThisFrame()
                || !_context.action.triggered)
            {
                return false;
            }

            if (nextTabInputAction == _context.action)
            {
                SelectTabRelative(1);
                return true;
            }

            if (previousTabInputAction == _context.action)
            {
                SelectTabRelative(-1);
                return true;
            }

            return false;
        }

        public override UIKTarget GetInnerTarget(UIKInputDirection _direction)
        {
            return pageSwitcher?.GetInnerTarget(_direction) ?? tabButtonGroup?.GetInnerTarget(_direction);
        }

        protected virtual void BindTab(UIKTab _tab)
        {
            _tab.button?.OnClickHandled.AddListener(_ => SelectTab(tabs.FindIndex(t => t.button == _tab.button)));
        }

        protected virtual void RefreshTabButtons()
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].button?.GetComponent<UIK2DButtonCheckbox>()?.SetChecked(i == currentTabIndex, false);
            }

            if (pageSwitcher
                && currentTabIndex < tabs.Count)
            {
                pageSwitcher.navigation.Set(GetOppositeDirection(pagesDirectionFromTabs), tabs[currentTabIndex].button);
            }
        }

        private static UIKInputDirection GetOppositeDirection(UIKInputDirection _direction)
        {
            switch (_direction)
            {
                case UIKInputDirection.Up:
                    return UIKInputDirection.Down;
                case UIKInputDirection.Down:
                    return UIKInputDirection.Up;
                case UIKInputDirection.Left:
                    return UIKInputDirection.Right;
                default:
                    return UIKInputDirection.Left;
            }
        }
    }
} // UIKit namespace

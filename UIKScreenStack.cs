using UnityEngine;

namespace UIKit
{
    public class UIKScreenStack : UIKWidgetStack
    {
        protected override void UpdateWidgetsInStack()
        {
            base.UpdateWidgetsInStack();

            if (widgetsInStack.Count > 0)
            {
                UIKScreenLowerScreenVisibility visibility = UIKScreenLowerScreenVisibility.ShowAll;
                if (widgetsInStack[^1] is UIKScreen topScreen)
                {
                    // Top screen should always be visible
                    topScreen.GetCanvasGroup().alpha = 1f;

                    visibility = topScreen.lowerScreenVisibility;
                }
                
                for (int i = 0; i < widgetsInStack.Count - 1; i++)
                {
                    if (widgetsInStack[i] is UIKScreen screen
                        && screen.GetCanvasGroup() is CanvasGroup canvasGroup)
                    {
                        switch (visibility)
                        {
                            case UIKScreenLowerScreenVisibility.ShowAll:
                                canvasGroup.alpha = 1f;
                                break;
                            case UIKScreenLowerScreenVisibility.HideAll:
                                canvasGroup.alpha = 0f;
                                break;
                        }
                    }
                }
            }
        }

        protected override UIKWidget TryInstantiateWidget(GameObject _widgetPrefab, Transform _parentTransform)
        {
            UIKWidget widget = base.TryInstantiateWidget(_widgetPrefab, _parentTransform);
            if (widget is not UIKScreen)
            {
                Debug.LogError($"Can't instantiate a widget into {nameof(UIKScreenStack)} that isn't of type {nameof(UIKScreen)}");
                return null;
            }

            return widget;
        }
    }
} // UIKit namespace

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UIKit
{
    public class UIKWidgetStack : MonoBehaviour
    {
        [HideInInspector] public UnityEvent OnStackChanged = new();
        
        protected List<UIKWidget> widgetsInStack = new();
        

        public virtual void PushToStack(string _widgetName, GameObject _widgetPrefab)
        {
            if (TryInstantiateWidget(_widgetPrefab, transform) is UIKWidget widget)
            {
                widget.Setup(_widgetName, this);
            }
            
            UpdateWidgetsInStack();
        }
        
        public virtual void PopFromStack(UIKWidget _widget)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<UIKWidget>() is UIKWidget widget
                    && widget == _widget)
                {
                    widget.gameObject.SafeDestroy();
                    break;
                }
            }
            
            UpdateWidgetsInStack();
        }

        public virtual void PopStack()
        {
            if (transform.childCount > 0)
            {
                if (transform.GetChild(0).GetComponent<UIKWidget>() is UIKWidget widget)
                {
                    widget.gameObject.SafeDestroy();
                }
            }
            
            UpdateWidgetsInStack();
        }

        public virtual void ClearStack()
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<UIKWidget>() is UIKWidget widget)
                {
                    widget.gameObject.SafeDestroy();
                }
            }
            
            UpdateWidgetsInStack();
        }

        public List<UIKWidget> GetWidgets()
        {
            return widgetsInStack;
        }

        public int GetNumWidgets()
        {
            return widgetsInStack.Count;
        }

        public List<UIKWidget> GetWidgetsOrdered()
        {
            List<UIKWidget> reversedWidgets = widgetsInStack;
            reversedWidgets.Reverse();
            return reversedWidgets;
        }
        
        public UIKWidget GetWidgetByName(string _widgetName)
        {
            return widgetsInStack.FirstOrDefault(w => w.widgetName == _widgetName);
        }

        protected virtual void UpdateWidgetsInStack()
        {
            // Re-determine the widgets in the stack by checking our children
            widgetsInStack.Clear();
            foreach (Transform child in transform)
            {
                if (!child
                    || !child.gameObject
                    || child.gameObject.IsPendingDestroy())
                {
                    continue;
                }

                if (child.GetComponent<UIKWidget>() is UIKWidget widget)
                {
                    widgetsInStack.Add(widget);
                }
            }

            // Deactivate all widgets in the stack and activate the topmost one
            for (int i = 0; i < widgetsInStack.Count; i++)
            {
                widgetsInStack[i].SetActive(i == widgetsInStack.Count - 1);
            }
            
            OnStackChanged.Invoke();
        }

        protected virtual UIKWidget TryInstantiateWidget(GameObject _widgetPrefab, Transform _parentTransform)
        {
            if (_widgetPrefab == null)
            {
                Debug.LogError("Failed to instantiate widget prefab because it was null.");
                return null;
            }
            
            if (Instantiate(_widgetPrefab, _parentTransform) is GameObject widgetGO)
            {
                return widgetGO.GetComponent<UIKWidget>();
            }

            Debug.LogError($"Failed to instantiate widget prefab `{_widgetPrefab.name}` because it could not be found in resources.");
            return null;
        }
    }
} // UIKit namespace

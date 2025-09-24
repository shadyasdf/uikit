using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UIKit
{
    public class UIKWidgetStack : MonoBehaviour
    {
        [SerializeField] private Transform stackTransform;

        private List<UIKWidget> widgetsInStack = new();
        

        public void PushToStack(string _widgetName, GameObject _widgetPrefab)
        {
            if (TryInstantiateWidget(_widgetPrefab, stackTransform) is UIKWidget widget)
            {
                widget.Setup(_widgetName, this);
            }
            
            UpdateWidgetsInStack();
        }
        
        public void PopFromStack(UIKWidget _widget)
        {
            foreach (Transform child in stackTransform)
            {
                if (child.GetComponent<UIKWidget>() is UIKWidget widget
                    && widget == _widget)
                {
                    Destroy(widget.gameObject);
                    break;
                }
            }
            
            UpdateWidgetsInStack();
        }

        public void PopStack()
        {
            if (stackTransform.childCount > 0)
            {
                if (stackTransform.GetChild(0).GetComponent<UIKWidget>() is UIKWidget widget)
                {
                    Destroy(widget.gameObject);
                }
            }
            
            UpdateWidgetsInStack();
        }

        public void ClearStack()
        {
            foreach (Transform child in stackTransform)
            {
                if (child.GetComponent<UIKWidget>() is UIKWidget widget)
                {
                    Destroy(widget.gameObject);
                }
            }
            
            UpdateWidgetsInStack();
        }

        public UIKWidget GetWidgetByName(string _widgetName)
        {
            return widgetsInStack.FirstOrDefault(w => w.widgetName == _widgetName);
        }

        private void UpdateWidgetsInStack()
        {
            // Re-determine the widgets in the stack by checking our children
            widgetsInStack.Clear();
            foreach (Transform child in stackTransform)
            {
                if (child == null)
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
        }

        private UIKWidget TryInstantiateWidget(GameObject _widgetPrefab, Transform _parentTransform)
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

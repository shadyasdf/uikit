using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UIKit
{
    [RequireComponent(typeof(Canvas))]
    public class UIKCanvas : MonoBehaviour
    {
        private Transform screenStackPanelTransform;
        private Dictionary<int, UIKWidgetStack> screenStackByLayer = new();
        
        public static UIKCanvas instance { get; private set; }
        
        
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        
            DontDestroyOnLoad(gameObject);
        }
        
        
        public void PushScreen(string _name)
        {
            if (UIKScreen.GetScreenAttribute(_name) is UIKScreenAttribute screenAttribute
                && GetScreenStack(screenAttribute.layer) is UIKWidgetStack screenStack
                && UIKScreen.GetScreenPrefab(_name) is GameObject screenPrefab)
            {
                screenStack.PushToStack(screenAttribute.name, screenPrefab);
            }
        }

        public void PopScreen(string _name)
        {
            if (UIKScreen.GetScreenAttribute(_name) is UIKScreenAttribute screenAttribute
                && GetScreenStack(screenAttribute.layer) is UIKWidgetStack screenStack
                && screenStack.GetWidgetByName(_name) is UIKWidget screenWidget)
            {
                screenStack.PopFromStack(screenWidget);
            }
        }

        protected UIKWidgetStack GetScreenStack(int _layer)
        {
            if (screenStackPanelTransform == null)
            {
                GameObject screenStackPanelGO = new("ScreenStackPanel", typeof(RectTransform));
                screenStackPanelGO.transform.SetParent(transform);
                
                RectTransform screenStackPanelRectTransform = screenStackPanelGO.GetComponent<RectTransform>();
                screenStackPanelRectTransform.anchorMin = Vector2.zero;
                screenStackPanelRectTransform.anchorMax = Vector2.one;
                screenStackPanelRectTransform.offsetMin = Vector2.zero;
                screenStackPanelRectTransform.offsetMax = Vector2.zero;
                screenStackPanelRectTransform.sizeDelta = Vector2.zero;
                
                screenStackPanelTransform = screenStackPanelGO.transform;
            }
            
            if (!screenStackByLayer.ContainsKey(_layer))
            {
                GameObject screenStackGO = new(_layer.ToString(), typeof(RectTransform), typeof(UIKWidgetStack));
                screenStackGO.transform.SetParent(screenStackPanelTransform);
                
                RectTransform screenStackRectTransform = screenStackGO.GetComponent<RectTransform>();
                screenStackRectTransform.anchorMin = Vector2.zero;
                screenStackRectTransform.anchorMax = Vector2.one;
                screenStackRectTransform.offsetMin = Vector2.zero;
                screenStackRectTransform.offsetMax = Vector2.zero;
                screenStackRectTransform.sizeDelta = Vector2.zero;

                screenStackByLayer.Add(_layer, screenStackGO.GetComponent<UIKWidgetStack>());
                
                UpdateScreenStacks();
            }
            
            return screenStackByLayer[_layer];
        }

        private void UpdateScreenStacks()
        {
            foreach (KeyValuePair<int, UIKWidgetStack> pair in screenStackByLayer.ToArray().OrderByDescending(p => p.Key))
            {
                pair.Value.transform.SetAsFirstSibling();
            }
        }
    }
} // UIKit namespace

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UIKit
{
    [Serializable]
    public struct UIKScreenInputTypeActionMap
    {
        public UIKScreenInputType screenInputType;
        public UIKActionMap actionMap;
    }
    
    [RequireComponent(typeof(Canvas))]
    public class UIKCanvas : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<UIKScreen> OnTopScreenChanged = new();

        [SerializeField] public List<UIKScreenInputTypeActionMap> screenInputTypeActionMaps = new();
        [SerializeField] protected UIKInputAction leftClickInputAction;
        [SerializeField] protected UIKInputAction uiSubmitInputAction;
        [SerializeField] protected UIKInputAction uiMoveInputAction;
        
        protected Transform screenStackPanelTransform;
        protected Dictionary<int, UIKScreenStack> screenStackByLayer = new();

        public UIKScreen topScreen { get; private set; }
        private List<InputControl> consumedInputControlsThisFrame = new();
        
        
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            // Create the screen stack panel
            GameObject screenStackPanelGO = new("ScreenStackPanel", typeof(RectTransform));
            screenStackPanelGO.transform.SetParent(transform);
            screenStackPanelGO.transform.localScale = Vector3.one;
            screenStackPanelGO.transform.localPosition = Vector3.zero;
            
            RectTransform screenStackPanelRectTransform = screenStackPanelGO.GetComponent<RectTransform>();
            screenStackPanelRectTransform.anchorMin = Vector2.zero;
            screenStackPanelRectTransform.anchorMax = Vector2.one;
            screenStackPanelRectTransform.offsetMin = Vector2.zero;
            screenStackPanelRectTransform.offsetMax = Vector2.zero;
            screenStackPanelRectTransform.sizeDelta = Vector2.zero;
            
            screenStackPanelTransform = screenStackPanelGO.transform;
        }

        protected virtual void Update()
        {
            consumedInputControlsThisFrame.Clear();
        }


        private void ScreenStack_OnStackChanged()
        {
            RefreshTopScreen();
        }
        
        public string GetActionMapForScreenInputType(UIKScreenInputType _inputType)
        {
            return screenInputTypeActionMaps.FirstOrDefault(o => o.screenInputType == _inputType).actionMap.name;
        }
        
        private Coroutine CoCheckTopScreenChanged;
        public virtual void RefreshTopScreen()
        {
            if (CoCheckTopScreenChanged != null)
            {
                StopCoroutine(CoCheckTopScreenChanged);
            }
            CoCheckTopScreenChanged = StartCoroutine(DoCheckTopScreenChanged());
        }

        private IEnumerator DoCheckTopScreenChanged()
        {
            // Wait a frame for deleted screens and child index changes to propagate
            yield return new WaitForEndOfFrame();
            
            UIKScreen lastTopScreen = topScreen;
            UIKScreen newTopScreen = GetTopScreen();
            topScreen = newTopScreen;
            if (lastTopScreen != topScreen)
            {
                OnTopScreenChanged?.Invoke(topScreen);
                
                if (topScreen
                    && GetOwningPlayer() is UIKPlayer player)
                {
                    // Determine if we should lock/unlock the cursor on this screen
                    if (player.inputDeviceType.UsesCursor())
                    {
                        if (topScreen.inputType == UIKScreenInputType.Game)
                        {
                            Cursor.lockState = CursorLockMode.Locked;
                            Cursor.visible = false;
                        }
                        else
                        {
                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true;
                        }
                    }
                    
                    if (GetActionMapForScreenInputType(topScreen.inputType) is string actionMapName
                        && player.playerInput.currentActionMap.name != actionMapName)
                    {
                        // Wait a frame for press input to be finish
                        // Switching our current action map will re-broadcast any inputs from this frame
                        yield return new WaitForEndOfFrame();
                    
                        player.playerInput.SwitchCurrentActionMap(actionMapName);
                    }
                }  
            }
        }

        public virtual Camera GetCamera()
        {
            return GetComponent<Canvas>()?.worldCamera;
        }

        protected virtual UIKScreen GetTopScreen()
        {
            foreach (UIKScreenStack screenStack in GetScreenStacksOrdered())
            {
                foreach (UIKWidget screenWidget in screenStack.GetWidgetsOrdered())
                {
                    if (screenWidget.active
                        && screenWidget is UIKScreen screen)
                    {
                        return screen;
                    }
                }
            }

            return null;
        }
        
        public virtual UIKPlayer GetOwningPlayer()
        {
            if (UIKPlayerManager.instance != null)
            {
                if (UIKPlayerManager.instance.players.FirstOrDefault(p => p.canvas == this) is UIKPlayer player)
                {
                    return player;
                }
            }

            return null;
        }
        
        public void PushScreen(string _name)
        {
            if (UIKScreen.GetScreenAttribute(_name) is UIKScreenAttribute screenAttribute
                && GetScreenStack(screenAttribute.layer) is UIKScreenStack screenStack
                && UIKScreen.GetScreenPrefab(_name) is GameObject screenPrefab)
            {
                screenStack.PushToStack(screenAttribute.name, screenPrefab);
            }
        }

        public void PopScreen(string _name)
        {
            if (UIKScreen.GetScreenAttribute(_name) is UIKScreenAttribute screenAttribute
                && GetScreenStack(screenAttribute.layer) is UIKScreenStack screenStack
                && screenStack.GetWidgetByName(_name) is UIKWidget screenWidget)
            {
                screenStack.PopFromStack(screenWidget);
            }
        }

        public UIKScreenStack GetScreenStack(int _layer)
        {
            if (!screenStackByLayer.ContainsKey(_layer))
            {
                GameObject screenStackGO = new(_layer.ToString(), typeof(RectTransform), typeof(UIKScreenStack));
                screenStackGO.transform.SetParent(screenStackPanelTransform);
                screenStackGO.transform.localScale = Vector3.one;
                screenStackGO.transform.localPosition = Vector3.zero;
                
                RectTransform screenStackRectTransform = screenStackGO.GetComponent<RectTransform>();
                screenStackRectTransform.anchorMin = Vector2.zero;
                screenStackRectTransform.anchorMax = Vector2.one;
                screenStackRectTransform.offsetMin = Vector2.zero;
                screenStackRectTransform.offsetMax = Vector2.zero;
                screenStackRectTransform.sizeDelta = Vector2.zero;

                if (screenStackGO.GetComponent<UIKScreenStack>() is UIKScreenStack screenStack)
                {
                    screenStackByLayer.Add(_layer, screenStack);
                    
                    screenStack.OnStackChanged.AddListener(ScreenStack_OnStackChanged);
                }
                
                UpdateScreenStacks();
            }
            
            return screenStackByLayer[_layer];
        }
        
        private void UpdateScreenStacks()
        {
            foreach (UIKScreenStack screenStack in GetScreenStacksOrdered())
            {
                screenStack.transform.SetAsFirstSibling();
            }
        }

        public virtual bool OnPreInputActionTriggered(UIKPlayer _player, InputAction.CallbackContext _context)
        {
            if (consumedInputControlsThisFrame.Contains(_context.control))
            {
                return false;
            }
            
            if (GetOwningPlayer() is UIKPlayer player)
            {
                // Consume UI inputs before broadcasting them
                if (_context.action == leftClickInputAction
                    || _context.action == uiSubmitInputAction)
                {
                    if (_context.action.WasPressedThisFrame()
                        && _context.action.triggered)
                    {
                        if (player.TrySubmitUI(player.GetTargetUI()))
                        {
                            ConsumeInputControl(_context);
                            return false;
                        }
                    }
                }
                else if (_context.action == uiMoveInputAction)
                {
                    if (_context.action.WasPerformedThisFrame())
                    {
                        if (player.TryNavigateUIByDirection(_context.ReadValue<Vector2>()))
                        {
                            ConsumeInputControl(_context);
                            return false;
                        }
                    }
                }
            }
            
            // If any of the screens consume the input action, then we return false
            foreach (UIKScreenStack screenStack in GetScreenStacksOrdered().ToArray())
            {
                foreach (UIKScreen screen in screenStack.GetWidgetsOrdered().Cast<UIKScreen>().ToArray())
                {
                    if (!screen.OnPreInputActionTriggered(_player, _context))
                    {
                        ConsumeInputControl(_context);
                        return false;
                    }
                }
            }

            return true;
        }

        private void ConsumeInputControl(InputAction.CallbackContext _context)
        {
            // Hack to get around some internal Unity code where they assume the
            // validity of controls inside an InputAction.CallbackContext. It's
            // possibly null in this case because sometimes game code will leave
            // a player due to an input
            try
            {
                if (_context.control == null)
                {
                    return;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
            
            consumedInputControlsThisFrame.Add(_context.control);
        }

        private IEnumerable<UIKScreenStack> GetScreenStacksOrdered()
        {
            return screenStackByLayer.OrderByDescending(p => p.Key).Select(p => p.Value);
        }
    }
} // UIKit namespace

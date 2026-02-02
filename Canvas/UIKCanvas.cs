using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    [Serializable]
    public struct UIKInputDeviceInputIconDatabase
    {
        public UIKInputDevice inputDevice;
        public UIKInputIconDatabase inputIconDatabase;
    }
    
    [RequireComponent(typeof(Canvas))]
    public class UIKCanvas : MonoBehaviour, UIKInputActionHandler
    {
        [HideInInspector] public UnityEvent<UIKScreen> OnTopScreenChanged = new();

        [SerializeField] protected UIKInputAction leftClickInputAction;
        [SerializeField] protected UIKInputAction uiSubmitInputAction;
        [SerializeField] protected UIKInputAction uiMoveInputAction;
        [SerializeField] public List<UIKScreenInputTypeActionMap> screenInputTypeActionMaps = new();
        [SerializeField] public List<UIKInputDeviceInputIconDatabase> inputDeviceInputIconDatabases = new();
        
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
                HandleTopScreenChanged();
                
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
                    
                        player.SetInputActionMap(actionMapName);
                    }
                }  
            }
        }

        protected virtual void HandleTopScreenChanged()
        {
            OnTopScreenChanged?.Invoke(topScreen);
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
        
        public UIKScreen PushScreen(string _name)
        {
            if (UIKScreen.GetScreenPrefab(_name) is GameObject screenPrefab
                && screenPrefab.GetComponent<UIKScreen>() is UIKScreen screen
                && screen.GetType().GetCustomAttribute<UIKScreenAttribute>() is UIKScreenAttribute screenAttribute
                && GetScreenStack(screenAttribute.layer) is UIKScreenStack screenStack)
            {
                screenStack.PushToStack(screenAttribute.name, screenPrefab);
                
                return screen;
            }

            return null;
        }

        public void PopScreen(string _name)
        {
            foreach (UIKScreenStack screenStack in GetScreenStacksOrdered())
            {
                foreach (UIKWidget screenWidget in screenStack.GetWidgetsOrdered())
                {
                    if (screenWidget.widgetName == _name)
                    {
                        screenStack.PopFromStack(screenWidget);
                        
                        return;
                    }
                }
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

        protected void ConsumeInputControl(InputAction.CallbackContext _context)
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
        
        public virtual bool HandleInputAction(InputAction.CallbackContext _context)
        {
            if (consumedInputControlsThisFrame.Contains(_context.control))
            {
                return true;
            }
            
            // Consume UI inputs before broadcasting them
            if ((leftClickInputAction == _context.action || uiSubmitInputAction == _context.action)
                && _context.action.WasPressedThisFrame()
                && _context.action.triggered)
            {
                if (GetOwningPlayer().TrySubmitUI(GetOwningPlayer().GetTargetUI()))
                {
                    ConsumeInputControl(_context);
                    
                    return true;
                }
            }
            else if (uiMoveInputAction == _context.action
                && _context.action.WasPerformedThisFrame())
            {
                if (GetOwningPlayer().TryNavigateUIByDirection(_context.ReadValue<Vector2>()))
                {
                    ConsumeInputControl(_context);
                    
                    return true;
                }
            }
            
            // If any of the screens consume the input action, then we return false
            foreach (UIKScreenStack screenStack in GetScreenStacksOrdered().ToArray())
            {
                foreach (UIKScreen screen in screenStack.GetWidgetsOrdered().Cast<UIKScreen>().ToArray())
                {
                    if (screen.HandleInputAction(_context))
                    {
                        ConsumeInputControl(_context);
                        
                        return true;
                    }
                }
            }

            return false;
        }

        public Sprite GetInputActionIcon(InputAction _inputAction)
        {
            if (GetOwningPlayer() is UIKPlayer player
                && _inputAction != null
                && _inputAction.bindings.Count > 0)
            {
                foreach (InputBinding inputBinding in _inputAction.bindings)
                {
                    string path = inputBinding.effectivePath;
                    string deviceFromPath = path.Split(">")[0].TrimStart('<');
                    UIKInputDevice bindingInputDevice = UIKInputExtensions.GetInputDeviceFromString(deviceFromPath);
                    
                    if (player.inputDeviceType.IsFlagSet(bindingInputDevice))
                    {
                        foreach (UIKInputDeviceInputIconDatabase inputIconDatabase in inputDeviceInputIconDatabases)
                        {
                            if (inputIconDatabase.inputDevice.IsFlagSet(bindingInputDevice))
                            {
                                if (inputIconDatabase.inputIconDatabase.GetIcon(inputBinding.effectivePath) is Sprite sprite)
                                {
                                    return sprite;
                                }
                                
                                break;
                            }
                        }
                        
                        break;
                    }
                }
            }

            return null;
        }

        public string GetInputActionName(InputAction _inputAction)
        {
            if (_inputAction != null)
            {
                string bindingDisplayString = _inputAction.GetBindingDisplayString();
                bindingDisplayString = bindingDisplayString.Split("|")[0];
                bindingDisplayString = bindingDisplayString.TrimEnd();
                return bindingDisplayString;
            }

            return string.Empty;
        }
    }
} // UIKit namespace

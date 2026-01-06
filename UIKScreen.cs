using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIKit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIKScreenAttribute : Attribute
    {
        public string name;
        public int layer;
    }
    
    [Serializable]
    public enum UIKScreenInputType
    {
        None,
        UI,
        Game
    }

    [Serializable]
    public enum UIKScreenLowerScreenVisibility
    {
        ShowAll,
        HideAll
    }
    
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIKScreen : UIKWidget
    {
        [SerializeField] public UIKScreenInputType inputType;
        [SerializeField] public UIKScreenLowerScreenVisibility lowerScreenVisibility;
        [SerializeField] public UIKInputAction backInputAction;
        
        protected Dictionary<UIKInputAction, List<UIKButton>> buttonByClickAction = new();
        
        
        public virtual bool OnPreInputActionTriggered(UIKPlayer _player, InputAction.CallbackContext _context)
        {
            // Only try to consume input actions on the screen if we're active
            if (active
                && GetCanvas().GetActionMapForScreenInputType(inputType) is UIKActionMap actionMap
                && _context.action.actionMap == actionMap) // and if the input action is in our screen's input type action map
            {
                // If this was a button press
                if (_context.action.WasPressedThisFrame()
                    && _context.action.triggered)
                {
                    // If this is a back action, and we'd like to handle it
                    if (backInputAction == _context.action)
                    {
                        HandleBackAction();
                        
                        return false;
                    }
                    
                    // If any of our registered buttons wants to consume this input action, handle it with a click event
                    if (buttonByClickAction.TryGetValue((UIKInputAction)_context.action, out List<UIKButton> buttons))
                    {
                        foreach (UIKButton button in buttons)
                        {
                            if (button is UIKTarget selectable)
                            {
                                if (_player.TryTargetUI(selectable))
                                {
                                    _player.TrySubmitUI(selectable);
                                }
                            }
                        }
                        
                        return false;
                    }
                }
            }
        
            return true;
        }

        public virtual void HandleBackAction()
        {
            CloseScreen();
        }
        
        public virtual void CloseScreen()
        {
            if (GetCanvas())
            {
                if (GetType().GetCustomAttribute<UIKScreenAttribute>() is UIKScreenAttribute attribute)
                {
                    GetCanvas().PopScreen(attribute.name);
                }
            }
        }

        public virtual void RegisterButton(UIKButton _button)
        {
            if (_button?.GetClickAction() is UIKInputAction clickAction
                && clickAction.IsValid())
            {
                if (buttonByClickAction.TryGetValue(clickAction, out List<UIKButton> boundButtons))
                {
                    boundButtons.Add(_button);
                }
                else
                {
                    buttonByClickAction.Add(clickAction, new List<UIKButton>() { _button });
                }
            }
        }

        public virtual void UnregisterButton(UIKButton _button)
        {
            if (_button?.GetClickAction() is UIKInputAction clickAction
                && clickAction.IsValid())
            {
                if (buttonByClickAction.TryGetValue(clickAction, out List<UIKButton> boundButtons))
                {
                    boundButtons.Remove(_button);
                }
            }
        }

        public CanvasGroup GetCanvasGroup()
        {
            return GetComponent<CanvasGroup>();
        }

        protected override void OnActiveChanged()
        {
            base.OnActiveChanged();

            GetCanvas()?.RefreshTopScreen();
        }


        private static Dictionary<string, GameObject> screenPrefabByName = new();

        
        public static GameObject GetScreenPrefab(string _name)
        {
            if (screenPrefabByName.ContainsKey(_name))
            {
                return screenPrefabByName[_name];
            }

            GameObject[] screenPrefabs = Resources.LoadAll<GameObject>("Screens");
            foreach (GameObject screenPrefab in screenPrefabs)
            {
                if (screenPrefab.GetComponent<UIKScreen>() is UIKScreen screen
                    && screen.GetType().GetCustomAttribute(typeof(UIKScreenAttribute)) is UIKScreenAttribute screenAttribute
                    && screenAttribute.name == _name)
                {
                    screenPrefabByName.Add(_name, screenPrefab);
                    return screenPrefabByName[_name];
                }
            }

            return null;
        }
        
        public static UIKScreenAttribute GetScreenAttribute(string _name)
        {
            if (GetScreenPrefab(_name) is GameObject screenPrefab
                && screenPrefab.GetComponent<UIKScreen>() is UIKScreen screen
                && screen.GetType().GetCustomAttribute(typeof(UIKScreenAttribute)) is UIKScreenAttribute screenAttribute)
            {
                return screenAttribute;
            }

            return null;
        }
    }
} // UIKit namespace

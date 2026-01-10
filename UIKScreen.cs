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
    public abstract class UIKScreen : UIKWidget, UIKInputActionHandler
    {
        [SerializeField] public UIKScreenInputType inputType;
        [SerializeField] public UIKScreenLowerScreenVisibility lowerScreenVisibility;
        [SerializeField] public UIKInputAction backInputAction;
        [SerializeField] public UIKTarget firstTarget;
        
        protected Dictionary<UIKInputAction, List<UIKButton>> buttonByClickAction = new();


        protected override void OnActivate()
        {
            base.OnActivate();
            
            // Attempt to make a new first selection on this screen automatically 
            if (GetOwningPlayer() is UIKPlayer player
                && !player.targetUI
                && !player.inputDeviceType.UsesCursor()) // if we don't use MKB
            {
                if (firstTarget
                    && firstTarget.CanPlayerTarget(player))
                {
                    player.SelectUI(firstTarget);
                }
            }
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            if (GetOwningPlayer() is UIKPlayer player)
            {
                foreach (UIKTarget target in GetComponentsInChildren<UIKTarget>())
                {
                    // Remove our current selection on this screen
                    if (player.targetUI == target)
                    {
                        player.DeselectUI();
                        break;
                    }
                }
            }
        }


        /// <returns>True if the back action was handled, false if not</returns>
        public virtual bool HandleBackAction()
        {
            CloseScreen();
            
            return true;
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
        
        public bool HandleInputAction(InputAction.CallbackContext _context)
        {
            // Only try to consume input actions on the screen if we're active
            if (active
                && GetCanvas().GetActionMapForScreenInputType(inputType) is string actionMapName
                && _context.action.actionMap.name == actionMapName) // and if the input action is in our screen's input type action map
            {
                // If child screen logic wants to capture input first, go for it
                if (HandleScreenInputAction(_context))
                {
                    return true;
                }
            }
        
            return false;
        }

        protected virtual bool HandleScreenInputAction(InputAction.CallbackContext _context)
        {
            // If this was a button press
            if (_context.action.WasPressedThisFrame()
                && _context.action.triggered)
            {
                if (HandleScreenButtonPress(_context))
                {
                    return true;
                }
            }
            
            return false;
        }

        protected virtual bool HandleScreenButtonPress(InputAction.CallbackContext _context)
        {
            // If this is a back action, and we'd like to handle it
            if (backInputAction == _context.action)
            {
                if (HandleBackAction())
                {
                    return true;
                }
            }
                    
            // If any of our registered buttons wants to consume this input action, handle it with a click event
            if (buttonByClickAction.TryGetValue((UIKInputAction)_context.action, out List<UIKButton> buttons))
            {
                foreach (UIKButton button in buttons)
                {
                    if (button is UIKTarget selectable
                        && GetOwningPlayer().TryTargetUI(selectable)
                        && GetOwningPlayer().TrySubmitUI(selectable))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
} // UIKit namespace

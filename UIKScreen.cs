using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UIKit
{
    public abstract class UIKScreen : UIKWidget
    {
        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
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
        
        public virtual bool OnPreInputActionTriggered(UIKPlayer _player, InputAction.CallbackContext _context)
        {
            if (ShouldHandleInputActionAsBackAction(_context))
            {
                HandleBackAction();
                
                return false;
            }
        
            return true;
        }

        protected virtual bool ShouldHandleInputActionAsBackAction(InputAction.CallbackContext _context)
        {
            return false;
        }

        public virtual void HandleBackAction()
        {
            CloseScreen();
        }
        
        public virtual void CloseScreen()
        {
            if (UIKCanvas.instance)
            {
                if (GetType().GetCustomAttribute<UIKScreenAttribute>() is UIKScreenAttribute attribute)
                {
                    UIKCanvas.instance.PopScreen(attribute.name);
                }
            }
        }
    }
} // UIKit namespace

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UIKit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIKActionObjectAttribute : Attribute
    {
        public string actionText;
    }
    
    public abstract class UIKActionObject
    {
        public virtual void OnReceivedContext(params object[] _args)
        {
        }
        
        public virtual bool CanExecute()
        {
            return true;
        }
        
        public abstract void Execute();

        public bool TryExecute()
        {
            if (CanExecute())
            {
                Execute();
                return true;
            }

            return false;
        }
        
        
        private static Dictionary<UIKPlayer, List<UIKActionObject>> actionObjectsByLocalPlayer = new();
        
        
        public static UIKActionObject Get(UIKPlayer _player, Type _type)
        {
            if (!actionObjectsByLocalPlayer.ContainsKey(_player))
            {
                actionObjectsByLocalPlayer.Add(_player, new List<UIKActionObject>());
                foreach (Type actionObjectType in UIKActionObjectReflector.GetAllActionObjectTypes())
                {
                    actionObjectsByLocalPlayer[_player].Add(UIKActionObjectReflector.Create(actionObjectType));
                }
            }
            
            foreach (UIKActionObject actionObject in actionObjectsByLocalPlayer[_player])
            {
                if (actionObject.GetType() == _type)
                {
                    return actionObject;
                }
            }

            return null;
        }
        
        public static void ReceiveContext<T>(UIKPlayer _player, params object[] _args) where T : UIKActionObject
        {
            ReceiveContext(_player, typeof(T), _args);
        }

        public static void ReceiveContext(UIKPlayer _player, Type _type, params object[] _args)
        {
            if (Get(_player, _type) is UIKActionObject actionObject)
            {
                actionObject.OnReceivedContext(_args);
            }
        }
        
        public static bool CanExecute<T>(UIKPlayer _player) where T : UIKActionObject
        {
            return CanExecute(_player, typeof(T));
        }

        public static bool CanExecute(UIKPlayer _player, Type _type)
        {
            if (Get(_player, _type) is UIKActionObject actionObject)
            {
                return actionObject.CanExecute();
            }

            return false;
        }

        public static bool TryExecute<T>(UIKPlayer _player) where T : UIKActionObject
        {
            return TryExecute(_player, typeof(T));
        }

        public static bool TryExecute(UIKPlayer _player, Type _type)
        {
            if (Get(_player, _type) is UIKActionObject actionObject)
            {
                return actionObject.TryExecute();
            }

            return false;
        }
    }

    [Serializable]
    public class UIKActionObjectReference
    {
        public string type;


        public UIKActionObject GetActionObject(UIKPlayer _player)
        {
            if (UIKActionObjectReflector.GetTypeFromString(type) is Type actionObjectType
                && UIKActionObject.Get(_player, actionObjectType) is UIKActionObject actionObject)
            {
                return actionObject;
            }
            
            return null;
        }
    }

    public static class UIKActionObjectExtensions
    {
        public static string GetActionText(this UIKActionObjectReference _actionObjectReference, UIKPlayer _player)
        {
            if (_actionObjectReference.GetActionObject(_player) is UIKActionObject actionObject)
            {
                return actionObject.GetActionText();
            }

            return string.Empty;
        }
        
        public static string GetActionText(this UIKActionObject _actionObject)
        {
            if (_actionObject.GetType().GetCustomAttribute<UIKActionObjectAttribute>() is UIKActionObjectAttribute attribute
                && !string.IsNullOrEmpty(attribute.actionText))
            {
                return attribute.actionText;
            }
            
            return string.Empty;
        }
    }
    
    public static class UIKActionObjectReflector
    {
        private static List<Type> actionObjectTypes = new();
        
        
        static UIKActionObjectReflector()
        {
            actionObjectTypes.Clear();
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(UIKActionObject))
                        && !type.IsAbstract) // Ignore abstract classes since we don't want to register them
                    {
                        actionObjectTypes.Add(type);
                    }
                }
            }
        }
        
        
        public static UIKActionObject Create(Type _type)
        {
            foreach (Type actionObjectType in actionObjectTypes)
            {
                if (actionObjectType == _type
                    && (UIKActionObject)Activator.CreateInstance(actionObjectType) is UIKActionObject actionObject)
                {
                    return actionObject;
                }
            }

            return null;
        }

        public static List<Type> GetAllActionObjectTypes()
        {
            return actionObjectTypes;
        }

        public static Type GetTypeFromString(string _string)
        {
            if (string.IsNullOrEmpty(_string))
            {
                return null;
            }
            
            foreach (Type actionObjectType in actionObjectTypes)
            {
                if (actionObjectType.Name == _string)
                {
                    return actionObjectType;
                }
            }

            return null;
        }
    }
} // UIKit namespace

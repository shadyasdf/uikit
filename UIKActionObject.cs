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
    }

    [Serializable]
    public class UIKActionObjectReference
    {
        public string type;


        public UIKActionObject GetActionObject()
        {
            foreach (UIKActionObject actionObject in UIKActionObjectReflector.GetAllActionObjects())
            {
                if (actionObject.GetType().Name == type)
                {
                    return actionObject;
                }
            }
            
            return null;
        }
    }
    
    public static class UIKActionObjectReflector
    {
        private static List<UIKActionObject> actionObjects = new();
        
        
        static UIKActionObjectReflector()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(UIKActionObject))
                        && !type.IsAbstract) // Ignore abstract classes since we don't want to register them
                    {
                        UIKActionObject actionObject = (UIKActionObject)Activator.CreateInstance(type);
                        if (actionObject != null)
                        {
                            actionObjects.Add(actionObject);
                        }
                    }
                }
            }
        }

        
        private static UIKActionObject Get(Type _type)
        {
            foreach (UIKActionObject actionObject in actionObjects)
            {
                if (actionObject.GetType() == _type)
                {
                    return actionObject;
                }
            }

            return null;
        }

        public static List<UIKActionObject> GetAllActionObjects()
        {
            return actionObjects;
        }
        
        public static void ReceiveContext<T>(params object[] _args) where T : UIKActionObject
        {
            ReceiveContext(typeof(T), _args);
        }

        public static void ReceiveContext(Type _type, params object[] _args)
        {
            if (Get(_type) is UIKActionObject actionObject)
            {
                actionObject.OnReceivedContext(_args);
            }
        }
        
        public static bool CanExecute<T>() where T : UIKActionObject
        {
            return CanExecute(typeof(T));
        }

        public static bool CanExecute(Type _type)
        {
            if (Get(_type) is UIKActionObject actionObject)
            {
                return actionObject.CanExecute();
            }

            return false;
        }

        public static bool TryExecute<T>() where T : UIKActionObject
        {
            return TryExecute(typeof(T));
        }

        public static bool TryExecute(Type _type)
        {
            if (Get(_type) is UIKActionObject actionObject)
            {
                return actionObject.TryExecute();
            }

            return false;
        }
    }
} // UIKit namespace

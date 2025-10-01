using System.Collections.Generic;
using UnityEngine;

namespace UIKit
{
    public static class UIKSafeDestroyer
    {
        private static HashSet<Object> pendingDeletion = new();
        private static bool runningCleanupLoop = false;
        

        static UIKSafeDestroyer()
        {
            StartCleanupLoop();
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif // UNITY_EDITOR
        }
        
        
        public static void SafeDestroy(this Object _object)
        {
            if (_object
                && !pendingDeletion.Contains(_object))
            {
                pendingDeletion.Add(_object);
                MonoBehaviour.Destroy(_object);
            }
        }

        public static bool IsPendingDestroy(this Object _object)
        {
            return _object && pendingDeletion.Contains(_object);
        }
        
        private static async void StartCleanupLoop()
        {
            if (runningCleanupLoop)
            {
                return;
            }

            runningCleanupLoop = true;

            while (runningCleanupLoop)
            {
                Cleanup();
                
                await System.Threading.Tasks.Task.Delay(10000); // 1000ms = 1s
            }
        }

        public static void Cleanup()
        {
            pendingDeletion.Clear();
        }
        
#if UNITY_EDITOR
        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange _stateChange)
        {
            if (_stateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                runningCleanupLoop = false;
                
                Cleanup();
            }
        }
#endif // UNITY_EDITOR
    }
} // UIKit namespace

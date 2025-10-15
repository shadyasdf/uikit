using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

namespace UIKit
{
    [System.Flags]
    [System.Serializable]
    public enum UIKInputDevice
    {
        NONE = 0,
        Gamepad = 1,
        Touch = 2,
        Mouse = 4,
        Keyboard = 8,
        MouseAndKeyboard = Mouse | Keyboard
    }

    public class UIKEventSystem : EventSystem
    {
        [SerializeField] private InputSystemUIInputModule inputModule;

        public static UIKEventSystem instance { get; private set; }

        
        protected override void Awake()
        {
            base.Awake();
            
            if (instance)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        protected override void Update()
        {
            base.Update();

            if (UIKPlayerManager.instance != null)
            {
                foreach (UIKPlayer player in UIKPlayerManager.instance.players)
                {
                    if (player.inputDeviceType.UsesCursor())
                    {
                        // If any of our Cursor players are no longer hovering their currently selected UI, we deselect it
                        if (player.GetTargetUI()
                            && !player.GetTargetUI().hovered)
                        {
                            player.DeselectUI();
                        }
                    }
                }
            }
        }
        

        public InputSystemUIInputModule GetUIInputModule()
        {
            return inputModule;
        }

        public void ExecuteUIEvent<T>(UIKPlayer _player, UIKTarget _target, ExecuteEvents.EventFunction<T> _eventFunction, string _specialInputKey = null) where T : IEventSystemHandler
        {
            if (_player == null
                || !_target)
            {
                Debug.LogError("Failed to execute event because some of the supplied data was invalid");
                return;
            }

            UIKEventData uiEventData = new UIKEventData(_player, _target, this, _specialInputKey);
            ExecuteEvents.Execute(_target.gameObject, uiEventData, _eventFunction);
        }
    }
} // UIKit namespace

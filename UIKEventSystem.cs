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
                foreach (UIKPlayer player in UIKPlayerManager.instance.GetPlayers())
                {
                    if (player.GetInputDeviceType().UsesCursor())
                    {
                        // If we have something selected via the internal Unity event system, and we're not hovering it [as a Cursor player], deselect it
                        if (currentSelectedGameObject)
                        {
                            List<RaycastResult> raycastResults = new();
                            RaycastAll(new(this) { position = Input.mousePosition }, raycastResults);

                            if (!raycastResults.FirstOrDefault(r => r.gameObject == currentSelectedGameObject).isValid)
                            {
                                SetSelectedGameObject(null, new UIKEventData(player, player.selectedUI, this));
                                player.TryDeselectUI(true, false);
                            }
                        }
                        
                        // If any of our Cursor players are no longer hovering their currently selected UI, we deselect it
                        if (player.GetSelectedUI()
                            && !player.GetSelectedUI().hovered)
                        {
                            player.TryDeselectUI();
                        }
                    }
                }
            }
        }
        

        public InputSystemUIInputModule GetUIInputModule()
        {
            return inputModule;
        }

        public void ExecuteUIEvent<T>(UIKPlayer _player, UIKSelectable _selectable, ExecuteEvents.EventFunction<T> _eventFunction, string _specialInputKey = null) where T : IEventSystemHandler
        {
            if (_player == null
                || !_selectable)
            {
                Debug.LogError("Failed to execute event because some of the supplied data was invalid");
                return;
            }

            UIKEventData uiEventData = new UIKEventData(_player, _selectable, this, _specialInputKey);
            ExecuteEvents.Execute(_selectable.gameObject, uiEventData, _eventFunction);
        }
    }
} // UIKit namespace

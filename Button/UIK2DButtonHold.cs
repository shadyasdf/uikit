using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UIKit
{
    public class UIK2DButtonHold : UIK2DButtonPart
    {
        [SerializeField] public bool holdRequired = true;
        [SerializeField] protected float holdDuration = 0.8f;
        [SerializeField] protected Image progressFill;

        protected readonly List<InputAction> heldActions = new();
        protected UIKEventData heldEventData;
        protected float holdTime;

        public bool holding => heldEventData != null;


        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            RefreshProgress();
        }

        protected virtual void Update()
        {
            if (!holding)
            {
                return;
            }

            if (!heldActions.Exists(a => a.IsPressed()))
            {
                CancelHold();
                return;
            }

            holdTime += Time.unscaledDeltaTime;
            RefreshProgress();
            if (holdTime >= holdDuration)
            {
                UIKEventData eventData = heldEventData;
                CancelHold();
                uik2DButton.CompleteClick(eventData);
            }
        }


        public override bool HandleButtonClickDeferred(UIKEventData _eventData)
        {
            if (!holdRequired)
            {
                return false;
            }

            if (holding)
            {
                return true;
            }

            // The press that submitted this click is whichever of the player's actions went down this frame
            heldActions.Clear();
            if (_eventData.pressingPlayer?.playerInput?.actions is InputActionAsset actions)
            {
                foreach (InputAction action in actions)
                {
                    if (action.enabled
                        && action.WasPressedThisFrame())
                    {
                        heldActions.Add(action);
                    }
                }
            }

            if (heldActions.Count == 0)
            {
                return true;
            }

            heldEventData = _eventData;
            holdTime = 0f;
            RefreshProgress();

            return true;
        }

        public void CancelHold()
        {
            heldEventData = null;
            heldActions.Clear();
            holdTime = 0f;
            RefreshProgress();
        }

        protected override void Button_OnUntargeted(UIKPlayer _player)
        {
            base.Button_OnUntargeted(_player);

            CancelHold();
        }

        protected override void Button_OnLockedChanged(bool _locked)
        {
            base.Button_OnLockedChanged(_locked);

            CancelHold();
        }

        protected virtual void RefreshProgress()
        {
            if (progressFill)
            {
                progressFill.fillAmount = holdDuration > 0f ? Mathf.Clamp01(holdTime / holdDuration) : 0f;
            }
        }
    }
} // UIKit namespace

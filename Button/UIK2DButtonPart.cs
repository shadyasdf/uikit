namespace UIKit
{
    public abstract class UIK2DButtonPart : UIKMonoBehaviour
    {
        protected UIK2DButton uik2DButton { get; private set; }


        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            if (!uik2DButton)
            {
                uik2DButton = GetComponent<UIK2DButton>();
            }

            if (!_isOnValidate
                && uik2DButton)
            {
                uik2DButton.OnClickHandled.AddListener(Button_OnClickHandled);
                uik2DButton.OnTargeted.AddListener(Button_OnTargeted);
                uik2DButton.OnUntargeted.AddListener(Button_OnUntargeted);
                uik2DButton.OnLockedChanged.AddListener(Button_OnLockedChanged);
            }
        }

        protected override void OnPreDestroy()
        {
            base.OnPreDestroy();

            if (uik2DButton)
            {
                uik2DButton.OnClickHandled.RemoveListener(Button_OnClickHandled);
                uik2DButton.OnTargeted.RemoveListener(Button_OnTargeted);
                uik2DButton.OnUntargeted.RemoveListener(Button_OnUntargeted);
                uik2DButton.OnLockedChanged.RemoveListener(Button_OnLockedChanged);
            }
        }


        public virtual bool HandleButtonNavigation(UIKPlayer _player, UIKInputDirection _direction)
        {
            return false;
        }

        protected virtual void Button_OnClickHandled(UIKEventData _eventData)
        {
        }

        protected virtual void Button_OnTargeted(UIKPlayer _player)
        {
        }

        protected virtual void Button_OnUntargeted(UIKPlayer _player)
        {
        }

        protected virtual void Button_OnLockedChanged(bool _locked)
        {
        }
    }
} // UIKit namespace

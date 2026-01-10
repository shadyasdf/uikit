using UnityEngine;

namespace UIKit
{
    public abstract class UIKWidget : UIKElement
    {
        public string widgetName { get; protected set; }
        
        public bool active
        {
            get
            {
                return __active;
            }
            protected set
            {
                if (__active != value)
                {
                    __active = value;
                    OnActiveChanged();
                }
            }
        }

        private bool __active = false;

        private UIKWidgetStack widgetStack;


        public void Setup(string _widgetName, UIKWidgetStack _widgetStack)
        {
            widgetName = _widgetName;
            widgetStack = _widgetStack;
        }

        public void Close()
        {
            widgetStack?.PopFromStack(this);
        }
        
        public void SetActive(bool _active)
        {
            if (_active)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }
        
        public void Activate()
        {
            active = true;
        }

        public void Deactivate()
        {
            active = false;
        }

        protected virtual void OnActiveChanged()
        {
            if (active)
            {
                OnActivate();
            }
            else
            {
                OnDeactivate();
            }
        }

        protected virtual void OnActivate()
        {
        }

        protected virtual void OnDeactivate()
        {
        }
    }
} // UIKit namespace

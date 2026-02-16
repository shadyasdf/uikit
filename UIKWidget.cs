using UnityEngine;
using UnityEngine.Events;

namespace UIKit
{
    public abstract class UIKWidget : UIKElement
    {
        [HideInInspector] public UnityEvent<bool> OnActiveChanged = new();
        
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
                    OnActiveChanged.Invoke(__active);
                }
            }
        }

        private bool __active = false;

        private UIKWidgetStack widgetStack;


        protected override void Awake()
        {
            base.Awake();

            OnActiveChanged.AddListener(Widget_OnActiveChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            OnActiveChanged.RemoveListener(Widget_OnActiveChanged);
        }


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

        private void Widget_OnActiveChanged(bool _active)
        {
            if (_active)
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

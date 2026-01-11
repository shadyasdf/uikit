using System.Collections.Generic;
using UIKit;
using UnityEngine;
using UnityEngine.Events;

public class UIKWidgetSwitcher : UIKElement
{
    [HideInInspector] public UnityEvent<UIKWidget> OnCurrentWidgetChanged = new();

    [SerializeField] private int currentWidgetIndex;
    [SerializeField] protected bool manageActivation = true;


    protected override void Awake()
    {
        base.Awake();
        
        UpdateWidgetsInSwitcher();
    }


    public void SetCurrentWidget(int _index)
    {
        if (transform.childCount < _index
            || _index < 0)
        {
            Debug.LogError("Index is out of range in switcher");
            
            return;
        }

        if (currentWidgetIndex == _index)
        {
            return;
        }
        
        currentWidgetIndex = _index;
        
        UpdateWidgetsInSwitcher();
    }

    public int GetCurrentWidgetIndex()
    {
        return currentWidgetIndex;
    }
    
    public UIKWidget GetCurrentWidget()
    {
        return transform.GetChild(currentWidgetIndex)?.GetComponent<UIKWidget>();
    }

    public override UIKTarget GetInnerTarget(UIKInputDirection _direction)
    {
        return GetCurrentWidget()?.GetInnerTarget(_direction);
    }

    protected virtual void UpdateWidgetsInSwitcher()
    {
        // Only consider widget children
        List<UIKWidget> widgets = new();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<UIKWidget>() is UIKWidget widget)
            {
                widgets.Add(widget);
            }
        }
        
        // Deactivate the inactive ones first
        for (int i = 0; i < widgets.Count; i++)
        {
            if (i != currentWidgetIndex)
            {
                widgets[i].gameObject.SetActive(false);
                if (manageActivation)
                {
                    widgets[i].Deactivate();
                }
            }
        }

        if (widgets.Count >= currentWidgetIndex)
        {
            // Active the active one
            widgets[currentWidgetIndex].gameObject.SetActive(true);
            if (manageActivation)
            {
                widgets[currentWidgetIndex].Activate();
            }
            
            OnCurrentWidgetChanged?.Invoke(widgets[currentWidgetIndex]);
        }
        else
        {
            OnCurrentWidgetChanged?.Invoke(null);
        }
    }
}

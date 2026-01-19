using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIKit
{
    public interface UIKInteractable
    {
        public List<UIKInteraction> GetInteractions(UIKInteractor _interactor);
    }
} // UIKit namespace

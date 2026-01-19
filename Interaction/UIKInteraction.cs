using System;
using UnityEngine;

namespace UIKit
{
    [Serializable]
    public abstract class UIKInteraction : ScriptableObject
    {
        public string interactionName;
        
        
        public abstract bool IsAllowed(UIKInteractor _interactor, UIKInteractable _interactable);
        public abstract bool CanExecute(UIKInteractor _interactor, UIKInteractable _interactable);
        public abstract void Execute(UIKInteractor _interactor, UIKInteractable _interactable);
    }
} // UIKit namespace

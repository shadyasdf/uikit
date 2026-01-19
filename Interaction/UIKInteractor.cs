using System.Collections.Generic;
using UnityEngine;

namespace UIKit
{
    public interface UIKInteractor
    {
        public List<UIKInteractable> QueryNearbyInteractables();
        public UIKInteractable QueryPrimaryInteractable(List<UIKInteractable> _interactables);
        public bool TryInteract(UIKInteraction _interaction, UIKInteractable _interactable);
    }
} // UIKit namespace

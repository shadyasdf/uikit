using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    public abstract class UIKElement_2DLayoutGroup : UIKElement
    {
        protected abstract LayoutGroup GetLayoutGroup();
        
        public virtual void AddTarget(UIKElement _element)
        {
            if (_element == null
                || !GetLayoutGroup())
            {
                return;
            }
            
            _element.transform.SetParent(GetLayoutGroup().transform, false);
            
            RefreshNavigation();
        }

        public virtual void RemoveTarget(UIKElement _element)
        {
            if (_element == null)
            {
                return;
            }
            
            _element.gameObject.SafeDestroy();

            RefreshNavigation();
        }

        public virtual void SetTargetVisible(UIKElement _element, bool _visible)
        {
            if (!_element
                || _element.gameObject.activeSelf == _visible)
            {
                return;
            }

            List<UIKPlayer> focusedPlayers = new();
            if (!_visible)
            {
                foreach (UIKTarget target in _element.GetComponentsInChildren<UIKTarget>())
                {
                    focusedPlayers.AddRange(target.targetedByPlayers);
                }
            }

            _element.gameObject.SetActive(_visible);
            RefreshNavigation();

            if (focusedPlayers.Count > 0
                && GetNearestVisibleTarget(_element) is UIKTarget nearestTarget)
            {
                foreach (UIKPlayer player in focusedPlayers)
                {
                    player.TryTargetUI(nearestTarget);
                }
            }
        }

        protected UIKTarget GetNearestVisibleTarget(UIKElement _element)
        {
            List<Transform> children = new();
            foreach (Transform child in GetLayoutGroup().transform)
            {
                children.Add(child);
            }

            int index = children.IndexOf(_element.transform);
            for (int offset = 1; offset < children.Count; offset++)
            {
                foreach (int i in new[] { index - offset, index + offset })
                {
                    if (i >= 0
                        && i < children.Count
                        && children[i].gameObject.activeSelf
                        && !children[i].gameObject.IsPendingDestroy()
                        && children[i].GetComponent<UIKElement>() is UIKElement element
                        && element.GetInnerTarget(UIKInputDirection.Down) is UIKTarget target
                        && target.interactable)
                    {
                        return target;
                    }
                }
            }

            return null;
        }

        public virtual List<UIKElement> GetTargets()
        {
            List<UIKElement> targets = new();
            foreach (Transform child in GetLayoutGroup().transform)
            {
                if (child.GetComponent<UIKElement>() is UIKElement element)
                {
                    targets.Add(element);
                }
            }

            return targets;
        }

        public virtual void ClearTargets()
        {
            foreach (UIKElement target in GetTargets())
            {
                RemoveTarget(target);
            }
        }

        protected abstract void RefreshNavigation();
    }
} // UIKit namespace

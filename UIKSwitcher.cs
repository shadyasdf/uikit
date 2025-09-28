using UnityEngine;

namespace UIKit
{
    public class UIKSwitcher : MonoBehaviour
    {
        public void SetActiveChild(Transform _child)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(child == _child);
            }
        }
    }
} // UIKit namespace

using UnityEngine;

namespace UIKit
{
    public class UIKSwitcher_Name : MonoBehaviour
    {
        public Transform GetChildByName(string _name)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == _name)
                {
                    return child;
                }
            }

            return null;
        }
        
        public void SetActiveChildByName(string _name)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(child.name == _name);
            }
        }
    }
} // UIKit namespace

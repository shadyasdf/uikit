using UnityEngine;

namespace UIKit
{
    public class UIKSwitcher_Name : UIKSwitcher
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
            SetActiveChild(GetChildByName(_name));
        }
    }
} // UIKit namespace

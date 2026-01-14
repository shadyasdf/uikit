using System;
using System.Linq;
using UnityEngine;

namespace UIKit
{
    [CreateAssetMenu(menuName = "UIKit/Input Icon Database", fileName = "InputIconDatabase")]
    public class UIKInputIconDatabase : ScriptableObject
    {
        [Serializable]
        public struct InputIconEntry
        {
            public string path;
            public Sprite icon;
        }

        [SerializeField] private InputIconEntry[] inputIcons;


        public Sprite GetIcon(string _path)
        {
            return inputIcons.FirstOrDefault(i => i.path == _path).icon;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    public class UIK2DButtonImage : UIK2DButtonPart
    {
        [SerializeField] protected Image image;


        public virtual void SetSprite(Sprite _sprite)
        {
            if (image)
            {
                image.sprite = _sprite;
            }
        }

        public virtual Sprite GetSprite()
        {
            return image ? image.sprite : null;
        }
    }
} // UIKit namespace

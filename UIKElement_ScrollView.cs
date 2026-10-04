using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIKElement_ScrollView : UIKElement
    {
        [SerializeField] protected UIKElement content;
        [SerializeField] protected float scrollIntoViewPadding = 4f;

        protected ScrollRect scrollRect;


        protected override void OnPreConstruct(bool _isOnValidate)
        {
            base.OnPreConstruct(_isOnValidate);

            scrollRect = GetComponent<ScrollRect>();
        }


        public override UIKTarget GetInnerTarget(UIKInputDirection _direction)
        {
            return content?.GetInnerTarget(_direction);
        }

        public override void HandleDescendantTargeted(UIKTarget _target, UIKPlayer _player)
        {
            base.HandleDescendantTargeted(_target, _player);

            if (_player != null
                && !_player.inputDeviceType.UsesCursor())
            {
                ScrollIntoView((RectTransform)_target.transform);
            }
        }

        public virtual void ScrollIntoView(RectTransform _target)
        {
            if (!scrollRect
                || !scrollRect.content
                || !scrollRect.viewport)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();

            Rect view = scrollRect.viewport.rect;
            Bounds targetBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(scrollRect.viewport, _target);

            float offset = 0f;
            if (targetBounds.max.y + scrollIntoViewPadding > view.yMax)
            {
                offset = targetBounds.max.y + scrollIntoViewPadding - view.yMax;
            }
            else if (targetBounds.min.y - scrollIntoViewPadding < view.yMin)
            {
                offset = targetBounds.min.y - scrollIntoViewPadding - view.yMin;
            }

            if (offset != 0f)
            {
                scrollRect.StopMovement();
                scrollRect.content.anchoredPosition -= new Vector2(0f, offset);
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
            }
        }
    }
} // UIKit namespace

using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class UIKGridCellFitter : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            Refresh();
        }

        protected virtual void OnRectTransformDimensionsChange()
        {
            Refresh();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            Refresh();
        }


        public void Refresh()
        {
            GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
            Rect rect = ((RectTransform)transform).rect;
            int numColumns = grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount ? Mathf.Max(grid.constraintCount, 1) : 1;
            int numRows = Mathf.Max(Mathf.CeilToInt(GetNumActiveChildren() / (float)numColumns), 1);

            float width = rect.width - grid.padding.horizontal - grid.spacing.x * (numColumns - 1);
            float height = rect.height - grid.padding.vertical - grid.spacing.y * (numRows - 1);
            grid.cellSize = new Vector2(Mathf.Max(width / numColumns, 0f), Mathf.Max(height / numRows, 0f));
        }

        private int GetNumActiveChildren()
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf
                    && !child.gameObject.IsPendingDestroy())
                {
                    count++;
                }
            }

            return count;
        }
    }
} // UIKit namespace

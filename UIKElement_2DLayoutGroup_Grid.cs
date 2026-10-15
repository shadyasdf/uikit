using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class UIKElement_2DLayoutGroup_Grid : UIKElement_2DLayoutGroup
    {
        protected override LayoutGroup GetLayoutGroup()
        {
            return GetComponent<GridLayoutGroup>();
        }

        public int GetNumColumns()
        {
            GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
            return grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount ? Mathf.Max(grid.constraintCount, 1) : 1;
        }

        public void SetNumColumns(int _numColumns)
        {
            GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = Mathf.Max(_numColumns, 1);

            RefreshNavigation();
        }

        public void AddSpacer(GameObject _spacer)
        {
            if (!_spacer)
            {
                return;
            }

            _spacer.transform.SetParent(GetLayoutGroup().transform, false);

            RefreshNavigation();
        }

        public override void ClearTargets()
        {
            foreach (GameObject cell in GetCells())
            {
                cell.SafeDestroy();
            }

            RefreshNavigation();
        }

        public override UIKTarget GetInnerTarget(UIKInputDirection _direction)
        {
            IEnumerable<GameObject> cells = GetCells();
            if (_direction == UIKInputDirection.Up
                || _direction == UIKInputDirection.Left)
            {
                cells = cells.Reverse();
            }

            foreach (GameObject cell in cells)
            {
                if (cell.GetComponent<UIKElement>() is UIKElement element
                    && element.GetInnerTarget(_direction) is UIKTarget target
                    && target.interactable)
                {
                    return target;
                }
            }

            return null;
        }

        protected override void RefreshNavigation()
        {
            List<GameObject> cells = GetCells();
            int numColumns = GetNumColumns();
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetComponent<UIKElement>() is not UIKElement element)
                {
                    continue;
                }

                bool hasRight = (i + 1) % numColumns != 0;
                bool hasLeft = i % numColumns != 0;
                element.navigation.Set(UIKInputDirection.Right, hasRight ? GetElement(cells, i + 1) : null);
                element.navigation.Set(UIKInputDirection.Left, hasLeft ? GetElement(cells, i - 1) : null);
                element.navigation.Set(UIKInputDirection.Down, GetElement(cells, i + numColumns));
                element.navigation.Set(UIKInputDirection.Up, GetElement(cells, i - numColumns));
            }
        }

        private List<GameObject> GetCells()
        {
            List<GameObject> cells = new();
            foreach (Transform child in GetLayoutGroup().transform)
            {
                if (child
                    && child.gameObject.activeSelf
                    && !child.gameObject.IsPendingDestroy())
                {
                    cells.Add(child.gameObject);
                }
            }

            return cells;
        }

        private static UIKElement GetElement(List<GameObject> _cells, int _index)
        {
            return _index >= 0 && _index < _cells.Count ? _cells[_index].GetComponent<UIKElement>() : null;
        }
    }
} // UIKit namespace

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    public class UIK2DButtonCellGrid : UIK2DButtonPart
    {
        [SerializeField] protected GridLayoutGroup grid;
        [SerializeField] protected GameObject cellPrefab;
        [SerializeField] protected GameObject spacerPrefab;


        public void SetCells(int _numColumns, IReadOnlyList<string> _labels)
        {
            if (!grid)
            {
                return;
            }

            foreach (Transform child in grid.transform)
            {
                child.gameObject.SafeDestroy();
            }

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = Mathf.Max(_numColumns, 1);

            foreach (string label in _labels)
            {
                GameObject prefab = label != null ? cellPrefab : spacerPrefab;
                if (!prefab)
                {
                    continue;
                }

                GameObject cell = Instantiate(prefab, grid.transform, false);
                if (label != null)
                {
                    cell.GetComponentInChildren<TMP_Text>()?.SetText(label);
                }
            }

            grid.GetComponent<UIKGridCellFitter>()?.Refresh();
        }
    }
} // UIKit namespace

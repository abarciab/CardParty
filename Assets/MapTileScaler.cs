using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTileScaler : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _layout;

    private const int _referenceWidth = 25;
    private const int _referenceGridSize = 15;
    private const int _nonTileChildren = 3;
    private int _lastCalculatedTotal;

    private void Update()
    {
        var total = transform.childCount;
        if (total == _lastCalculatedTotal || total <= _nonTileChildren) return;
        CalculateDesiredGridSize(total);
    }

    private void CalculateDesiredGridSize(int total)
    {
        total -= _nonTileChildren;
        _lastCalculatedTotal = total;
        var newWidth = Mathf.Sqrt(total);
        var refMax = _referenceGridSize * _referenceWidth;
        var newTargetGridSize = refMax / newWidth;
        _layout.cellSize = Vector2.one * newTargetGridSize;
    }
}

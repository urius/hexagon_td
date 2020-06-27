using System;
using System.Linq;
using UnityEngine;

public class GridView : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _plane;

    private bool _isTransposed;

    public void SetTransposed(bool isTransposed)
    {
        _isTransposed = isTransposed;

        transform.rotation = Quaternion.LookRotation(isTransposed ? Vector3.left : Vector3.forward, Vector3.up);
    }

    public void UpdatePLaneSize()
    {
        var childCells = _grid.gameObject.GetComponentsInChildren<CellView>();
        var maxX = childCells.Max(c => c.transform.position.x);
        var minX = childCells.Min(c => c.transform.position.x);

        var maxY = childCells.Max(c => c.transform.position.z);
        var minY = childCells.Min(c => c.transform.position.z);

        var sizeHor = 0.1f * (maxX - minX);
        var sizeVer = 0.1f * (maxY - minY);

        _plane.transform.localScale = new Vector3(!_isTransposed ? sizeHor : sizeVer,
                                                    1,
                                                    !_isTransposed ? sizeVer : sizeHor);
        _plane.transform.position = new Vector3(
            (maxX + minX) / 2,
            _plane.transform.position.y,
            (maxY + minY) / 2);
    }

    public Func<Vector3Int, Vector3> CellToWorld => _grid.CellToWorld;
    public Func<Vector3, Vector3Int> WorldToCell => _grid.WorldToCell;
}

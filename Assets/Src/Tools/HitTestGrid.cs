using UnityEngine;

public class HitTestGrid : MonoBehaviour
{
    [SerializeField]
    private Grid _grid;

    private Vector3 _hitPoint = Vector3.zero;
    private Vector3 _cellPoint = Vector3.zero;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo))
            {
                _hitPoint = hitInfo.point;

                _cellPoint = _grid.CellToWorld(_grid.WorldToCell(_hitPoint));
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(_hitPoint, 4);
        //Gizmos.DrawCube(_cellPoint, Vector3.one * 3);
    }
}

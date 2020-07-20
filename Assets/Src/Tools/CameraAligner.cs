using System;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class CameraAligner : MonoBehaviour
{
    [SerializeField] private Grid _grid;

#if UNITY_EDITOR
    void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }

        var camera = Camera.main;
        var allCells = _grid.GetComponentsInChildren<CellView>();
        var minZ = allCells.Min(c => c.transform.position.z);
        var maxZ = allCells.Max(c => c.transform.position.z);
        var minX = allCells.Min(c => c.transform.position.x);
        var maxX = allCells.Max(c => c.transform.position.x);

        var cameraSettings = CalculateCameraSettings(camera.transform, minX, maxX, minZ, maxZ, 0.2f);
        camera.orthographicSize = cameraSettings.size;
        var cameraBounds = cameraSettings.bounds;

        var camPos = camera.transform.position;
        camPos.z = cameraBounds.yMin;
        camPos.x = cameraBounds.xMin;
        camera.transform.position = camPos;

        Debug.Log("cameraBounds: " + cameraBounds);
        Debug.Log("xMin/xMax: " + cameraBounds.xMin + "/" + cameraBounds.xMax);
        Debug.Log("yMin/yMax: " + cameraBounds.yMin + "/" + cameraBounds.yMax);
        Debug.Log("center: " + cameraBounds.center);
    }
#endif

    public (float size, Rect bounds) CalculateCameraSettings(
        Transform cameraTransform,
        float horizontalMin,
        float horizontalMax,
        float verticalMin,
        float verticalMax,
        float offsetToGUIPercent)
    {
        var cameraAngle = cameraTransform.rotation.eulerAngles.x;
        var gameViewportSize = new Vector2(Screen.width, Screen.height * (1 - offsetToGUIPercent));

        var fieldWidth = horizontalMax - horizontalMin;
        var fieldHeight = verticalMax - verticalMin;

        var viewportSizeToFitVertical = Math.Sin(Mathf.Deg2Rad * cameraAngle) * fieldHeight * 0.5;
        var viewportSizeToFitHorizontal = fieldWidth * 0.5 * gameViewportSize.y / gameViewportSize.x;

        var isVerticalScrollMode = viewportSizeToFitHorizontal < viewportSizeToFitVertical;
        var viewportVerticalSize = (float)Math.Min(viewportSizeToFitHorizontal, viewportSizeToFitVertical);
        var viewportHorizontalSize = viewportVerticalSize * gameViewportSize.x / gameViewportSize.y;

        var cameraOrtographicSize = viewportVerticalSize / (1 - offsetToGUIPercent);

        var fieldHeightCoveredByViewport = 2 * viewportVerticalSize / Math.Sin(Mathf.Deg2Rad * cameraAngle);
        var fieldHeightCoveredByCamera = 2 * cameraOrtographicSize / Math.Sin(Mathf.Deg2Rad * cameraAngle);
        var cameraOffsetForCenterOnViewport = -0.5f * (fieldHeightCoveredByCamera - fieldHeightCoveredByViewport);
        var fieldCenter = new Vector3((horizontalMax + horizontalMin) * 0.5f, 0, (verticalMax + verticalMin) * 0.5f);
        var cameraDistanceToField = cameraTransform.position.y;
        var cameraZOffset = -Math.Tan(Mathf.Deg2Rad * (90 - cameraAngle)) * cameraDistanceToField + cameraOffsetForCenterOnViewport;

        var cameraCenter = new Vector3(0, cameraDistanceToField);
        cameraCenter.x = fieldCenter.x;
        cameraCenter.z = fieldCenter.z + (float)cameraZOffset;

        var boundsSize = new Vector2(fieldWidth - 2 * viewportHorizontalSize, (float) (fieldHeight - fieldHeightCoveredByViewport));
        var bounds = new Rect(cameraCenter.x - boundsSize.x * 0.5f, cameraCenter.z - boundsSize.y * 0.5f, boundsSize.x, boundsSize.y);

        return (cameraOrtographicSize, bounds);
    }
}

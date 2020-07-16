using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class DebugCameraAligner : MonoBehaviour
{
    [SerializeField] private Grid _grid;

    void Update()
    {
        var camera = Camera.main;
        var allCells = _grid.GetComponentsInChildren<CellView>();
        var minZ = allCells.Min(c => c.transform.position.z);
        var maxZ = allCells.Max(c => c.transform.position.z);
        var minX = allCells.Min(c => c.transform.position.x);
        var maxX = allCells.Max(c => c.transform.position.x);

        var cameraSettings = CalculateCameraSettings(camera, minX, maxX, minZ, maxZ);
        camera.orthographicSize = cameraSettings.size;
        var cameraBounds = cameraSettings.bounds;

        Debug.Log("cameraBounds: " + cameraBounds);
        Debug.Log("xMin/xMax: " + cameraBounds.xMin + "/" + cameraBounds.xMax);
        Debug.Log("yMin/yMax: " + cameraBounds.yMin + "/" + cameraBounds.yMax);
        Debug.Log("center: " + cameraBounds.center);
    }

    public (float size, Rect bounds) CalculateCameraSettings(
        Camera camera,
        float horizontalMin,
        float horizontalMax,
        float verticalMin,
        float verticalMax)
    {
        var cameraTransform = camera.transform;
        var cameraAngle = cameraTransform.rotation.eulerAngles.x;

        var fieldWidth = horizontalMax - horizontalMin;
        var fieldHeight = verticalMax - verticalMin;
        var fieldCenter = new Vector3((horizontalMax + horizontalMin) * 0.5f, 0, (verticalMax + verticalMin) * 0.5f);

        var cameraSizeToFitVertical = Math.Sin(Mathf.Deg2Rad * cameraAngle) * fieldHeight * 0.5;
        var cameraSizeToFitHorizontal = fieldWidth * 0.5 * Screen.height / Screen.width;

        var cameraOrthographicSize = (float)Math.Min(cameraSizeToFitHorizontal, cameraSizeToFitVertical);
        var cameraHorizontalSize = camera.orthographicSize * Screen.width / Screen.height;

        var cameraZOffset = Math.Tan(Mathf.Deg2Rad * (90 - cameraAngle)) * cameraTransform.position.y;
        var cameraCenter = new Vector3(0, cameraTransform.position.y);
        cameraCenter.x = fieldCenter.x;
        cameraCenter.z = fieldCenter.z - (float)cameraZOffset;
        //cameraTransform.position = cameraCenter;

        var cameraHeightProjection = 2 * (camera.orthographicSize / Math.Sin(Mathf.Deg2Rad * cameraAngle)); //actually it's not a projection

        var bounds = new Vector2((float)(fieldWidth - 2 * cameraHorizontalSize), (float)(fieldHeight - cameraHeightProjection));
        var leftBound = cameraCenter.x - bounds.x * 0.5f;
        var topBound = cameraCenter.z - bounds.y * 0.5f;
        var cameraBounds = new Rect(leftBound, topBound, bounds.x, bounds.y);

        var cameraPos = cameraCenter;
        cameraPos.z += cameraBounds.height * 0.5f;
        cameraTransform.position = cameraPos;

        return (cameraOrthographicSize, cameraBounds);
    }
}

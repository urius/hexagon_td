using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class CameraViewMediator : EventMediator
{
    [Inject] public GameCameraView CameraView { get; set; }
    [Inject] public GridViewProvider GridViewProvider { get; set; }
    [Inject] public WorldMousePositionProvider WorldMousePositionProvider { get; set; }

    private Plane _zeroPlane;
    private Bounds _cameraBounds;
    private Vector3 _startDragWorldMousePos;
    private bool _isDragging = false;
    private Vector3 _deltaWorldMouse;
    private Vector3 _currentMouseWorldPoint;

    public override void OnRegister()
    {
        base.OnRegister();

        dispatcher.AddListener(MediatorEvents.DRAW_GRID_COMPLETE, OnDrawGridCompleted);
        dispatcher.AddListener(MediatorEvents.UI_GAME_SCREEN_MOUSE_DOWN, OnGameScreenMouseDown);
        dispatcher.AddListener(MediatorEvents.UI_GAME_SCREEN_MOUSE_UP, OnGameScreenMouseUp);
    }

    public override void OnRemove()
    {
        base.OnRemove();

        dispatcher.RemoveListener(MediatorEvents.DRAW_GRID_COMPLETE, OnDrawGridCompleted);
        dispatcher.RemoveListener(MediatorEvents.UI_GAME_SCREEN_MOUSE_DOWN, OnGameScreenMouseDown);
        dispatcher.RemoveListener(MediatorEvents.UI_GAME_SCREEN_MOUSE_UP, OnGameScreenMouseUp);
    }

    private void Start()
    {
        _zeroPlane = new Plane(Vector3.up, Vector3.zero);
    }

    private void Update()
    {
        _currentMouseWorldPoint = GetMouseWorldPoint();
        WorldMousePositionProvider.SetPosition(_currentMouseWorldPoint);
        ProcessCameraMove();
    }

    private void ProcessCameraMove()
    {
        if (_isDragging)
        {
            _deltaWorldMouse = _currentMouseWorldPoint - _startDragWorldMousePos;
        }
        else if (_deltaWorldMouse.magnitude > 0.05f)
        {
            _deltaWorldMouse *= 0.8f;
        }
        else
        {
            _deltaWorldMouse = Vector3.zero;
            return;
        }

        var newCameraPos = CameraView.CameraPosition - _deltaWorldMouse;
        CameraView.CameraPosition = ClampByBounds(newCameraPos);
    }

    private Vector3 ClampByBounds(Vector3 newCameraPos)
    {
        if (!_cameraBounds.Contains(newCameraPos))
        {
            newCameraPos = _cameraBounds.ClosestPoint(newCameraPos);
        }

        return newCameraPos;
    }

    private void OnGameScreenMouseDown(IEvent payload)
    {
        _startDragWorldMousePos = GetMouseWorldPoint();
        _isDragging = true;
    }

    private void OnGameScreenMouseUp(IEvent payload)
    {
        _isDragging = false;
    }

    private Vector3 GetMouseWorldPoint()
    {
        var ray = CameraView.GetMouseRay(Input.mousePosition);
        _zeroPlane.Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }

    private void OnDrawGridCompleted(IEvent payload)
    {
        var gridView = GridViewProvider.GridView;
        var allCells = gridView.GetComponentsInChildren<CellView>();
        var cellSize = gridView.CellSize;
        var offsetVertical = gridView.IsTransposed ? 0 : cellSize.y * 0.5f;
        var offsetHorizontal = gridView.IsTransposed ? cellSize.x * 0.5f : 0;
        var minZ = allCells.Min(c => c.transform.position.z);// - offsetVertical;
        var maxZ = allCells.Max(c => c.transform.position.z) + offsetVertical;
        var minX = allCells.Min(c => c.transform.position.x) - offsetHorizontal;
        var maxX = allCells.Max(c => c.transform.position.x) + offsetHorizontal;

        var (size, bounds) = CalculateCameraSettings(transform, minX, maxX, minZ, maxZ, 0.2f);
        CameraView.Camera.orthographicSize = size;
        CameraView.CameraPosition = bounds.center;
        _cameraBounds = bounds;
    }

    #region CalculateCamera bounds and size
    private (float size, Bounds bounds) CalculateCameraSettings(
        Transform cameraTransform,
        float horizontalMin,
        float horizontalMax,
        float verticalMin,
        float verticalMax,
        float offsetToGUIPercent)
    {
        Vector3 Vec2ToCameraVec3(Vector2 vec2)
        {
            return new Vector3(vec2.x, cameraTransform.position.y, vec2.y);
        }

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

        var boundsSize = new Vector2(fieldWidth - 2 * viewportHorizontalSize, (float)(fieldHeight - fieldHeightCoveredByViewport));
        var bounds = new Bounds(cameraCenter, Vec2ToCameraVec3(boundsSize));
        return (cameraOrtographicSize, bounds);
    }
    #endregion
}

﻿using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PrefabPainterEditor : EditorWindow
{
    private static string _windowTitle = "Prefab Painter";
    private static string _sceneNameToRestore;

    private bool _isDrawing;
    private GridView _grid;
    private ConfigsHolder _configsProvider;
    private LevelConfig _levelConfig;

    //Cell brush
    private CellType _cellBrushType = CellType.Wall;
    private CellSubType _cellBrushSubType = CellSubType.Default;
    private CellConfig _currentBrushCellConfig;

    //
    private string _saveFileName = "LevelConfig";
    private bool _isTransposed = false;
    private int _numCellsHor = 13;
    private int _numCellsVer = 16;
    private WallType _defaultWall = WallType.Default;
    private GroundType _defaultGround = GroundType.Default;

    [MenuItem("Tools/Cells Prefab Painter")]
    private static void InitWindow()
    {
        _sceneNameToRestore = EditorSceneManager.GetActiveScene().path;
        EditorSceneManager.OpenScene("Assets/Scenes/Helper scenes/EditLevelScene.unity");

        var window = GetWindow<PrefabPainterEditor>();
        window.titleContent = new GUIContent(_windowTitle);

        EditorUtility.FocusProjectWindow();
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/Src/Configs/Levels/LevelConfig");
        Selection.activeObject = obj;
    }

    private void Awake()
    {
        _grid = FindObjectOfType<GridView>();
        _configsProvider = FindObjectOfType<ConfigsHolder>();
    }

    //Calls also if project reloads
    private void OnEnable()
    {
        if (_grid == null || _configsProvider == null)
        {
            return;
        }

        if (_levelConfig != null)
        {
            DrawLevel(_levelConfig);
        }
        else
        {
            ResetLevel();
        }

        SceneView.duringSceneGui += DuringSceneGui;
    }

    private void DrawLevel(LevelConfig levelConfig)
    {
        DestroyAllCells();
        _grid.SetTransposed(levelConfig.IsTransposed);
        foreach (var cell in levelConfig.Cells)
        {
            DrawCell(cell);
        }
        foreach (var modifier in levelConfig.Modifiers)
        {
            DrawModifier(modifier);
        }

        _grid.UpdatePLaneSize();
    }

    private void OnDisable()
    {
        DestroyAllCells();

        EditorSceneManager.OpenScene(_sceneNameToRestore);

        SceneView.duringSceneGui -= DuringSceneGui;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        var prevIsTransposed = _isTransposed;
        _isTransposed = EditorGUILayout.Toggle("Transposed (rotated):", _isTransposed);
        if (prevIsTransposed != _isTransposed)
        {
            OnTransposed();
        }
        _numCellsHor = EditorGUILayout.IntField("Horizontal Size:", _numCellsHor);
        _numCellsVer = EditorGUILayout.IntField("Vertical Size:", _numCellsVer);
        _defaultWall = (WallType)EditorGUILayout.EnumPopup("Default Wall type: ", _defaultWall);
        _defaultGround = (GroundType)EditorGUILayout.EnumPopup("Default Ground type: ", _defaultGround);
        GUILayout.Space(25f);

        var cellBrushTypeOld = _cellBrushType;
        var cellBrushSubTypeOld = _cellBrushSubType;
        _cellBrushType = (CellType)EditorGUILayout.EnumPopup("Cell Brush type: ", _cellBrushType);
        if (_cellBrushType != cellBrushTypeOld)
        {
            _cellBrushSubType = CellSubType.Default;
        }
        _cellBrushSubType = (CellSubType)EditorGUILayout.EnumPopup("Cell Brush subtype: ", _cellBrushSubType.ConvertToSpecifiedEnum(_cellBrushType));
        if (_cellBrushType != cellBrushTypeOld || _cellBrushSubType != cellBrushSubTypeOld)
        {
            UpdateBrushConfig();
        }
        if (_currentBrushCellConfig == null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Error:\nBrush config is invalid");
            EditorGUILayout.EndVertical();
        }
        GUILayout.Space(3f);
        if (GUILayout.Button("Reset level"))
        {
            ResetLevel();
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(50f);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (GUILayout.Button("Load selected level"))
        {
            LoadLevel();
        }
        GUILayout.Space(30f);

        _saveFileName = EditorGUILayout.TextField("File name:", _saveFileName);
        GUILayout.Space(3f);
        if (GUILayout.Button("Save level"))
        {
            SaveLevel();
        }
        EditorGUILayout.EndVertical();


        GUILayout.Space(30f);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Usage:\n\nHold Ctrl + Left mouse to start painting.\nHold Shift + Left mouse to delete painted cells.");
        EditorGUILayout.EndVertical();
    }

    private void OnTransposed()
    {
        _levelConfig.IsTransposed = _isTransposed;
        _grid.SetTransposed(_levelConfig.IsTransposed);
        DrawLevel(_levelConfig);
    }

    private void LoadLevel()
    {
        if (Selection.activeObject is LevelConfig levelConfig)
        {
            _saveFileName = Selection.activeObject.name;

            _levelConfig = Instantiate(levelConfig);

            _isTransposed = levelConfig.IsTransposed;
            DrawLevel(_levelConfig);
        }
    }

    private void SaveLevel()
    {
        var clone = ScriptableObject.Instantiate<LevelConfig>(_levelConfig);

        if (_levelConfig.NameKey == null || _levelConfig.NameKey == string.Empty)
        {
            _levelConfig.NameKey = _saveFileName;
        }
        var folder = "Assets/Src/Configs/Levels/";
        ScriptableObjectCreator.SaveAsset(_levelConfig, folder, _saveFileName, true);
        EditorUtility.FocusProjectWindow();

        _levelConfig = clone;
    }

    private void ResetLevel()
    {
        if (_levelConfig == null)
        {
            RecreateLevelConfig();
        }

        _levelConfig.Reset();

        DestroyAllCells();
        SetupWalls(_levelConfig);
        DrawLevel(_levelConfig);
    }

    private void SetupWalls(LevelConfig levelConfig)
    {
        var wallCellConfig = _configsProvider.CellConfigProvider.GetConfig(CellType.Wall, (CellSubType)_defaultWall).CellConfigMin;
        var groundCellConfig = _configsProvider.CellConfigProvider.GetConfig(CellType.Ground, (CellSubType)_defaultGround).CellConfigMin;

        (int start, int end) GetBoundCoords(int size)
        {
            var bigSize = (int)Math.Ceiling((float)size / 2);
            var smallSize = size - bigSize;
            return (-smallSize, bigSize - 1);
        }
        var horBounds = GetBoundCoords(!levelConfig.IsTransposed ? _numCellsHor : _numCellsVer);
        var verBounds = GetBoundCoords(!levelConfig.IsTransposed ? _numCellsVer : _numCellsHor);

        for (var i = horBounds.start; i <= horBounds.end; i++)
        {
            var topPoint = new Vector2Int(i, verBounds.end);
            var cell = new CellDataMin(topPoint, wallCellConfig);
            levelConfig.AddCell(cell);

            var bottomPoint = new Vector2Int(i, verBounds.start);
            cell = new CellDataMin(bottomPoint, wallCellConfig);
            levelConfig.AddCell(cell);
        }

        for (var i = verBounds.start; i < verBounds.end; i++)
        {
            var leftPoint = new Vector2Int(horBounds.start, i);
            var cell = new CellDataMin(leftPoint, wallCellConfig);
            levelConfig.AddCell(cell);

            var rightPoint = new Vector2Int(horBounds.end, i);
            cell = new CellDataMin(rightPoint, wallCellConfig);
            levelConfig.AddCell(cell);
        }

        for (var i = horBounds.start; i <= horBounds.end; i++)
        {
            for (var j = verBounds.start; j < verBounds.end; j++)
            {
                var point = new Vector2Int(i, j);
                var cell = new CellDataMin(point, groundCellConfig);
                levelConfig.AddCell(cell);
            }
        }
    }

    private void DestroyAllCells()
    {
        _grid.DestroyAllCells();

        var unregisteredCellViews = SceneView.FindObjectsOfType<CellView>();
        foreach (var cellView in unregisteredCellViews)
        {
            DestroyImmediate(cellView.gameObject);
        }
    }

    private void RecreateLevelConfig()
    {
        _levelConfig = ScriptableObject.CreateInstance<LevelConfig>();
        _levelConfig.IsTransposed = _isTransposed;
    }

    private void DuringSceneGui(SceneView sceneView)
    {
        var currentEvent = Event.current;
        var isPaintKeyDown = currentEvent.control;
        var isEraseKeyDown = currentEvent.shift;
        var isUseBrushKeyHold = isPaintKeyDown || isEraseKeyDown;
        if (isUseBrushKeyHold)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (TryGetMouseHit(currentEvent, out var hit))
            {
                Handles.color = Color.green;
                var brushSize = 2f;
                var normalRotation = Quaternion.LookRotation(hit.normal);
                var snappedPoint = _grid.CellToWorld(_grid.WorldToCell(hit.point));
                Handles.ArrowHandleCap(2, snappedPoint, normalRotation, brushSize, EventType.Repaint);
                Handles.CircleHandleCap(3, snappedPoint, normalRotation, brushSize, EventType.Repaint);

                var gridPosition = GetGridPosition(hit.point);
                Handles.BeginGUI();
                GUILayout.Label($"({gridPosition.x},{gridPosition.y})");
                Handles.EndGUI();
            }
        }

        if (isUseBrushKeyHold && currentEvent.type == EventType.MouseDown)
        {
            if (_isDrawing == false)
            {
                OnStartDrawing();
            }
            _isDrawing = true;
        }

        if (_isDrawing && (!isUseBrushKeyHold || currentEvent.type == EventType.MouseUp))
        {
            _isDrawing = false;
        }

        if (_isDrawing)
        {
            if (TryGetMouseHit(currentEvent, out var hit))
            {
                var cellPosition = GetGridPosition(hit.point);
                var isGround = IsGround(cellPosition);
                if (isPaintKeyDown)
                {
                    if (_currentBrushCellConfig != null && (_levelConfig.IsCellFree(cellPosition) || isGround))
                    {
                        var isFree = _levelConfig.IsCellFree(cellPosition);
                        var isBrushOfModifierType = _currentBrushCellConfig.CellConfigMin.CellType == CellType.Modifier;

                        var cellDataMin = new CellDataMin(cellPosition, _currentBrushCellConfig.CellConfigMin);

                        if (isBrushOfModifierType && isGround)
                        {
                            EraseModifier(cellPosition);
                            _levelConfig.AddModifier(cellDataMin);
                            DrawModifier(cellDataMin);
                        }
                        else
                        {
                            EraseCell(cellPosition);
                            _levelConfig.AddCell(cellDataMin);
                            DrawCell(cellDataMin);
                        }
                    }
                }
                else
                {
                    if (!_levelConfig.IsCellFree(cellPosition) && !isGround)
                    {
                        EraseCell(cellPosition);

                        var cellConfig = _configsProvider.CellConfigProvider.GetConfig(CellType.Ground);
                        var cellDataMin = new CellDataMin(cellPosition, cellConfig.CellConfigMin);
                        _levelConfig.AddCell(cellDataMin);

                        DrawCell(cellDataMin);
                    }
                    else if (isGround)
                    {
                        EraseModifier(cellPosition);
                    }
                }
            }
        }
    }

    private void EraseCell(Vector2Int cellPosition)
    {
        _levelConfig.Remove(cellPosition);
        _grid.EraseCell(cellPosition);
    }

    private void EraseModifier(Vector2Int cellPosition)
    {
        _levelConfig.RemoveModifier(cellPosition);
        _grid.EraseModifier(cellPosition);
    }

    private bool IsGround(Vector2Int cellPosition)
    {
        return _levelConfig.IsGround(cellPosition);
    }

    private void DrawCell(CellDataMin cellDataMin)
    {
        var cellConfigMin = cellDataMin.CellConfigMin;
        var cellPosition = cellDataMin.CellPosition;
        var prefab = GetCellFullConfig(cellConfigMin).Prefab;
        var go = _grid.DrawCell(cellPosition, prefab, CellInfoHelper.IsRotatableCell(cellConfigMin.CellType, cellConfigMin.CellSubType));

        go.GetComponent<CellView>().SetDebugText(cellDataMin.CellPosition.x + "," + cellDataMin.CellPosition.y);
    }

    private void DrawModifier(CellDataMin cellDataMin)
    {
        var cellPosition = cellDataMin.CellPosition;
        var prefab = GetCellFullConfig(cellDataMin.CellConfigMin).Prefab;
        var go = _grid.DrawModifier(cellPosition, prefab,
            CellInfoHelper.IsRotatableCell(cellDataMin.CellConfigMin.CellType, cellDataMin.CellConfigMin.CellSubType));
    }

    private CellConfig GetCellFullConfig(CellConfigMin cellConfigMin)
    {
        return _configsProvider.CellConfigProvider.GetConfig(cellConfigMin.CellType, cellConfigMin.CellSubType);
    }

    private Vector2Int GetGridPosition(Vector3 point)
    {
        return CellVec3ToVec2(_grid.WorldToCell(point));
    }

    private void OnStartDrawing()
    {
        if (_levelConfig == null)
        {
            RecreateLevelConfig();
        }
        UpdateBrushConfig();
    }

    private void UpdateBrushConfig()
    {
        _currentBrushCellConfig = _configsProvider.CellConfigProvider.GetConfig(_cellBrushType, _cellBrushSubType);
    }

    private bool TryGetMouseHit(Event currentEvent, out RaycastHit hit)
    {
        var ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
        return Physics.Raycast(ray, out hit);
    }

    private static Vector2Int CellVec3ToVec2(Vector3Int vec3)
    {
        return new Vector2Int(vec3.x, vec3.y);
    }

    private static Vector3Int CellVec2ToVec3(Vector2Int vec2)
    {
        return new Vector3Int(vec2.x, vec2.y, 0);
    }
}

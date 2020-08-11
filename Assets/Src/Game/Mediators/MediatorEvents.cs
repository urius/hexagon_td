using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediatorEvents
{
    public static string SECOND_PASSED = "SECOND_PASSED";

    public static string DRAW_GRID_COMPLETE = "DRAW_GRID_COMPLETE";
    public static string UNIT_SPAWNED = "UNIT_SPAWNED";
    public static string UNIT_MOVE_TO_NEXT_CELL_FINISHED = "UNIT_MOVE_TO_NEXT_CELL_FINISHED";
    public static string UNIT_HALF_STATE_PASSED = "UNIT_HALF_STATE_PASSED";
    public static string REQUEST_BUILD_TURRET = "REQUEST_BUILD_TURRET";
    public static string UNIT_DESTROY_ANIMATION_FINISHED = "UNIT_DESTROY_ANIMATION_FINISHED";
    public static string TURRET_DETECTED_UNIT_IN_ATTACK_ZONE = "TURRET_NOTIFY_UNIT_IN_ATTACK_ZONE";
    public static string TURRET_TARGET_LOCKED = "TURRET_TARGET_LOCKED";
    public static string TURRET_TARGET_LEAVE_ATTACK_ZONE = "TURRET_TARGET_LEAVE_ATTACK_ZONE";    
    public static string BULLET_HIT_TARGET = "BULLET_HIT_TARGET";
    public static string BULLET_HIT_TARGET_ON_CELL = "BULLET_HIT_TARGET_ON_CELL";    

    public static string UI_GAME_SCREEN_MOUSE_DOWN = "UI_GAME_SCREEN_MOUSE_DOWN";
    public static string UI_GAME_SCREEN_MOUSE_UP = "UI_GAME_SCREEN_MOUSE_UP";
    public static string UI_BUILD_TURRET_MOUSE_DOWN = "UI_BUILD_TURRET_MOUSE_DOWN";
    public static string UI_BUILD_TURRET_MOUSE_UP = "UI_BUILD_TURRET_MOUSE_UP";

    public static string DEBUG_BUTTON_CLICKED = "DEBUG_BUTTON_CLICKED";    
}

public static class MediatorEventsParams
{
    public class RequestBuildParams
    {
        public readonly TurretType TurretType;
        public readonly Vector2Int GridPosition;

        public RequestBuildParams(TurretType turretType, Vector3Int gridPosition)
        {
            TurretType = turretType;
            GridPosition = new Vector2Int(gridPosition.x, gridPosition.y);
        }
    }

    public class TurretUnitInAttackZoneParams
    {
        public readonly TurretModel TurretModel;
        public readonly UnitModel UnitModel;

        public TurretUnitInAttackZoneParams(TurretModel turretModel, UnitModel unitModel)
        {
            TurretModel = turretModel;
            UnitModel = unitModel;
        }
    }
}
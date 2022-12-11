using UnityEngine;
using UnityEditor;
using Assets.Src.Common.Local_Save;

[CustomEditor(typeof(PlayerSessionModel))]
public class PlayerGlobalModelHolderEditor : Editor
{
    private string _id;
    private string _gold;

    private void OnEnable()
    {
        _id = SystemInfo.deviceUniqueIdentifier;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Label("\n\n---\nEditor tools:");

        _id = EditorGUILayout.TextField("ID:", _id);
        _gold = EditorGUILayout.TextField("Gold:", _gold);
        if (GUILayout.Button("Load"))
        {
            var modelHolder = (PlayerSessionModel)serializedObject.targetObject;
            Load(modelHolder);
        }

        if (GUILayout.Button("Save"))
        {
            var modelHolder = ((PlayerSessionModel)serializedObject.targetObject);
            Save(modelHolder.PlayerGlobalModel);
        }

        if (GUILayout.Button("Save audio settings"))
        {
            var modelHolder = ((PlayerSessionModel)serializedObject.targetObject);
            SaveAudio(modelHolder.PlayerGlobalModel);
        }

        if (GUILayout.Button("Save gold"))
        {
            var modelHolder = ((PlayerSessionModel)serializedObject.targetObject);
            SaveGold(modelHolder.PlayerGlobalModel);
        }
    }

    private void Load(PlayerSessionModel modelHolder)
    {
        LocalDataManager.Instance.TryLoadUserData(out var result);
        modelHolder.SetModel(new PlayerGlobalModel(result));
        _gold = modelHolder.PlayerGlobalModel.Gold.ToString();
    }

    private void Save(PlayerGlobalModel playerGlobalModel)
    {
        if (_gold != string.Empty && int.TryParse(_gold, out var intGold))
        {
            playerGlobalModel.Gold = intGold;
        }
        var result = LocalDataManager.Instance.SaveUserData(playerGlobalModel.ToSaveDto());
        Debug.Log("Save result: " + result);
    }

    private void SaveAudio(PlayerGlobalModel playerGlobalModel)
    {
        var result = LocalDataManager.Instance.SaveUserData(playerGlobalModel.ToSaveDto());
        Debug.Log("Save audio result: " + result);
    }

    private void SaveGold(PlayerGlobalModel playerGlobalModel)
    {
        if (_gold != string.Empty && int.TryParse(_gold, out var intGold))
        {
            playerGlobalModel.Gold = intGold;
        }
        var result = LocalDataManager.Instance.SaveUserData(playerGlobalModel.ToSaveDto());
        Debug.Log("Save Gold result: " + result);
    }
}

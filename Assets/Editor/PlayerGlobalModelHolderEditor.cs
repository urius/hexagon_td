using UnityEngine;
using UnityEditor;
using System.IO;
using System;

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

    private async void Load(PlayerSessionModel modelHolder)
    {
        var result = await NetworkManager.GetUserDataAsync(_id);
        modelHolder.SetModel(new PlayerGlobalModel(result.Result.payload));
        _gold = modelHolder.PlayerGlobalModel.Gold.ToString();
    }

    private async void Save(PlayerGlobalModel playerGlobalModel)
    {
        if (_gold != string.Empty && int.TryParse(_gold, out var intGold))
        {
            playerGlobalModel.Gold = intGold;
        }
        var result = await NetworkManager.SaveUserDataAsync(_id, playerGlobalModel.ToSaveDto());
        Debug.Log("Save result: " + result);
    }

    private async void SaveAudio(PlayerGlobalModel playerGlobalModel)
    {
        var result = await NetworkManager.SaveUserAudioSettingsAsync(playerGlobalModel.Id, playerGlobalModel.ToSaveSettingsDto());
        Debug.Log("Save audio result: " + result);
    }

    private async void SaveGold(PlayerGlobalModel playerGlobalModel)
    {
        if (_gold != string.Empty && int.TryParse(_gold, out var intGold))
        {
            playerGlobalModel.Gold = intGold;
        }
        var result = await NetworkManager.SaveUserGoldAsync(_id, playerGlobalModel.Gold);
        Debug.Log("Save Gold result: " + result);
    }
}

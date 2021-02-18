using UnityEngine;
using UnityEditor;
using System.IO;
using System;

[CustomEditor(typeof(PlayerGlobalModelHolder))]
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
            var modelHolder = (PlayerGlobalModelHolder)serializedObject.targetObject;
            Load(modelHolder);
        }

        if (GUILayout.Button("Save"))
        {
            var modelHolder = ((PlayerGlobalModelHolder)serializedObject.targetObject);
            Save(modelHolder.PlayerGlobalModel);
        }

        if (GUILayout.Button("Save audio settings"))
        {
            var modelHolder = ((PlayerGlobalModelHolder)serializedObject.targetObject);
            SaveAudio(modelHolder.PlayerGlobalModel);
        }

        if (GUILayout.Button("Save gold"))
        {
            var modelHolder = ((PlayerGlobalModelHolder)serializedObject.targetObject);
            SaveGold(modelHolder.PlayerGlobalModel);
        }
    }

    private async void Load(PlayerGlobalModelHolder modelHolder)
    {
        var result = await NetworkManager.GetUserDataAsync(_id);
        modelHolder.SetModel(result.Result.payload);
        _gold = modelHolder.PlayerGlobalModel.Gold.ToString();
    }

    private async void Save(PlayerGlobalModel playerGlobalModel)
    {
        var result = await NetworkManager.SaveUserDataAsync(playerGlobalModel);
        Debug.Log("Save result: " + result);
    }

    private async void SaveAudio(PlayerGlobalModel playerGlobalModel)
    {
        var result = await NetworkManager.SaveUserAudioSettingsAsync(playerGlobalModel);
        Debug.Log("Save audio result: " + result);
    }

    private async void SaveGold(PlayerGlobalModel playerGlobalModel)
    {
        if (_gold != string.Empty && int.TryParse(_gold, out var intGold))
        {
            playerGlobalModel.Gold = intGold;
        }
        var result = await NetworkManager.SaveUserGoldAsync(playerGlobalModel.Id, playerGlobalModel.Gold);
        Debug.Log("Save Gold result: " + result);
    }
}

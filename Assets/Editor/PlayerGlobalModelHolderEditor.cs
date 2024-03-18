using UnityEngine;
using UnityEditor;
using Src.Common.Commands;
using Src.StartScreen.Commands.NotStrange;

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

        if (GUILayout.Button("Reset"))
        {
            Reset();
        }
    }

    private void Load(PlayerSessionModel modelHolder)
    {
        var playerDataDto = new GetPlayerGlobalModelDtoCommand().Execute();
        modelHolder.SetModel(new PlayerGlobalModel(playerDataDto));
        _gold = modelHolder.PlayerGlobalModel.Gold.ToString();
    }

    private void Save(PlayerGlobalModel playerGlobalModel)
    {
        if (_gold != string.Empty && int.TryParse(_gold, out var intGold))
        {
            playerGlobalModel.Gold = intGold;
        }
        new SaveUserModelCommand().Execute(playerGlobalModel);
        
        Debug.Log("Saved result");
    }

    private void Reset()
    {
        var modelHolder = ((PlayerSessionModel)serializedObject.targetObject);
        var newModel = new PlayerGlobalModel(modelHolder.PlayerGlobalModel.Id);
        modelHolder.SetModel(newModel);
        
        new SaveUserModelCommand().Execute(newModel);
    }
}

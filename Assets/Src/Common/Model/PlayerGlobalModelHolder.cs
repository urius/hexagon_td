﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerGlobalModelHolder", menuName = "Common/Model/PlayerGlobalModelHolder")]
public class PlayerGlobalModelHolder : ScriptableObject
{
    [SerializeField]
    private PlayerGlobalModel _playerGlobalModel;
    public PlayerGlobalModel PlayerGlobalModel => _playerGlobalModel;

    private TaskCompletionSource<bool> _modelInnitializedTsc = new TaskCompletionSource<bool>();
    public Task ModelInnitializedTask => _modelInnitializedTsc.Task;

    public void SetModel(PlayerGlobalModel model)
    {
        _playerGlobalModel = model;
        _modelInnitializedTsc.TrySetResult(true);
    }
}

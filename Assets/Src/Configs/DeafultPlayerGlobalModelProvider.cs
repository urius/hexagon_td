using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeafultPlayerGlobalModelProvider", menuName = "Configs/DeafultPlayerGlobalModelProvider")]
public class DeafultPlayerGlobalModelProvider : ScriptableObject
{
    [SerializeField]
    private PlayerGlobalModel _playerGlobalModel;
    public PlayerGlobalModel PlayerGlobalModel => _playerGlobalModel;
}

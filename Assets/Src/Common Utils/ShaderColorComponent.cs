using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderColorComponent : MonoBehaviour
{
    [SerializeField] private SettingUnit[] _settings;
    [SerializeField] private Color _color;

    //private MaterialPropertyBlock _materialPropertyBlock = new MaterialPropertyBlock();

    void Start()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        foreach (var setting in _settings)
        {
            var materialPropertyBlock = new MaterialPropertyBlock();
            setting.Renderer.GetPropertyBlock(materialPropertyBlock);

            materialPropertyBlock.SetColor(setting.VariableName, _color);

            setting.Renderer.SetPropertyBlock(materialPropertyBlock);
        }
    }

    void Update()
    {
        UpdateColor();
    }

    [Serializable]
    private class SettingUnit
    {
        [SerializeField] public string VariableName;
        [SerializeField] public Renderer Renderer;
    }
}

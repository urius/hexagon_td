using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField] private Text _debugText;

    public void SetDebugText(string text)
    {
        if (_debugText != null)
        {
            _debugText.text = text;
        }
    }
}

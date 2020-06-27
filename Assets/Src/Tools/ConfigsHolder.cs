using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigsHolder : MonoBehaviour
{
    [SerializeField] private CellConfigProvider _cellConfigProvider;
    public CellConfigProvider CellConfigProvider => _cellConfigProvider;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPainter : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    public GameObject Prefab => _prefab;
}

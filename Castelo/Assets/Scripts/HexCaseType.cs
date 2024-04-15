using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hex", menuName = "Hex")]
public class HexCaseType : ScriptableObject
{
    public new string name;
    public float height;
    public Material material;
    public bool isWalkable;
}

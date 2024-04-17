using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Biome", menuName = "Biome")]
public class Biome : ScriptableObject
{
    public HexCaseType hexCaseType;
    public Color color;
    public float spawnRate = 1;
    public bool useByVoronoi = false;
    private int _percentage;

    public void SetPercentage(int percent)
    {
        _percentage = percent;
    }

    public int GetPercentage()
    {
        return _percentage;
    }
}

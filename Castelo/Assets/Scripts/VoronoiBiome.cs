using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "Biome")]
public class VoronoiBiome : ScriptableObject
{
    public HexCaseType hexCaseType;
    public Color color;
    public float spawnRate = 1;
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

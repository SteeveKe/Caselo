using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PostGenerationBiome
{
    public Biome biome;
    public List<Biome> constraintBiomes = new List<Biome>();
    public List<Biome> allowedBiomes = new List<Biome>();
    public int seedNumber = 1;
    public int propagateNumber = 3;
    [Range(0, 1)] public float propagatePower = 0.3f;
    [Range(0, 6)]public int smoothHexRequirement;
    [Range(0, 1)] public float smoothChance;
    public bool isUsed = true;
}

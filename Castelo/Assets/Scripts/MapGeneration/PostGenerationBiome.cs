using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PostGenerationBiome
{
    public Biome biome;
    public List<Biome> constraintBiomes;
    public List<Biome> allowedBiomes;
    public int seedNumber;
    public int propagatePower;
    [Range(0, 6)]public int smoothHexRequirement;
    [Range(0, 1)] public float smoothChance;
}

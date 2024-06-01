using System.Collections.Generic;
using ScriptableObject;
using UnityEngine;

namespace MapGeneration
{
    [System.Serializable]
    public class PostGenerationBiome
    {
        public Biome biome;
        public List<Biome> notNeighboringBiomes = new List<Biome>();
        public List<Biome> neighboringBiomes = new List<Biome>();
        public List<Biome> allowedBiomes = new List<Biome>();
        public int seedNumber = 1;
        public int propagateNumber = 3;
        [Range(0, 1)] public float propagatePower = 0.3f;
        public bool isUsed = true;
    }
}

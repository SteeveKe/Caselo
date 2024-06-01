using System.Collections.Generic;
using ScriptableObject;
using UnityEngine;

namespace MapGeneration
{
    [System.Serializable]
    public class BiomeHeight
    {
        public Biome biome;
        public List<Biome> notAffectedBiome = new List<Biome>();
        public List<Biome> lowerBiome = new List<Biome>();
        public int layerNumber;
        public AnimationCurve curve;
    }
}

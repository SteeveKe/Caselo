using UnityEngine;

namespace ScriptableObject
{
    [CreateAssetMenu(fileName = "New Hex", menuName = "Hex")]
    public class HexCaseType : UnityEngine.ScriptableObject
    {
        public new string name = "new hex";
        public float height = 1;
        public float offSet = 0;
        public float offSetMultiplier = 0;
        public Material material;
        public bool isWalkable = false;

        [Header("PerlinNoise generation")] 
        public float scale = 20;
        [Range(1,10)]public int octaves = 1;
        [Range(0,1)]public float persistance = 0.5f;
        [Range(1,10)]public float lacunarity = 2f;
    }
}

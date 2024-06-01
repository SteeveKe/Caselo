using UnityEngine;

namespace ScriptableObject
{
    [CreateAssetMenu(fileName = "New Biome", menuName = "Biome")]
    public class Biome : UnityEngine.ScriptableObject
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
}

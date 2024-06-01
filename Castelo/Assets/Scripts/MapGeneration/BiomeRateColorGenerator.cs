using ScriptableObject;
using UnityEngine;
using Random = System.Random;

namespace MapGeneration
{
    [RequireComponent(typeof(MapGeneration))]
    public class BiomeRateColorGenerator : MonoBehaviour
    {
        private MapGeneration _mapGeneration;
        private Biome[] _biomes;
        private float _rates = 0;
        private Biome _lastColor;
        public void InitColor()
        {
            _mapGeneration = GetComponent<MapGeneration>();
            _biomes = _mapGeneration.biomes;
            GetRates();
            _lastColor = LastBiome();
        }

        private void GetRates()
        {
            foreach (Biome biome in _biomes)
            {
                if (biome.useByVoronoi)
                {
                    _rates += biome.spawnRate;
                }
            }

            SetPercentage();
        }

        private void SetPercentage()
        {
            foreach (Biome biome in _biomes)
            {
                if (biome.useByVoronoi)
                {
                    biome.SetPercentage(Mathf.RoundToInt(100 * biome.spawnRate / _rates));
                }
            }
        
        }

        public Biome GetRandomBiome(Random random)
        {
            if (_biomes.Length > 0)
            {
                int rdm = random.Next(0, 100);
            
                for (int i = 0; i < _biomes.Length; i++)
                {
                    if (_biomes[i].useByVoronoi)
                    {
                        rdm -= _biomes[i].GetPercentage();
                        if (rdm <= 0)
                        {
                            return _biomes[i];
                        }
                    }
                }

                return _lastColor;
            }
            Debug.Log("ERROR GetRandomBiome");
            return null;
        }

        private Biome LastBiome()
        {
            for (int i = _biomes.Length - 1; i > 0; i--)
            {
                if (_biomes[i].useByVoronoi)
                {
                    return _biomes[i];
                }
            }
            Debug.Log("ERROR LastBiome");
            return null;
        }
    
    }
}

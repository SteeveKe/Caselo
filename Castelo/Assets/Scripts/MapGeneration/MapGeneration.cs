using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ScriptableObject;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace MapGeneration
{
    [RequireComponent(typeof(GridLayout))]
    [RequireComponent(typeof(GenerateMapPattern))]
    [RequireComponent(typeof(BiomeRateColorGenerator))]
    [RequireComponent(typeof(PostVoronoiGeneration))]
    [RequireComponent(typeof(SmoothHeight))]
    public class MapGeneration : MonoBehaviour
    {
        private GridLayout _gridLayout;
        private GenerateMapPattern _generateMapPattern;
        private BiomeRateColorGenerator _biomeRateColorGenerator;
        private PostVoronoiGeneration _postVoronoiGeneration;
        private SmoothHeight _smoothHeight;
        
        private Random _random;

        [Header("GridLayout")] 
        public Vector2Int gridSize;
        public float outerSize;
        public float innerSize;
        public bool isFlatTopped;
        public Transform tileHandler;

        [Header("GenerateMapPattern")] 
        public Biome borderBiome;
        public bool showPoint = false;
        [Range(0, 1)] public float borderPerturbation;
        public Vector2Int patternGrid;
        public int patternGridSize;
        public int mapSeed;
        public RawImage rawImage;
        
        [Header("BiomeRateColorGenerator")]
        public Biome[] biomes;
        
        [Header("PostVoronoiGeneration")]
        public List<PostGenerationBiome> postGenerationBiomes;
        public Vector2Int safeZoneSize;
        public Biome safeZoneBiome;
        
        [Header("SmoothHeight")]
        public List<BiomeHeight> biomeHeights;

        public GridLayout GridLayout => _gridLayout;

        public GenerateMapPattern GenerateMapPattern => _generateMapPattern;

        public BiomeRateColorGenerator BiomeRateColorGenerator => _biomeRateColorGenerator;

        public PostVoronoiGeneration PostVoronoiGeneration => _postVoronoiGeneration;

        public SmoothHeight SmoothHeight => _smoothHeight;

        public Random GetRandom => _random;

        void Start()
        {
            _random = new Random(mapSeed);
            _gridLayout = GetComponent<GridLayout>();
            _generateMapPattern = GetComponent<GenerateMapPattern>();
            _biomeRateColorGenerator = GetComponent<BiomeRateColorGenerator>();
            _postVoronoiGeneration = GetComponent<PostVoronoiGeneration>();
            _smoothHeight = GetComponent<SmoothHeight>();
        
        
            _gridLayout.InitGridLayout();
        }
    }
}

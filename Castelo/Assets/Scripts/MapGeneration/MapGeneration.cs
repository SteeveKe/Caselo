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
        private Vector2Int _patternGrid;
        
        [Header("HexTileInfo")]
        public float outerSize;
        public float innerSize;
        public bool isFlatTopped;

        [Header("HexGridInfo")] 
        public Vector2Int gridSize;
        public Transform tileHandler;
        public int mapSeed;
        [Range(0, 1)] public float borderPerturbation;
        
        [Header("BiomeInfo")] 
        public Biome borderBiome;
        public Biome[] biomes;
        
        [Header("SpawnArea")]
        public Vector2Int safeZoneSize;
        public Biome safeZoneBiome;

        [Header("Voronoi")] 
        public bool showImage = false;
        public bool showImagePoint = false;
        public bool saveImage = false;
        public int patternGridSize;
        public RawImage rawImage;
        
        [Header("PostGeneration")]
        public List<PostGenerationBiome> postGenerationBiomes;
        public List<BiomeHeight> biomeHeights;

        public GridLayout GridLayout => _gridLayout;

        public GenerateMapPattern GenerateMapPattern => _generateMapPattern;

        public BiomeRateColorGenerator BiomeRateColorGenerator => _biomeRateColorGenerator;

        public PostVoronoiGeneration PostVoronoiGeneration => _postVoronoiGeneration;

        public SmoothHeight SmoothHeight => _smoothHeight;

        public Random GetRandom => _random;

        public Vector2Int PatternGrid => _patternGrid;

        void Start()
        {
            _patternGrid = new Vector2Int(gridSize.x / 10, gridSize.y / 10);
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

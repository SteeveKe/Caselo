using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridLayout : MonoBehaviour
{
    public GenerateMapPattern generateMapPattern;
    private Perlin _perlin;
    
    [Header("Grid Settings")]
    public Vector2Int gridSize;

    [Header("Tile Settings")] 
    public float outerSize = 1f;
    public float innerSize = 1f;
    public bool isFlatTopped;
    
    private Dictionary<string, Mesh> _meshesDictionary;
    private Texture2D _texture2D;

    // Start is called before the first frame update
    void Start()
    {
        _meshesDictionary = new Dictionary<string, Mesh>();
        foreach (Biome biome in generateMapPattern.biomeRateColorGenerator.biomes)
        {
            _meshesDictionary.Add(biome.hexCaseType.name, 
                new HexRenderer(innerSize, outerSize, biome.hexCaseType.height, isFlatTopped)._mesh);
        }
        _meshesDictionary.Add(generateMapPattern.border.hexCaseType.name, 
            new HexRenderer(innerSize, outerSize, generateMapPattern.border.hexCaseType.height, isFlatTopped)._mesh);

        _texture2D = generateMapPattern.texture;
        _perlin = new Perlin();
        _perlin.SetSeed(generateMapPattern.seed);
        LayoutGrid();
    }

    private void LayoutGrid()
    {
        Biome[,] hexTilePattern = generateMapPattern.GetHexTilePatternBiomes();
        GameObject newTile = new GameObject($"Hex", typeof(HexTile));

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 position = GetPossitionForHexFromCoordinate(new Vector2Int(x, y));
                GameObject tile = Instantiate(newTile, position, Quaternion.identity, transform);
                tile.name = $"Hex {x},{y}";

                HexTile hexTile = tile.GetComponent<HexTile>();
                HexCaseType hex = hexTilePattern[x, y].hexCaseType;

                MeshFilter meshFilter = hexTile.GetComponent<MeshFilter>();
                meshFilter.mesh = 
                    _meshesDictionary[hex.name];
                
                hexTile.GetComponent<MeshRenderer>().material = 
                    hex.material;
               
                Vector3 pos = tile.transform.position;
                
                float perl = GetNoiseValue(x, y, hex.scale, hex.octaves, hex.persistance, hex.lacunarity, 
                    gridSize.x, gridSize.y);
                pos.y += meshFilter.mesh.bounds.extents.y + hex.offSet + perl * hex.offSetMultiplier;
                tile.transform.position = pos;
            }
        }
        Destroy(newTile);
    }
    
    private float GetNoiseValue(int x, int y, float scale, int octaves, float persistance, float lacunarity,
    int width, int height)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int octave = 0; octave < octaves; octave++)
        {
            float sampleX = x / (float)width * scale * frequency;
            float sampleY = y / (float)height * scale * frequency;

            float perlinValue = _perlin.Noise(sampleX, sampleY);
            noiseHeight += perlinValue * amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        return noiseHeight / octaves;
    }
    
    public Vector3 GetPossitionForHexFromCoordinate(Vector2Int coordinate)
    {
        int column = coordinate.x;
        int row = coordinate.y;
        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = outerSize;

        if (!isFlatTopped)
        {
            shouldOffset = row % 2 == 0;
            width = Mathf.Sqrt(3) * size;
            height = 2f * size;

            horizontalDistance = width;
            verticalDistance = height * (3f / 4f);

            offset = shouldOffset ? width / 2 : 0;

            xPosition = column * horizontalDistance + offset;
            yPosition = row * verticalDistance;
        }
        else
        {
            shouldOffset = column % 2 == 0;
            width = 2f * size;
            height = Mathf.Sqrt(3) * size;

            horizontalDistance = width * (3f / 4f);
            verticalDistance = height;

            offset = shouldOffset ? height / 2 : 0;

            xPosition = column * horizontalDistance;
            yPosition = row * verticalDistance - offset;
        }

        return new Vector3(xPosition, 0, yPosition);
    }
}

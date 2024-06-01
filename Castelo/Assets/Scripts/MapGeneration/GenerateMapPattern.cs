using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

public class GenerateMapPattern : MonoBehaviour
{
    public GridLayout gridLayout;
    public BiomeRateColorGenerator biomeRateColorGenerator;
    public PostVoronoiGeneration postVoronoiGeneration;
    public SmoothHeight smoothHeight;
    public Biome border;
    public bool showPoints = false;

    [Range(0, 1)] public float borderPerturbation;
    public Vector2Int grid;
    public int gridSize;
    public int seed;
    
    public Texture2D texture;
    //private Color[,] _colors;
    private Biome[,] _textureBiomeColor;
    private Biome[,] _textureColorToBiome;
    
    private RawImage _image;
    private Vector2Int[,] _pointsPosition;

    private int _height;
    private int _witdh;

    private Random _random;
    private int _borderPerturbationNumber;
    private Biome[,] _hexTilesPattern;

    public Random GetRandom()
    {
        return _random;
    }
    
    public Biome[,] GetTextureBiomeColor()
    {
        return _textureBiomeColor;
    }
    
        // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GeneratePattern()
    {
        _borderPerturbationNumber = (int)((grid.x + grid.y) * borderPerturbation);
        _random = new Random(seed);
        _witdh = grid.x * gridSize;
        _height = grid.y * gridSize;
        _image = GetComponent<RawImage>();
        _image.GetComponent<RectTransform>().sizeDelta = new Vector2(_witdh, _height);
        texture = new Texture2D(_witdh, _height)
        {
            filterMode = FilterMode.Point
        };
        texture.name = "VoronoiTexture";
        GenerateVoronoi();
        GenerateHexPattern();
    }

    private void GenerateVoronoi()
    {
        GeneratePoints();
        postVoronoiGeneration.GeneratePostBiome();
        
        _textureColorToBiome = new Biome[_witdh, _height];
        for (int height = 0; height < _height; height++)
        {
            for (int width = 0; width < _witdh; width++)
            {
                int gridX = width / gridSize;
                int gridY = height / gridSize;
                
                float nearestDistance = Mathf.Infinity;
                Vector2Int nearestPoint = new Vector2Int();
                
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        int x = gridX + j;
                        int y = gridY + i;

                        if (x < 0 || y < 0 || x >= grid.x || y >= grid.y)
                        {
                            continue;
                        }

                        float distance = Vector2Int.Distance(new Vector2Int(width, height), _pointsPosition[x, y]);
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestPoint = new Vector2Int(x, y);
                        }
                    }
                }

                Biome biome = _textureBiomeColor[nearestPoint.x, nearestPoint.y];
                _textureColorToBiome[width, height] = biome;
                texture.SetPixel(width, height, biome.color);
            }
        }

        if (showPoints)
        {
            for (int i = 0; i < grid.y; i++)
            {
                for (int j = 0; j < grid.x; j++)
                {
                    texture.SetPixel(_pointsPosition[j, i].x, _pointsPosition[j, i].y, Color.black);
                }
            }
        }
        
        texture.Apply();
        _image.texture = texture;
        
        //smoothHeight.SmoothBiomeHeight();
        
        System.IO.File.WriteAllBytes("VoronoiDiagram.png", texture.EncodeToPNG());
    }

    private void GeneratePoints()
    {
        _pointsPosition = new Vector2Int[grid.x, grid.y];
        _textureBiomeColor = new Biome[grid.x, grid.y];
        
        for (int i = 0; i < grid.y; i++)
        {
            for (int j = 0; j < grid.x; j++)
            {
                _pointsPosition[j, i] = new Vector2Int(j * gridSize + _random.Next(1, gridSize),
                    i * gridSize + _random.Next(1, gridSize));
                _textureBiomeColor[j, i] = GetRandomBiome();
            }
        }

        GenerateBorder();
    }

    private void GenerateBorder()
    {
        for (int i = 0; i < grid.y; i++)
        {
            for (int j = 0; j < grid.x; j++)
            {
                if (i == 0 || j == 0 || i == grid.y - 1 || j == grid.x -1)
                {
                    _textureBiomeColor[j, i] = border;
                }
            }
        }
        
        while (_borderPerturbationNumber > 0)
        {
            int selectBorder = _random.Next(0, 4);
            int x = 0;
            int y = 0;
            
            switch (selectBorder)
            {
                case 0:
                    x = 1;
                    y = _random.Next(1, grid.y - 2);
                    break;
                case 1:
                    x = grid.x - 2;
                    y = _random.Next(1, grid.y - 2);
                    break;
                case 2:
                    x = _random.Next(1, grid.x - 2);
                    y = 1;
                    break;
                case 3:
                    x = _random.Next(1, grid.x - 2);
                    y = grid.y - 2;
                    break;
                default:
                    Debug.Log("bugg");
                    break;
            }

            _textureBiomeColor[x, y] = border;
            _borderPerturbationNumber--;
        }
    }

    private Biome GetRandomBiome()
    {
        return biomeRateColorGenerator.GetRandomBiome(_random);
    }

    private void GenerateHexPattern()
    {
        Vector2Int hexCellSize = new Vector2Int(texture.width / gridLayout.gridSize.x , 
            texture.height / gridLayout.gridSize.y);
        int pixelPerCell = hexCellSize.x * hexCellSize.y;

        _hexTilesPattern = new Biome[gridLayout.gridSize.x, gridLayout.gridSize.y];
        
        for (int width = 0; width < gridLayout.gridSize.x; width++)
        {
            for (int height = 0; height < gridLayout.gridSize.y; height++)
            {
                _hexTilesPattern[width, height] = GetMojorityBiome(
                    width * hexCellSize.x, height * hexCellSize.y, hexCellSize, pixelPerCell);
                /*
                _hexTilesPattern[width, height] = GetMatchingVoronoiBiome(GetMojorityBiome(
                    width * hexCellSize.x, height * hexCellSize.y, hexCellSize, pixelPerCell));
                    */
            }
        }
    }

    private Biome GetMojorityBiome(int width, int height, Vector2 cellSize, int pixelPerCell)
    {
        Dictionary<Biome, int> countBiome = new Dictionary<Biome, int>();
        
        for (int i = height; i < height + cellSize.y; i++)
        {
            for (int j = width; j < width + cellSize.x; j++)
            {
                //Color color = texture.GetPixel(width, height);
                Biome biome = _textureColorToBiome[width, height];
                if (countBiome.ContainsKey(biome))
                {
                    countBiome[biome] += 1;
                }
                else
                {
                    countBiome.Add(biome, 1);
                }

                if (countBiome[biome] >= pixelPerCell / 2)
                {
                    return biome;
                }
            }
        }

        int max = -1;
        Biome maxBiome = border;
        foreach (Biome biome in countBiome.Keys)
        {
            if (countBiome[biome] > max)
            {
                max = countBiome[biome];
                maxBiome = biome;
            }
        }

        return maxBiome;
    }

    /*
    private Biome GetMatchingVoronoiBiome(Color color)
    {
        foreach (Biome biome in biomeRateColorGenerator.biomes)
        {
            if (IsSameColor(color, biome.color, 0.01f))
            {
                return biome;
            }
        }

        return border;
    }

    private bool IsSameColor(Color color1, Color color2, float threshold)
    {
        float r = Mathf.Abs(color1.r - color2.r);
        float g = Mathf.Abs(color1.g - color2.g);
        float b = Mathf.Abs(color1.b - color2.b);
        float a = Mathf.Abs(color1.a - color2.a);

        return r < threshold && g < threshold && b < threshold && a < threshold;
    }
    */

    public Biome[,] GetHexTilePatternBiomes()
    {
        return _hexTilesPattern;
    }
    
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GenerateMapPattern : MonoBehaviour
{
    public GridLayout gridLayout;
    public BiomeRateColorGenerator biomeRateColorGenerator;
    public Biome border;
    public bool showPoints = false;

    [Range(0, 1)] public float borderPerturbation;
    public Vector2Int grid;
    public int gridSize;
    public int seed;
    
    public Texture2D texture;
    private Color[,] _colors;
    private Biome[,] _textureBiome;
    
    private RawImage _image;
    private Vector2Int[,] _pointsPosition;

    private int _height;
    private int _witdh;

    private System.Random _random;
    private int _borderPerturbationNumber;
    private Biome[,] _hexTilesPattern;

        // Start is called before the first frame update
    void Start()
    {
        _borderPerturbationNumber = (int)((grid.x + grid.y) * borderPerturbation);
        _random = new System.Random(seed);
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
                texture.SetPixel(width, height, _colors[nearestPoint.x, nearestPoint.y]);
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
        
        System.IO.File.WriteAllBytes("VoronoiDiagram.png", texture.EncodeToPNG());
    }

    private void GeneratePoints()
    {
        _pointsPosition = new Vector2Int[grid.x, grid.y];
        _colors = new Color[grid.x, grid.y];
        
        for (int i = 0; i < grid.y; i++)
        {
            for (int j = 0; j < grid.x; j++)
            {
                _pointsPosition[j, i] = new Vector2Int(j * gridSize + _random.Next(1, gridSize),
                    i * gridSize + _random.Next(1, gridSize));
                _colors[j, i] = GetRandomColor();
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
                    _colors[j, i] = border.color;
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

            _colors[x, y] = border.color;
            _borderPerturbationNumber--;
        }
    }

    private Color GetRandomColor()
    {
        return biomeRateColorGenerator.GetRandomColor(_random);
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
                _hexTilesPattern[width, height] = GetMatchingVoronoiBiome(GetMojorityColor(
                    width * hexCellSize.x, height * hexCellSize.y, hexCellSize, pixelPerCell));
            }
        }
    }

    private Color GetMojorityColor(int width, int height, Vector2 cellSize, int pixelPerCell)
    {
        Dictionary<Color, int> countColor = new Dictionary<Color, int>();
        
        for (int i = height; i < height + cellSize.y; i++)
        {
            for (int j = width; j < width + cellSize.x; j++)
            {
                Color color = texture.GetPixel(width, height);
                if (countColor.ContainsKey(color))
                {
                    countColor[color] += 1;
                }
                else
                {
                    countColor.Add(color, 1);
                }

                if (countColor[color] >= pixelPerCell / 2)
                {
                    return color;
                }
            }
        }

        int max = -1;
        Color maxColor = border.color;
        foreach (Color color in countColor.Keys)
        {
            if (countColor[color] > max)
            {
                max = countColor[color];
                maxColor = color;
            }
        }

        return maxColor;
    }

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

    public Biome[,] GetHexTilePatternBiomes()
    {
        return _hexTilesPattern;
    }
}

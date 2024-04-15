using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GenerateMapPattern : MonoBehaviour
{
    public BiomeRateColorGenerator biomeRateColorGenerator;
    public Color borderColor;
    public bool showPoints = false;

    [Range(0, 1)] public float borderPerturbation;
    public Vector2Int grid;
    public int gridSize;
    public int seed;
    
    public Texture2D texture;
    
    private Color[] _possibleColors;
    private Color[,] _colors;
    
    private RawImage _image;
    private Vector2Int[,] _pointsPosition;

    private int _height;
    private int _witdh;

    private System.Random _random;
    private int _borderPerturbationNumber;

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
                    _colors[j, i] = borderColor;
                }
            }
        }
        
        while (_borderPerturbationNumber > 0)
        {
            int selectBorder = _random.Next(0, 5);
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

            _colors[x, y] = borderColor;
            _borderPerturbationNumber--;
        }
    }

    private Color GetRandomColor()
    {
        return biomeRateColorGenerator.GetRandomColor(_random);
    }
}

using System.Collections.Generic;
using ScriptableObject;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace MapGeneration
{
    [RequireComponent(typeof(MapGeneration))]
    public class GenerateMapPattern : MonoBehaviour
    {
        private MapGeneration _mapGeneration;
        private BiomeRateColorGenerator _biomeRateColorGenerator;
        private PostVoronoiGeneration _postVoronoiGeneration;
        private Biome _border;
        private bool _showPoints;

        private float _borderPerturbation;
        private Vector2Int _grid;
        private int _gridSize;
    
        private Texture2D _texture;
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

        public void GeneratePattern()
        {
            _mapGeneration = GetComponent<MapGeneration>();
            _biomeRateColorGenerator = _mapGeneration.BiomeRateColorGenerator;
            _postVoronoiGeneration = _mapGeneration.PostVoronoiGeneration;
            _border = _mapGeneration.borderBiome;
            _showPoints = _mapGeneration.showImagePoint;
            _grid = _mapGeneration.PatternGrid;
            _gridSize = _mapGeneration.patternGridSize;

            _borderPerturbation = _mapGeneration.borderPerturbation;
            _borderPerturbationNumber = (int)((_grid.x + _grid.y) * _borderPerturbation);
            _random = _mapGeneration.GetRandom;
            _witdh = _grid.x * _gridSize;
            _height = _grid.y * _gridSize;
            _image = _mapGeneration.rawImage;
            _image.GetComponent<RectTransform>().sizeDelta = new Vector2(_witdh, _height);
            _texture = new Texture2D(_witdh, _height)
            {
                filterMode = FilterMode.Point
            };
            _texture.name = "VoronoiTexture";
            _mapGeneration.rawImage.gameObject.SetActive(_mapGeneration.showImage);
            GenerateVoronoi();
            GenerateHexPattern();
        }

        private void GenerateVoronoi()
        {
            GeneratePoints();
            _postVoronoiGeneration.GeneratePostBiome();
        
            _textureColorToBiome = new Biome[_witdh, _height];
            for (int height = 0; height < _height; height++)
            {
                for (int width = 0; width < _witdh; width++)
                {
                    int gridX = width / _gridSize;
                    int gridY = height / _gridSize;
                
                    float nearestDistance = Mathf.Infinity;
                    Vector2Int nearestPoint = new Vector2Int();
                
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            int x = gridX + j;
                            int y = gridY + i;

                            if (x < 0 || y < 0 || x >= _grid.x || y >= _grid.y)
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
                    _texture.SetPixel(width, height, biome.color);
                }
            }

            if (_showPoints)
            {
                for (int i = 0; i < _grid.y; i++)
                {
                    for (int j = 0; j < _grid.x; j++)
                    {
                        _texture.SetPixel(_pointsPosition[j, i].x, _pointsPosition[j, i].y, Color.black);
                    }
                }
            }
        
            _texture.Apply();
            _image.texture = _texture;
            if (_mapGeneration.saveImage)
            {
                System.IO.File.WriteAllBytes("VoronoiDiagram.png", _texture.EncodeToPNG());
            }
        }

        private void GeneratePoints()
        {
            _pointsPosition = new Vector2Int[_grid.x, _grid.y];
            _textureBiomeColor = new Biome[_grid.x, _grid.y];
            _biomeRateColorGenerator.InitColor();
        
            for (int i = 0; i < _grid.y; i++)
            {
                for (int j = 0; j < _grid.x; j++)
                {
                    _pointsPosition[j, i] = new Vector2Int(j * _gridSize + _random.Next(1, _gridSize),
                        i * _gridSize + _random.Next(1, _gridSize));
                    _textureBiomeColor[j, i] = GetRandomBiome();
                }
            }

            GenerateBorder();
        }

        private void GenerateBorder()
        {
            for (int i = 0; i < _grid.y; i++)
            {
                for (int j = 0; j < _grid.x; j++)
                {
                    if (i == 0 || j == 0 || i == _grid.y - 1 || j == _grid.x -1)
                    {
                        _textureBiomeColor[j, i] = _border;
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
                        y = _random.Next(1, _grid.y - 2);
                        break;
                    case 1:
                        x = _grid.x - 2;
                        y = _random.Next(1, _grid.y - 2);
                        break;
                    case 2:
                        x = _random.Next(1, _grid.x - 2);
                        y = 1;
                        break;
                    case 3:
                        x = _random.Next(1, _grid.x - 2);
                        y = _grid.y - 2;
                        break;
                    default:
                        Debug.Log("bugg");
                        break;
                }

                _textureBiomeColor[x, y] = _border;
                _borderPerturbationNumber--;
            }
        }

        private Biome GetRandomBiome()
        {
            return _biomeRateColorGenerator.GetRandomBiome(_random);
        }

        private void GenerateHexPattern()
        {
            Vector2Int hexCellSize = new Vector2Int(_texture.width / _mapGeneration.gridSize.x , 
                _texture.height / _mapGeneration.gridSize.y);
            int pixelPerCell = hexCellSize.x * hexCellSize.y;

            _hexTilesPattern = new Biome[_mapGeneration.gridSize.x, _mapGeneration.gridSize.y];
        
            for (int width = 0; width < _mapGeneration.gridSize.x; width++)
            {
                for (int height = 0; height < _mapGeneration.gridSize.y; height++)
                {
                    _hexTilesPattern[width, height] = GetMojorityBiome(
                        width * hexCellSize.x, height * hexCellSize.y, hexCellSize, pixelPerCell);
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
            Biome maxBiome = _border;
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

        public Biome[,] GetHexTilePatternBiomes()
        {
            return _hexTilesPattern;
        }
    
    
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothHeight : MonoBehaviour
{
    public List<BiomeHeight> biomeHeights;
    private GridLayout _gridLayout;
    private Biome[,] _hexMapBiomes;
    private bool[,] _hexMapNewBorder;
    private Dictionary<Biome, List<Vector2Int>> _biomePosition;
    private float[,] _heightMap;

    public void SetNewHexHeight()
    {
        SmoothBiomeHeight();

        Mesh[,] hexMesh = _gridLayout.GetHexMeshes();
        Transform[,] hexTransform = _gridLayout.GetHexTransform();

        for (int height = 0; height < _heightMap.GetLength(1); height++)
        {
            for (int width = 0; width < _heightMap.GetLength(0); width++)
            {
                Vector3 newPos = hexTransform[width, height].position;
                newPos.y = _heightMap[width, height]- hexMesh[width, height].bounds.extents.y;
                hexTransform[width, height].position = newPos;
            }
        }
    }

    private void SmoothBiomeHeight()
    {
        _biomePosition = new Dictionary<Biome, List<Vector2Int>>();
        _gridLayout = FindObjectOfType<GridLayout>();
        SetHexMapBiome(_gridLayout.generateMapPattern.GetHexTilePatternBiomes());
        _heightMap = _gridLayout.GetHeightMap();

        GetAllPossibleBorder();
        
        foreach (BiomeHeight biomeHeight in biomeHeights)
        {
            ResetHexMapNewBorder();
            Smooth(biomeHeight.biome, 1, biomeHeight.layerNumber, biomeHeight.curve, 
                biomeHeight.notAffectedBiome, biomeHeight.lowerBiome);
        }
    }

    private void ResetHexMapNewBorder()
    {
        _hexMapNewBorder = new bool[_hexMapBiomes.GetLength(0), _hexMapBiomes.GetLength(1)];
        
        for (int height = 0; height < _hexMapNewBorder.GetLength(1); height++)
        {
            for (int width = 0; width < _hexMapNewBorder.GetLength(0); width++)
            {
                _hexMapNewBorder[width, height] = false;
            }
        }
    }

    private void SetHexMapBiome(Biome[,] hexMap)
    {
        _hexMapBiomes = new Biome[hexMap.GetLength(0), hexMap.GetLength(1)];

        for (int height = 0; height < hexMap.GetLength(1); height++)
        {
            for (int width = 0; width < hexMap.GetLength(0); width++)
            {
                _hexMapBiomes[width, height] = hexMap[width, height];
            }
        }
    }

    private void Smooth(Biome biome, int layerNumber, int maxLayerNumber, AnimationCurve curve, List<Biome> notAffectedBiomes, List<Biome> lowerBiomes)
    {
        List<Vector2Int> borderPos = GetAllBiomeBorder(biome, notAffectedBiomes);

        foreach (Vector2Int pos in borderPos)
        {
            float newHeight = GetAverageHeight(biome, notAffectedBiomes, lowerBiomes, pos);
            newHeight = curve.Evaluate((float)layerNumber / maxLayerNumber) * newHeight;
            _heightMap[pos.x, pos.y] = newHeight;

            if (!_biomePosition[biome].Remove(pos))
            {
                Debug.Log("Smooth error dictionary");
            }
        }

        foreach (Vector2Int pos in borderPos)
        {
            _hexMapNewBorder[pos.x, pos.y] = true;
        }

        if (layerNumber < maxLayerNumber && _biomePosition[biome].Count != 0)
        {
            Smooth(biome, layerNumber + 1, maxLayerNumber, curve, notAffectedBiomes, lowerBiomes);
        }
    }

    private float GetAverageHeight(Biome biome, List<Biome> notAffectedBiomes, List<Biome> lowerBiomes, Vector2Int pos)
    {
        int number = 0;
        float sumHeight = 0;
        float lowerBiomeHeight = -Mathf.Infinity;
        
        for (int height = pos.y - 1; height < pos.y + 2; height++)
        {
            for (int width = pos.x - 1; width < pos.x + 2; width++)
            {

                if (height < 0 || width < 0 ||
                    height >= _hexMapBiomes.GetLength(1) || width >= _hexMapBiomes.GetLength(0))
                {
                    continue;
                }
                
                if ((height == pos.y - 1 && width == pos.x - 1) ||
                    (height == pos.y + 1 && width == pos.x - 1))
                {
                    continue;
                }
                
                if (width == pos.x && height == pos.y)
                {
                    number++;
                    sumHeight += _heightMap[width, height];
                }
                else
                {
                    if (IsBorder(notAffectedBiomes, biome, width, height))
                    {
                        number++;
                        sumHeight += _heightMap[width, height];
                    }
                }
                
                if (lowerBiomes.Contains(_hexMapBiomes[width, height]))
                {
                    if (lowerBiomeHeight < _heightMap[width, height])
                    {
                        lowerBiomeHeight = _heightMap[width, height];
                    }
                }
            }
        }

        float averageHeight = 0;

        if (number != 0)
        {
            averageHeight = sumHeight / number;
            if (averageHeight <= lowerBiomeHeight)
            {
                averageHeight += Mathf.Abs((lowerBiomeHeight - averageHeight) * 2);
            }
        }
        else
        {
            averageHeight = _heightMap[pos.x, pos.y];
        }

        return averageHeight;
    }
    
    

    private void GetAllPossibleBorder()
    {
        _biomePosition = new Dictionary<Biome, List<Vector2Int>>();
        for (int height = 0; height < _hexMapBiomes.GetLength(1); height++)
        {
            for (int width = 0; width < _hexMapBiomes.GetLength(0); width++)
            {
                if (ContainBiome(_hexMapBiomes[width, height]))
                {
                    AddBiomeToDictionary(_hexMapBiomes[width, height], width, height);
                }
            }
        }
    }

    private void AddBiomeToDictionary(Biome biome, int width, int height)
    {
        if (_biomePosition.ContainsKey(biome))
        {
            _biomePosition[biome].Add(new Vector2Int(width, height));
        }
        else
        {
            _biomePosition.Add(biome, new List<Vector2Int>());
            _biomePosition[biome].Add(new Vector2Int(width, height));
        }
    }

    private bool ContainBiome(Biome biome)
    {
        foreach (BiomeHeight biomeHeight in biomeHeights)
        {
            if (biomeHeight.biome.Equals(biome))
            {
                return true;
            }
        }

        return false;
    }

    private List<Vector2Int> GetAllBiomeBorder(Biome biome, List<Biome> notAffectedBiomes)
    {
        List<Vector2Int> biomeBorder = new List<Vector2Int>();
        foreach (Vector2Int pos in _biomePosition[biome])
        {
            if (IsBiomeBorder(biome, pos, notAffectedBiomes))
            {
                biomeBorder.Add(pos);
            }
        }

        return biomeBorder;
    }

    private bool IsBiomeBorder(Biome biome, Vector2Int position, List<Biome> notAffectedBiomes)
    {
        for (int height = position.y - 1; height < position.y + 2; height++)
        {
            for (int width = position.x - 1; width < position.x + 2; width++)
            {

                if (height < 0 || width < 0 ||
                    height >= _hexMapBiomes.GetLength(1) || width >= _hexMapBiomes.GetLength(0))
                {
                    continue;
                }

                if (width == position.x && height == position.y)
                {
                    continue;
                }

                if ((height == position.y - 1 && width == position.x - 1) ||
                    (height == position.y + 1 && width == position.x - 1))
                {
                    continue;
                }

                if (IsBorder(notAffectedBiomes, biome, width, height))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsBorder(List<Biome> notAffectedBiomes, Biome biome, int width, int height)
    {

        if (_hexMapNewBorder[width, height])
        {
            return true;
        }
        
        if (_hexMapBiomes[width, height].Equals(biome))
        {
            return false;
        }

        foreach (Biome b in notAffectedBiomes)
        {
            if (_hexMapBiomes[width, height].Equals(b))
            {
                return false;
            }
        }

        return true;
    }
}

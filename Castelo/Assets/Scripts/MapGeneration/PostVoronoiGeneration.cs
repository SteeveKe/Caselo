using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class PostVoronoiGeneration : MonoBehaviour
{
    public List<PostGenerationBiome> postGenerationBiomes;
    public Vector2Int safeZoneSize;
    public Biome safeZoneBiome;

    //_textureBiomeColor to change
    private Biome[,] _textureBiome;
    private GenerateMapPattern _generateMapPattern;
    private Dictionary<Biome, List<Vector2Int>> _biomePosition;
    private List<Vector2Int> _safeZonePosition;
    private Random _random;

    private void Start()
    {

    }

    public void GeneratePostBiome()
    {
        _generateMapPattern = GetComponent<GenerateMapPattern>();
        _textureBiome = _generateMapPattern.GetTextureBiomeColor();
        _random = _generateMapPattern.GetRandom();
        GenerateSafeZone();
        InitBiomePosition();


        foreach (PostGenerationBiome postGeneration in postGenerationBiomes)
        {
            if (postGeneration.isUsed)
            {
                List<Vector2Int> positions = GetValidBiomePosition(postGeneration.notNeighboringBiomes,
                    postGeneration.neighboringBiomes, postGeneration.allowedBiomes);
                PlantBiomeSeed(positions, postGeneration.seedNumber, postGeneration.propagateNumber,
                    postGeneration.propagatePower, postGeneration.biome);
            }
        }
    }

    private void PlantBiomeSeed(List<Vector2Int> allowedPositions, int seedNumber, int propagateNumber,
        float propagatePower, Biome biome)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int i = 0; i < seedNumber; i++)
        {
            if (allowedPositions.Count != 0)
            {
                Vector2Int pos = allowedPositions[_random.Next(0, allowedPositions.Count)];

                while (positions.Count < allowedPositions.Count && positions.Contains(pos))
                {
                    pos = allowedPositions[_random.Next(0, allowedPositions.Count)];
                }

                if (positions.Count < allowedPositions.Count)
                {
                    positions.Add(pos);
                }
                else
                {
                    Debug.Log("all allowed possition used in plantBiomeSeed");
                }
            }
            else
            {
                Debug.Log("PlantBiomeSeed no more position");
            }
        }

        if (positions.Count != 0)
        {
            PropagateBiome(allowedPositions, positions, propagateNumber, propagatePower, biome);
        }
        else
        {
            Debug.Log("PlantBiomeSeed no position");
        }
    }

    private void PropagateBiome(List<Vector2Int> allowedPositions, List<Vector2Int> positions,
        int propagateNumber, float propagatePower, Biome biome)
    {
        foreach (Vector2Int pos in positions)
        {
            ChangeDictionaryBiome(biome, pos);
            _textureBiome[pos.x, pos.y] = biome;

            if (!allowedPositions.Remove(pos))
            {
                Debug.Log("Propagate position not exist");
            }
        }

        if (propagateNumber > 0)
        {
            List<Vector2Int> adjacent = GetAllAdjacent(positions, allowedPositions);

            if (adjacent.Count > 0)
            {
                List<Vector2Int> newPositions = GetRandomPossitions(adjacent, propagateNumber, propagatePower);
                PropagateBiome(allowedPositions, newPositions, propagateNumber - newPositions.Count, propagatePower,
                    biome);
            }
            else
            {
                Debug.Log("no more adjacent in PropagateBiome");
            }
        }
    }

    private void ChangeDictionaryBiome(Biome biome, Vector2Int pos)
    {
        if (_biomePosition[_textureBiome[pos.x, pos.y]].Remove(pos))
        {
            SetBiomePosition(pos.x, pos.y, biome);
        }
        else
        {
            Debug.Log("can't change dictionaryBiome");
        }
    }

    private List<Vector2Int> GetAllAdjacent(List<Vector2Int> positions, List<Vector2Int> allowedPositions)
    {
        List<Vector2Int> adjacent = new List<Vector2Int>();

        foreach (Vector2Int pos in positions)
        {
            List<Vector2Int> adj = GetValidAjacentPosition(pos, allowedPositions);
            foreach (Vector2Int p in adj)
            {
                if (!adjacent.Contains(p))
                {
                    adjacent.Add(p);
                }
            }
        }

        return adjacent;
    }

    private List<Vector2Int> GetRandomPossitions(List<Vector2Int> adjacent, int propagateNumber, float propagatePower)
    {
        List<Vector2Int> randomPositions = new List<Vector2Int>();

        while (randomPositions.Count == 0)
        {
            for (int i = 0; i < adjacent.Count; i++)
            {
                if (propagateNumber <= 0)
                {
                    break;
                }

                Vector2Int pos = adjacent[_random.Next(0, adjacent.Count)];
                if (propagatePower - _random.Next(0, 1) >= 0)
                {
                    randomPositions.Add(pos);
                    adjacent.Remove(pos);
                    propagateNumber--;
                }
            }
        }

        return randomPositions;
    }

    private void RemovePosFromDictionary(Vector2Int position)
    {
        foreach (var positions in _biomePosition)
        {
            if (positions.Value.Contains(position))
            {
                positions.Value.Remove(position);
                return;
            }
        }

        Debug.Log("position not removed from dictionary");
    }

    private List<Vector2Int> GetValidAjacentPosition(Vector2Int position, List<Vector2Int> allowedPositions)
    {
        List<Vector2Int> adjacent = new List<Vector2Int>();

        for (int height = position.y - 1; height < position.y + 2; height++)
        {
            for (int width = position.x - 1; width < position.x + 2; width++)
            {

                if (height < 0 || width < 0 ||
                    height >= _textureBiome.GetLength(1) || width >= _textureBiome.GetLength(0))
                {
                    continue;
                }

                if (width == position.x && height == position.y)
                {
                    continue;
                }

                if ((height == position.y - 1 && width == position.x - 1) ||
                    (height == position.y - 1 && width == position.x + 1) ||
                    (height == position.y + 1 && width == position.x - 1) ||
                    (height == position.y + 1 && width == position.x + 1))
                {
                    continue;
                }

                Vector2Int pos = new Vector2Int(width, height);
                if (allowedPositions.Contains(pos))
                {
                    adjacent.Add(pos);
                }
            }
        }

        return adjacent;
    }

    private void GenerateSafeZone()
    {
        _safeZonePosition = new List<Vector2Int>();

        int width = _textureBiome.GetLength(0);
        int height = _textureBiome.GetLength(1);

        for (int i = height / 2 - safeZoneSize.y / 2; i < height / 2 + safeZoneSize.y / 2 + safeZoneSize.y % 2; i++)
        {
            for (int j = width / 2 - safeZoneSize.x / 2; j < width / 2 + safeZoneSize.x / 2 + safeZoneSize.x % 2; j++)
            {
                _textureBiome[j, i] = safeZoneBiome;
                _safeZonePosition.Add(new Vector2Int(j, i));
            }
        }
    }

    private void InitBiomePosition()
    {
        _biomePosition = new Dictionary<Biome, List<Vector2Int>>();
        for (int i = 0; i < _textureBiome.GetLength(1); i++)
        {
            for (int j = 0; j < _textureBiome.GetLength(0); j++)
            {
                if (!_safeZonePosition.Contains(new Vector2Int(j, i)))
                {
                    SetBiomePosition(j, i, _textureBiome[j, i]);
                }
            }
        }
    }

    private void SetBiomePosition(int width, int height, Biome biome)
    {
        if (_biomePosition.ContainsKey(biome))
        {
            _biomePosition[biome].Add(new Vector2Int(width, height));
        }
        else
        {
            List<Vector2Int> list = new List<Vector2Int>();
            list.Add(new Vector2Int(width, height));
            _biomePosition.Add(biome, list);
        }
    }

    private List<Vector2Int> GetValidBiomePosition(List<Biome> notNeighbaring, List<Biome> neighboring,
        List<Biome> allowed)
    {
        List<Vector2Int> allowedPosition = GetAllowed(allowed);
        allowedPosition = GetNeighboring(notNeighbaring, allowedPosition, false);
        return GetNeighboring(neighboring, allowedPosition, true);
    }

    private List<Vector2Int> GetNeighboring(List<Biome> neighboring, List<Vector2Int> allowedPosition, bool allow)
    {
        List<Vector2Int> validPosition = new List<Vector2Int>();

        if (neighboring.Count == 0)
        {
            return allowedPosition;
        }
        else
        {
            foreach (Vector2Int position in allowedPosition)
            {
                if (IsNeighboringPosValid(position, neighboring, allow))
                {
                    validPosition.Add(position);
                }
            }
        }

        return validPosition;
    }

    private bool IsNeighboringPosValid(Vector2Int position, List<Biome> neighboring, bool allow)
    {
        for (int height = position.y - 1; height < position.y + 2; height++)
        {
            for (int width = position.x - 1; width < position.x + 2; width++)
            {
                if (height < 0 || width < 0 ||
                    height >= _textureBiome.GetLength(1) || width >= _textureBiome.GetLength(0))
                {
                    continue;
                }

                if (width == position.x && height == position.y)
                {
                    continue;
                }

                if (allow)
                {
                    if ((height == position.y - 1 && width == position.x - 1) ||
                        (height == position.y - 1 && width == position.x + 1) ||
                        (height == position.y + 1 && width == position.x - 1) ||
                        (height == position.y + 1 && width == position.x + 1))
                    {
                        continue;
                    }

                    if (neighboring.Contains(_textureBiome[width, height]))
                    {
                        return true;
                    }
                }
                else
                {
                    if (neighboring.Contains(_textureBiome[width, height]))
                    {
                        return false;
                    }
                }
            }
        }

        return !allow;
    }

    private List<Vector2Int> GetAllowed(List<Biome> allowed)
    {
        List<Vector2Int> validPosition = new List<Vector2Int>();

        if (allowed.Count == 0)
        {
            validPosition = GetAllAllowedBiome();
        }
        else
        {
            foreach (Biome biome in allowed)
            {
                List<Vector2Int> position = GetAllowedBiome(biome);
                foreach (Vector2Int pos in position)
                {
                    if (!_safeZonePosition.Contains(pos))
                    {
                        validPosition.Add(pos);
                    }
                }
            }
        }
        return validPosition;
    }

    private List<Vector2Int> GetAllowedBiome(Biome biome)
    {
        List<Vector2Int> validPosition = new List<Vector2Int>();
        
        if (_biomePosition.ContainsKey(biome))
        {
            foreach (Vector2Int position in _biomePosition[biome])
            {
                validPosition.Add(position);
            }
        }
        else
        {
            Debug.Log("GetValidBiome error");
        }

        return validPosition;
    }

    private List<Vector2Int> GetAllAllowedBiome()
    {
        List<Vector2Int> validPosition = new List<Vector2Int>();
        
        foreach (var biomePosition in _biomePosition)
        {
            foreach (Vector2Int position in biomePosition.Value)
            {
                validPosition.Add(position);
            }
        }

        return validPosition;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BiomeRateColorGenerator : MonoBehaviour
{
    public Biome[] biomes;
    private float _rates = 0;
    private Color _lastColor;
    private void Start()
    {
        GetRates();
        _lastColor = LastBiomeColor();
    }

    private void GetRates()
    {
        foreach (Biome biome in biomes)
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
        foreach (Biome biome in biomes)
        {
            if (biome.useByVoronoi)
            {
                biome.SetPercentage(Mathf.RoundToInt(100 * biome.spawnRate / _rates));
            }
        }
        
    }

    public Color GetRandomColor(Random random)
    {
        if (biomes.Length > 0)
        {
            int rdm = random.Next(0, 100);
            
            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i].useByVoronoi)
                {
                    rdm -= biomes[i].GetPercentage();
                    if (rdm <= 0)
                    {
                        return biomes[i].color;
                    }
                }
            }

            return _lastColor;
        }
        return Color.black;
    }

    private Color LastBiomeColor()
    {
        for (int i = biomes.Length - 1; i > 0; i--)
        {
            if (biomes[i].useByVoronoi)
            {
                return biomes[i].color;
            }
        }
        
        return Color.clear;
    }
    
}

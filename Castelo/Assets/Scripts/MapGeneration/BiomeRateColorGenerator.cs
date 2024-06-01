using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BiomeRateColorGenerator : MonoBehaviour
{
    public Biome[] biomes;
    private float _rates = 0;
    private Biome _lastColor;
    public void InitColor()
    {
        GetRates();
        _lastColor = LastBiome();
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

    public Biome GetRandomBiome(Random random)
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
                        return biomes[i];
                    }
                }
            }

            return _lastColor;
        }
        Debug.Log("ERROR GetRandomBiome");
        return null;
    }

    private Biome LastBiome()
    {
        for (int i = biomes.Length - 1; i > 0; i--)
        {
            if (biomes[i].useByVoronoi)
            {
                return biomes[i];
            }
        }
        Debug.Log("ERROR LastBiome");
        return null;
    }
    
}

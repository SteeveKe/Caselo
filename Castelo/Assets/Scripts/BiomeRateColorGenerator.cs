using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BiomeRateColorGenerator : MonoBehaviour
{
    public VoronoiBiome[] biomes;
    private float _rates = 0;
    private void Start()
    {
        GetRates();
    }

    private void GetRates()
    {
        foreach (VoronoiBiome biome in biomes)
        {
            _rates += biome.spawnRate;
        }

        SetPercentage();
    }

    private void SetPercentage()
    {
        foreach (VoronoiBiome biome in biomes)
        {
            biome.SetPercentage(Mathf.RoundToInt(100 * biome.spawnRate / _rates));
        }
        
    }

    public Color GetRandomColor(Random random)
    {
        if (biomes.Length > 0)
        {
            int rdm = random.Next(0, 100);
        
            int index = 0;
            rdm -= biomes[index].GetPercentage();
        
            while (rdm > 0 && index < biomes.Length)
            {
                index++;
                rdm -= biomes[index].GetPercentage();
            }

            return biomes[index].color;
        }
        return Color.black;
    }
    
}

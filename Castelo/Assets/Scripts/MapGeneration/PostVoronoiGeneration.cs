using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PostVoronoiGeneration : MonoBehaviour
{
    public List<PostGenerationBiome> postGenerationBiomes;
    private GenerateMapPattern _generateMapPattern;

    private void Start()
    {
        _generateMapPattern = GetComponent<GenerateMapPattern>();
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridLayout : MonoBehaviour
{
    public GenerateMapPattern generateMapPattern;

    [Header("Grid Settings")]
    public Vector2 gridSize;

    [Header("Tile Settings")] 
    public float outerSize = 1f;
    public float innerSize = 1f;
    public bool isFlatTopped;
    private Dictionary<string, Mesh> _meshesDictionary;

    // Start is called before the first frame update
    void Start()
    {
        _meshesDictionary = new Dictionary<string, Mesh>();
        foreach (VoronoiBiome biome in generateMapPattern.biomeRateColorGenerator.biomes)
        {
            _meshesDictionary.Add(biome.hexCaseType.name, 
                new HexRenderer(innerSize, outerSize, biome.hexCaseType.height, isFlatTopped)._mesh);
        }
        LayoutGrid();
    }

    private void LayoutGrid()
    {
        GameObject newTile = new GameObject($"Hex", typeof(HexTile));
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 position = GetPossitionForHexFromCoordinate(new Vector2Int(x, y));
                GameObject tile = Instantiate(newTile, position, Quaternion.identity, transform);
                tile.name = $"Hex {x},{y}";

                HexTile hexTile = tile.GetComponent<HexTile>();

                int index = Random.Range(0, _meshesDictionary.Count);
                hexTile.GetComponent<MeshFilter>().mesh = 
                    _meshesDictionary[generateMapPattern.biomeRateColorGenerator.biomes[index].name];

                Vector3 pos = tile.transform.position;
                pos.y += hexTile.GetComponent<MeshFilter>().mesh.bounds.extents.y;

                tile.transform.position = pos;

                hexTile.GetComponent<MeshRenderer>().material = 
                    generateMapPattern.biomeRateColorGenerator.biomes[index].hexCaseType.material;
            }
        }
        Destroy(newTile);
    }

    public Vector3 GetPossitionForHexFromCoordinate(Vector2Int coordinate)
    {
        int column = coordinate.x;
        int row = coordinate.y;
        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = outerSize;

        if (!isFlatTopped)
        {
            shouldOffset = row % 2 == 0;
            width = Mathf.Sqrt(3) * size;
            height = 2f * size;

            horizontalDistance = width;
            verticalDistance = height * (3f / 4f);

            offset = shouldOffset ? width / 2 : 0;

            xPosition = column * horizontalDistance + offset;
            yPosition = row * verticalDistance;
        }
        else
        {
            shouldOffset = column % 2 == 0;
            width = 2f * size;
            height = Mathf.Sqrt(3) * size;

            horizontalDistance = width * (3f / 4f);
            verticalDistance = height;

            offset = shouldOffset ? height / 2 : 0;

            xPosition = column * horizontalDistance;
            yPosition = row * verticalDistance - offset;
        }

        return new Vector3(xPosition, 0, yPosition);
    }
}

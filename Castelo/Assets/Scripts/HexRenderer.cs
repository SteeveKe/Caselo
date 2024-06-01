using System.Collections.Generic;
using UnityEngine;

public struct Face
{
    public List<Vector3> Vertices { get; private set; }
    public List<int> Triangles { get; private set; }
    public List<Vector2> Uvs { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.Vertices = vertices;
        this.Triangles = triangles;
        this.Uvs = uvs;
    }
}

public class HexRenderer
{
    public Mesh _mesh;
    private List<Face> _faces;
    

    private float _innerSize;
    private float _outerSize;
    private float _height;
    private bool _isFlatTopped;

    public HexRenderer(float innerSize, float outerSize, float height, bool isFlatTopped)
    {
        _mesh = new Mesh
        {
            name = "Hex"
        };
        _innerSize = innerSize;
        _outerSize = outerSize;
        _height = height;
        _isFlatTopped = isFlatTopped;
        DrawMesh();
    }

    private void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
    }

    private void DrawFaces()
    {
        _faces = new List<Face>();

        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(_innerSize, _outerSize, _height / 2f, _height / 2f, point));
        }
        
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(_innerSize, _outerSize, -_height / 2f, -_height / 2f, point, true));
        }

        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(_outerSize, _outerSize, _height / 2f, -_height / 2f, point, true));
        }
        
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(_innerSize, _innerSize, _height / 2f, -_height / 2f, point));
        }
    }

    private void CombineFaces()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < _faces.Count; i++)
        {
            vertices.AddRange(_faces[i].Vertices);
            uvs.AddRange(_faces[i].Uvs);

            int offset = 4 * i;
            foreach (int triangle in _faces[i].Triangles)
            {
                triangles.Add(triangle + offset);
            }
        }

        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        _mesh.uv = uvs.ToArray();
        _mesh.RecalculateNormals();
    }

    private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point,
        bool reverse = false)
    {
        Vector3 pointA = GetPoint(innerRad, heightB, point);
        Vector3 pointB = GetPoint(innerRad, heightB, (point < 5) ? point + 1 : 0);
        Vector3 pointC = GetPoint(outerRad, heightA, (point < 5) ? point + 1 : 0);
        Vector3 pointD = GetPoint(outerRad, heightA, point);

        List<Vector3> vertices = new List<Vector3>() { pointA, pointB, pointC, pointD };
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>() { new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)};

        if (reverse)
        {
            vertices.Reverse();
        }
        return new Face(vertices, triangles, uvs);
    }

    protected Vector3 GetPoint(float size, float height, int index)
    {
        float angleDeg = _isFlatTopped ? 60 * index : 60 * index - 30;
        float angleRad = Mathf.PI / 180f * angleDeg;
        return new Vector3(size * Mathf.Cos(angleRad), height, size * Mathf.Sin(angleRad));
    }
}

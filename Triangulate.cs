using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Meshing;
using TriangleNet.Geometry;

public class Triangulate : MonoBehaviour
{
    public TriangleNet.Mesh trianglee;
    public TriangleNet.Mesh triangle(List<Vector2> points)
    {
        var polygon = new Polygon();
        foreach (var point in points)
        {
            polygon.Add(new Vertex(point.x, point.y));
        }

        var qualityOptions = new QualityOptions();
        trianglee = (TriangleNet.Mesh)polygon.Triangulate(qualityOptions);
        
        foreach (var triangleVertex in trianglee.Vertices)
        {
            Debug.Log($"Vertex: {triangleVertex.X}, {triangleVertex.Y}");
        }
        return trianglee;
    }
    

}
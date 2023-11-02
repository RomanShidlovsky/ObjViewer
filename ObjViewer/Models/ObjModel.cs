using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ObjViewer.Models;

public class ObjModel : ICloneable
{
    public List<Vector4> Points { get; set; }
    public List<List<Vector3>> Faces { get; set; }
    public List<List<Vector3>> TriangleFaces { get; set; }
    public List<Vector3> Textures { get; set; }
    public List<Vector3> Normals { get; set; }
    public List<Vector4> WorldPoints { get; set; }
    public List<Vector4> LocalPoints { get; set; }


    public ObjModel(List<Vector4> points, List<List<Vector3>> faces, List<Vector3> texture, List<Vector3> normal,
        List<List<Vector3>> triangleFaces)
    {
        Points = new List<Vector4>();
        WorldPoints = new List<Vector4>();
        LocalPoints = points;
        Faces = faces;
        TriangleFaces = triangleFaces;
        Textures = texture;
        Normals = normal;
    }

    public bool CheckSize(int width, int height)
    {
        bool result = true;
        bool outOfLeftSide = false, outOfRightSide = false, outOfTopSide = false, outOfBottomSide = false;

        _ = Parallel.ForEach(Points, point =>
        {
            if (point.X <= 0)
            {
                outOfLeftSide = true;
            }

            if (point.X >= width)
            {
                outOfRightSide = true;
            }

            if (point.Y <= 0)
            {
                outOfTopSide = true;
            }

            if (point.Y >= height)
            {
                outOfBottomSide = true;
            }
        });

        if ((outOfLeftSide && outOfRightSide) || (outOfTopSide && outOfBottomSide))
        {
            result = false;
        }

        return result;
    }

    public bool IsPointInModelRect(double x, double y)
    {
        var xValues = Points.Select(p => p.X).ToArray();
        var yValues = Points.Select(p => p.Y).ToArray();

        float minX = xValues.Min();
        float maxX = xValues.Max();
        float minY = yValues.Min();
        float maxY = yValues.Max();

        return x > minX && x < maxX &&
               y > minY && y < maxY;
    }

    public object Clone()
    {
        List<Vector4> clonePoints = Points.Select(x => x).ToList();
        List<Vector3> cloneNormals = Normals.Select(x => x).ToList();
        ObjModel obj = new(clonePoints, Faces, Textures, cloneNormals, TriangleFaces);

        return obj;
    }
}
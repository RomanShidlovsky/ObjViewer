using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using ObjViewer.Core.Lightings;
using ObjViewer.Models;

namespace ObjViewer.Core.Shaders;

public class FlatShading : Bresenham
{
    protected ZBuffer ZBuffer { get; set; }
    public ILighting Lighting { get; set; }
    
    public FlatShading() {}
    
    public FlatShading(Bgra32Bitmap bitmap, ILighting lighting, ObjModel model, Color? color = null) : base(bitmap,
        model)
    {
        Lighting = lighting;
        ZBuffer = new ZBuffer(Bitmap.PixelWidth, Bitmap.PixelHeight);
    }

    public void SetParams(Bgra32Bitmap bitmap, ILighting lighting, ObjModel model, Color? color = null)
    {
        ZBuffer.Reset();
        Bitmap = bitmap;
        Lighting = lighting;
        Model = model;
        if (color is not null)
            Color = color.Value;
    }
    
    protected override void DrawFace(List<Vector3> face)
    {
        Color color = GetFaceColor(face, Color);
        Vector3[] points = GetFacePoints(face);
        
        ScanlineTriangle(points[0], points[1], points[2], color);
    }

    protected virtual Vector3[] GetFacePoints(List<Vector3> face)
    {
        Vector3[] points = new Vector3[face.Count];

        for (int i = 0; i < face.Count; i++)
        {
            Vector4 point = Model.Points[(int)face[i].X];
            points[i] = new Vector3(point.X, point.Y, point.Z);
        }

        return points;
    }
    
    protected virtual void ScanlineTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        if (a.Y > c.Y) (a, c) = (c, a);
        if (a.Y > b.Y) (a, b) = (b, a);
        if (b.Y > c.Y) (b, c) = (c, b);
        
        Vector3 kp1 = new Vector3(c.X - a.X, c.Y - a.Y, 0) / (c.Y - a.Y);
        Vector3 kp2 = new Vector3(b.X - a.X, b.Y - a.Y, 0) / (b.Y - a.Y);
        Vector3 kp3 = new Vector3(c.X - b.X, c.Y - b.Y, 0) / (c.Y - b.Y);

        int top = int.Max(0, (int)Math.Ceiling(a.Y));
        int bottom = int.Min(Bitmap.PixelHeight, (int)Math.Ceiling(c.Y));

        for (int y = top; y < bottom; y++)
        {
            Vector3 lp = a + (y - a.Y) * kp1;
            Vector3 rp = y < b.Y ? a + (y - a.Y) * kp2 : b + (y - b.Y) * kp3;

            if (lp.X > rp.X)
            {
                (lp, rp) = (rp, lp);
            }
            
            int left = int.Max(0, (int)Math.Ceiling(lp.X));
            int right = int.Min(Bitmap.PixelWidth, (int)Math.Ceiling(rp.X));
            
            float z = lp.Z;
            float dz = (rp.Z - lp.Z) / (rp.X - lp.X);

            for (int x = left; x < right; x++, z+=dz)
            {
                DrawPixel(x, y, z, color);
            }

        }
    }

    protected virtual void DrawPixel(int x, int y, float z, Color color)
    {
        if (z > 0 && z < 1 && z <= ZBuffer[x, y])
        {
            ZBuffer[x, y] = z;
            Bitmap[x, y] = color;
        }
    }

    protected Color GetFaceColor(List<Vector3> face, Color color)
    {
        Vector3 normal1 = Model.Normals[(int)face[0].Z];
        Vector3 normal2 = Model.Normals[(int)face[1].Z];
        Vector3 normal3 = Model.Normals[(int)face[2].Z];

        Color color1 = Lighting.GetPointColor(normal1, color);
        Color color2 = Lighting.GetPointColor(normal2, color);
        Color color3 = Lighting.GetPointColor(normal3, color);

        return GetAverageColor(color1, color2, color3);
    }

    public static Color GetAverageColor(params Color[] colors)
    {
        int sumR = 0;
        int sumG = 0;
        int sumB = 0;
        int sumA = 0;

        foreach (var color in colors)
        {
            sumR += color.R;
            sumG += color.G;
            sumB += color.B;
            sumA += color.A;
        }

        int colorCount = colors.Length;
        byte r = (byte)Math.Round((double)sumR / colorCount);
        byte g = (byte)Math.Round((double)sumG / colorCount);
        byte b = (byte)Math.Round((double)sumB / colorCount);
        byte a = (byte)Math.Round((double)sumA / colorCount);

        return Color.FromArgb(a, r, g, b);
    }
}
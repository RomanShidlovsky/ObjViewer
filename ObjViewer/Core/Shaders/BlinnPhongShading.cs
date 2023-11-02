using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ObjViewer.Core.Lightings;
using ObjViewer.Models;
using System.Windows.Media;

namespace ObjViewer.Core.Shaders;

public class BlinnPhongShading : Bresenham
{
    protected ZBuffer ZBuffer { get; set; }
    public ILighting Lighting { get; set; }
    
    public Color AmbientColor { get; set; }
    public Color DiffuseColor { get; set; }
    public Color SpecularColor { get; set; }
    
    public BlinnPhongShading(
        Bgra32Bitmap bitmap, 
        ILighting lighting, 
        ObjModel model) 
        : base(bitmap, model)
    {
        Lighting = lighting;
        ZBuffer = new ZBuffer(Bitmap.PixelWidth, Bitmap.PixelHeight);
    }

    public void SetParams(Bgra32Bitmap bitmap, ILighting lighting, ObjModel objModel)
    {
        
    }
    
    protected override void DrawFace(List<Vector3> face)
    {
        Vector3[] points = face.Select(f => Model.Points[(int)f.X]).Select(p => new Vector3(p.X, p.Y, p.Z)).ToArray();
        Vector3[] worldPoints = face.Select(f => Model.WorldPoints[(int)f.X]).Select(p => new Vector3(p.X, p.Y, p.Z)).ToArray();
        Vector3[] normals = face.Select(f => Model.Normals[(int)f.Z]).ToArray();
        
        ScanlineTriangle((points[0], normals[0], worldPoints[0]), (points[1], normals[1], worldPoints[1]), (points[2], normals[2], worldPoints[2]), Color);
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

    protected virtual Vector3[] GetFaceNormals(List<Vector3> face)
    {
        Vector3[] normals = new Vector3[face.Count];

        for (int i = 0; i < face.Count; i++)
        {
            normals[i] = Model.Normals[(int)face[i].Y];
        }

        return normals;
    }
    
    protected virtual void ScanlineTriangle((Vector3 p, Vector3 n, Vector3 w) a, (Vector3 p, Vector3 n, Vector3 w) b, (Vector3 p, Vector3 n, Vector3 w) c, Color modelColor)
    {
        if (a.p.Y > c.p.Y) (a, c) = (c, a);
        if (a.p.Y > b.p.Y) (a, b) = (b, a);
        if (b.p.Y > c.p.Y) (b, c) = (c, b);
        
        Vector3 kp1 = (c.p - a.p) / (c.p.Y - a.p.Y);
        Vector3 kp2 = (b.p - a.p) / (b.p.Y - a.p.Y);
        Vector3 kp3 = (c.p - b.p) / (c.p.Y - b.p.Y);
        Vector3 kn1 = (c.n - a.n) / (c.p.Y - a.p.Y);
        Vector3 kn2 = (b.n - a.n) / (b.p.Y - a.p.Y);
        Vector3 kn3 = (c.n - b.n) / (c.p.Y - b.p.Y);
        
        Vector3 kw1 = (c.w - a.w) / (c.p.Y - a.p.Y);
        Vector3 kw2 = (b.w - a.w) / (b.p.Y - a.p.Y);
        Vector3 kw3 = (c.w - b.w) / (c.p.Y - b.p.Y);
        

        int top = int.Max(0, (int)Math.Ceiling(a.p.Y));
        int bottom = int.Min(Bitmap.PixelHeight, (int)Math.Ceiling(c.p.Y));

        for (int y = top; y < bottom; y++)
        {
            Vector3 lp = a.p + (y - a.p.Y) * kp1;
            Vector3 ln = a.n + (y - a.p.Y) * kn1;
            Vector3 lw = a.w + (y - a.p.Y) * kw1;
            Vector3 rp = y < b.p.Y ? a.p + (y - a.p.Y) * kp2 : b.p + (y - b.p.Y) * kp3;
            Vector3 rn = y < b.p.Y ? a.n + (y - a.p.Y) * kn2 : b.n + (y - b.p.Y) * kn3;
            Vector3 rw = y < b.p.Y ? a.w + (y - a.p.Y) * kw2 : b.w + (y - b.p.Y) * kw3;

            if (lp.X > rp.X)
            {
                (lp, rp) = (rp, lp);
                (ln, rn) = (rn, ln);
                (lw, rw) = (rw, lw);
            }
            
            int left = int.Max(0, (int)Math.Ceiling(lp.X));
            int right = int.Min(Bitmap.PixelWidth, (int)Math.Ceiling(rp.X));

            Vector3 kn = (rn - ln) / (rp.X - lp.X);
            Vector3 kw = (rw - lw) / (rp.X - lp.X);
            Vector3 kp = (rp - lp) / (rp.X - lp.X);

            for (int x = left; x < right; x++)
            {
                Vector3 normal = ln + (x - lp.X) * kn;
                Vector3 worldPoint = lw + (x - lp.X) * kw;
                Vector3 point = lp + (x - lp.X) * kp;
                
                Color color = Lighting.GetPointColor(normal, modelColor, worldPoint);
                DrawPixel(x, y, point.Z, color);
            }
        }
    }

    protected virtual void DrawPixel(int x, int y, float z, Color color)
    {
        if (z > 0 && z < 1 && z < ZBuffer[x, y])
        {
            ZBuffer[x, y] = z;
            Bitmap[x, y] = color;
        }
    }
    
    
}
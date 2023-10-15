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

    public FlatShading(Bgra32Bitmap bitmap, ILighting lighting, ObjModel model, Color? color = null) : base(bitmap, model)
    {
        Lighting = lighting;
        ZBuffer = new ZBuffer(Bitmap.PixelWidth, Bitmap.PixelHeight);
    }

    protected override void DrawFace(List<Vector3> face)
    {
        List<Pixel> sidePixels = new List<Pixel>();
        Color color = GetFaceColor(face, Color);
        
        for (int i = 0; i < face.Count - 1; i++)
        {
            DrawSide(face, i, i + 1, color, sidePixels);
        }

        DrawSide(face, 0, face.Count - 1, color, sidePixels);

        DrawPixelsInFace(sidePixels);
    }

    protected override void DrawPixel(int x, int y, float z, Color color, List<Pixel>? sidePixels = null)
    {
        sidePixels?.Add(new Pixel(x, y, z, color));

        if (x > 0 && x < Bitmap.PixelWidth &&
            y > 0 && y < Bitmap.PixelHeight &&
            z > 0 && z < 1 && z <= ZBuffer[x, y])
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

    protected virtual void DrawPixelsInFace(List<Pixel>? sidePixels)
    {
        if (sidePixels is null)
            return;
        
        (int? minY, int? maxY) = GetMinMaxY(sidePixels);
        if (minY is null || maxY is null)
        {
            return;
        }

        Color color = sidePixels[0].Color;
        for (int y = (int)minY; y < maxY; y++) 
        {
            (Pixel? startPixel, Pixel? endPixel) = GetStartEndXForY(sidePixels, y);
            if (startPixel is null || endPixel is null)
            {
                continue;
            }

            Pixel start = (Pixel)startPixel;
            Pixel end = (Pixel)endPixel;

            float z = start.Z;
            float dz = (end.Z - start.Z) / Math.Abs((float)(end.X - start.X));

            
            for (int x = start.X; x < end.X; x++, z += dz)
            {
                if ((x > 0) && (x < ZBuffer.Width) && 
                    (y > 0) && (y < ZBuffer.Height) &&
                    (z <= ZBuffer[x, y]) && z > 0 && z < 1)
                {
                    ZBuffer[x, y] = z;
                    Bitmap[x, y] = color;
                }
            }
        }
    }

    protected static (int? min, int? max) GetMinMaxY(IEnumerable<Pixel> pixels)
    {
        int? minY = null;
        int? maxY = null;

        foreach (var pixel in pixels)
        {
            int y = pixel.Y;

            if (minY == null || y < minY)
            {
                minY = y;
            }

            if (maxY == null || y > maxY)
            {
                maxY = y;
            }
        }

        return (minY, maxY);
    }

    protected static (Pixel? start, Pixel? end) GetStartEndXForY(List<Pixel> pixels, int y)
    {
        Pixel? start = null;
        Pixel? end = null;

        foreach (var pixel in pixels)
        {
            if (pixel.Y == y)
            {
                if (start == null || pixel.X < ((Pixel)start).X)
                {
                    start = pixel;
                }

                if (end == null || pixel.X > ((Pixel)end).X)
                {
                    end = pixel;
                }
            }
        }

        return (start, end);
    }
}
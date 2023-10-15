using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using ObjViewer.Core.Lightings;
using ObjViewer.Model;
using ObjViewer.Model.Types;

namespace ObjViewer.Core.Shaders;

public class FlatShading : Bresenham
{
    protected ZBuffer ZBuffer { get; set; }
    public ILighting Lighting { get; set; }

    public FlatShading(Bgra32Bitmap bitmap, ILighting lighting, Color? color = null) : base(bitmap, color)
    {
        Lighting = lighting;
        ZBuffer = new ZBuffer(_bitmap.PixelWidth, _bitmap.PixelHeight);
    }

    protected override void DrawFace(Face face, Color commonColor, List<Pixel>? sidePixels = null)
    {
        sidePixels = new List<Pixel>();
        Color color = GetFaceColor(face, commonColor);
        
        base.DrawFace(face, color, sidePixels);

        DrawPixelsInFace(sidePixels, Colors.White);
    }

    protected override void DrawPixel(int x, int y, float z, Color color, List<Pixel>? sidePixels = null)
    {
        sidePixels?.Add(new Pixel(x, y, z));

        if (x > 0 && x < _bitmap.PixelWidth &&
            y > 0 && y < _bitmap.PixelHeight &&
            z <= ZBuffer[x, y])
        {
            ZBuffer[x, y] = z;
            _bitmap[x, y] = color;
        }
    }

    protected Color GetFaceColor(Face face, Color color)
    {
        Color[] colors;

        if (face.NormalIndexList.Count > 0)
        {
            colors = face.NormalIndexList
                .Select(index => Lighting.GetPointColor(GetNormalByIndex(index).Coordinates, color)).ToArray();
        }
        else
        {
            var normal = GetFaceNormal(face);
            colors = new[]
            {
                Lighting.GetPointColor(normal, color)
            };
        }

        return GetAverageColor(colors);
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

    protected virtual void DrawPixelsInFace(List<Pixel>? sidePixels, Color color)
    {
        if (sidePixels is null)
            return;

        (int? minY, int? maxY) = GetMinMaxY(sidePixels);
        if (minY is null || maxY is null)
            return;
        
        for (int y = (int)minY; y < maxY; y++) // по очереди отрисовываем линии для каждой y-координаты
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

            // отрисовываем линию
            for (int x = start.X; x < end.X; x++, z += dz)
            {
                if ((x > 0) && (x < ZBuffer.Width) && // x попал в область экрана
                    (y > 0) && (y < ZBuffer.Height) && // y попал в область экрана
                    (z <= ZBuffer[x, y])) // z координата отображаемая
                {
                    ZBuffer[x, y] = z;
                    _bitmap[x, y] = color;
                }
            }
        }
        
        
    }

    protected static (int? min, int? max) GetMinMaxY(IEnumerable<Pixel> pixels)
    {
        var sorted = pixels.OrderBy(p => p.Y).ToList();
        return sorted.Count == 0 ? (min: null, max: null) : (min: (int)sorted.First().Y, max: (int)sorted.Last().Y);
    }

    protected static (Pixel? start, Pixel? end) GetStartEndXForY(List<Pixel> pixels, int y)
    {
        var filtered = pixels.Where(p => p.Y == y).OrderBy(p => p.X).ToList();
        return filtered.Count == 0 ? (start: null, end: null) : (start: filtered.First(), end: filtered.Last());
    }
}
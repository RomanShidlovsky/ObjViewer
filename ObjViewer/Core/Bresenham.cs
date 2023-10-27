using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media;
using ObjViewer.Models;
using Color = System.Windows.Media.Color;

namespace ObjViewer.Core;

public class Bresenham
{
    protected ObjModel Model { get; set; }
    protected Bgra32Bitmap Bitmap { get; set; }
    protected Color Color { get; set; } = Colors.White;

    public Bresenham(Bgra32Bitmap bitmap, ObjModel model)
    {
        Bitmap = bitmap;
        Model = model;
    }
        
    public void DrawModel(Color color)
    {
        Color = color;
        DrawModel();
    }

    protected virtual void DrawModel()
    {
        _ = Parallel.ForEach(Model.TriangleFaces, face =>
        {
            if (IsFaceVisible(face))
            {
                DrawFace(face);
            }
        });
    }

    protected virtual void DrawFace(List<Vector3> face)
    {
        for (int i = 0; i < face.Count - 1; i++)
        {
            DrawSide(face, i, i + 1, Color);
        }

        DrawSide(face, 0, face.Count - 1, Color);
    }

    protected void DrawSide(List<Vector3> face, int index0, int index1, Color color)
    {
        Pixel point0 = GetFacePoint(face, index0, color);
        Pixel point1 = GetFacePoint(face, index1, color);

        DrawLine(point0, point1);
    }

    protected virtual void DrawLine(Pixel src, Pixel desc)
    {
        Color color = src.Color;

        // разница координат начальной и конечной точек
        int dx = Math.Abs(desc.X - src.X);
        int dy = Math.Abs(desc.Y - src.Y);
        float dz = Math.Abs(desc.Z - src.Z);

        // учитываем квадрант
        int signX = src.X < desc.X ? 1 : -1;
        int signY = src.Y < desc.Y ? 1 : -1;
        float signZ = src.Z < desc.Z ? 1 : -1;

        // текущий пиксель
        Pixel p = src;

        float curZ = src.Z;  // текущее z
        float deltaZ = dz / dy;  // при изменении y будем менять z

        int err = dx - dy;   // ошибка

        // пока не достигнем конца
        while (p.X != desc.X || p.Y != desc.Y)
        {
            // пиксель внутри окна
            DrawPixel(p.X, p.Y, curZ, color);

            int err2 = err * 2;      // модифицированное значение ошибки

            if (err2 > -dy)
            {
                p.X += signX;        // изменияем x на единицу
                err -= dy;           // корректируем ошибку
            }

            if (err2 < dx)
            {
                p.Y += signY;            // изменяем y на единицу
                curZ += signZ * deltaZ;  // меняем z
                err += dx;               // корректируем ошибку   
            }
        }

        // отрисовывем последний пиксель
        DrawPixel(desc.X, desc.Y, desc.Z, color);
    }

    protected virtual void DrawPixel(int x, int y, float z, Color color)
    {
        if (x > 0 && x < Bitmap.PixelWidth &&
            y > 0 && y < Bitmap.PixelHeight &&
            z is > 0 and < 1)
        {
            Bitmap[x, y] = color;
        }
    }

    protected bool IsFaceVisible(List<Vector3> face)
    {
        Pixel point1 = GetFacePoint(face, 0, Color);
        Pixel point2 = GetFacePoint(face, 1, Color);
        Pixel point3 = GetFacePoint(face, 2, Color);
        
        Vector2 vector1 = new(point2.X - point1.X,
            point2.Y - point1.Y);

        Vector2 vector2 = new(point3.X - point1.X,
            point3.Y - point1.Y);

        float z = vector2.X * vector1.Y - vector2.Y * vector1.X;
        return true; //z > 0;
    }

    /*protected Vector3 GetFaceNormal(List<Vector3> face)
    {
        Pixel point1 = GetFacePoint(face, 0, Color);
        Pixel point2 = GetFacePoint(face, 1, Color);
        Pixel point3 = GetFacePoint(face, 2, Color);

        return GetNormal(point1, point2, point3);
    }
        
    private static bool IsVisible(Pixel point1, Pixel point2, Pixel point3)
    {
        Vector2 vector1 = new(point2.X - point1.X,
            point2.Y - point1.Y);

        Vector2 vector2 = new(point3.X - point1.X,
            point3.Y - point1.Y);

        float z = vector2.X * vector1.Y - vector2.Y * vector1.X;

        return z > 0;
    }*/

    protected virtual Pixel GetFacePoint(List<Vector3> face, int i, Color color)
    {
        int indexPoint = (int)face[i].X;
        Vector4 point = Model.Points[indexPoint];

        return new Pixel((int)point.X, (int)point.Y, point.Z, color);
    }
}
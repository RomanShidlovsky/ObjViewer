using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media;
using ObjViewer.Models;
using Color = System.Windows.Media.Color;

namespace ObjViewer.Core
{
    public class Bresenham
    {
        protected ObjModel Model { get; set; }
        protected Bgra32Bitmap Bitmap { get; set; }
        protected Color Color { get; set; } = Colors.White;

        public Bresenham(Bgra32Bitmap bitmap)
        {
            Bitmap = bitmap;
        }


        public void DrawModel(ObjModel objModel, Color color)
        {
            Model = objModel;
            Color = color;
            DrawModel();
        }

        protected virtual void DrawModel()
        {
            _ = Parallel.ForEach(Model.Faces, face =>
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

        protected void DrawSide(List<Vector3> face, int index0, int index1, Color color, List<Pixel>? sidePixels = null)
        {
            Pixel point0 = GetFacePoint(face, index0, color);
            Pixel point1 = GetFacePoint(face, index1, color);

            DrawLine(point0, point1, sidePixels);
        }

        protected void DrawLine(Pixel src, Pixel dest, List<Pixel>? sidePixels = null)
        {
            Color color = src.Color;

            int dx = Math.Abs(dest.X - src.X);
            int dy = -Math.Abs(dest.Y - src.Y);
            float dz = Math.Abs(dest.Z - src.Z);

            int signX = src.X < dest.X ? 1 : -1;
            int signY = src.Y < dest.Y ? 1 : -1;
            float signZ = src.Z < dest.Z ? 1 : -1;

            Pixel p = src;
            
            float curZ = src.Z;
            float deltaZ = dz / dy;

            int err = dx + dy;

            while (p.X != dest.X || p.Y != dest.Y)
            {
                DrawPixel(p.X, p.Y, curZ, color, sidePixels);

                int err2 = err * 2;

                if (err2 > dy)
                {
                    p.X += signX;
                    err += dy;
                }

                if (err2 < dx)
                {
                    p.Y += signY;
                    curZ += signZ * deltaZ;
                    err += dx;
                }
            }

            DrawPixel(dest.X, dest.Y, dest.Z, color, sidePixels);
        }

        protected virtual void DrawPixel(int x, int y, float z, Color color, List<Pixel>? sidePixels = null)
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
            return GetFaceNormal(face).Z < 0;
        }

        protected Vector3 GetFaceNormal(List<Vector3> face)
        {
            Pixel point1 = GetFacePoint(face, 0, Color);
            Pixel point2 = GetFacePoint(face, 1, Color);
            Pixel point3 = GetFacePoint(face, 2, Color);

            return GetNormal(point1, point2, point3);
        }
        
        private static Vector3 GetNormal(Pixel point1, Pixel point2, Pixel point3)
        {
            Vector3 vector1 = new(point2.X - point1.X,
                point2.Y - point1.Y,
                point2.Z - point1.Z);

            Vector3 vector2 = new(point3.X - point1.X,
                point3.Y - point1.Y,
                point3.Z - point1.Z);

            Vector3 cross = Vector3.Cross(vector1, vector2);

            return Vector3.Normalize(cross);
        }

        protected virtual Pixel GetFacePoint(List<Vector3> face, int i, Color color)
        {
            int indexPoint = (int)face[i].X;
            Vector4 point = Model.Points[indexPoint];

            return new Pixel((int)point.X, (int)point.Y, point.Z, color);
        }
    }
}
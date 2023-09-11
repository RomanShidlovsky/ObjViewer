using ObjViewer.Model;
using ObjViewer.Model.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ObjViewer.Core
{
    public class Bresenham
    {
        private ObjModel _model;
        public System.Windows.Media.Color Color { get; set; } = System.Windows.Media.Color.FromRgb(255, 255, 255);
        private Bgra32Bitmap _bitmap;

        public Bresenham(Bgra32Bitmap bitmap, System.Windows.Media.Color? color = null)
        {
            _bitmap = bitmap;

            if (color != null)
            {
                Color = (System.Windows.Media.Color)color;
            }
        }


        public void DrawModel(ObjModel objModel)
        {
            _model = objModel;
            DrawModel();
        }

        private void DrawModel()
        {
            _ = Parallel.ForEach(_model!.FaceList, face =>
            {
                if (IsFaceVisible(face))
                {
                    DrawFace(face);
                }
            });
        }

        private void DrawFace(Face face)
        {
            for (int i = 0; i < face.VertexIndexList.Count - 1; i++)
            {
                DrawSide(face, i, i + 1);
            }

            DrawSide(face, 0, face.VertexIndexList.Count - 1);
        }

        private void DrawSide(Face face, int index0, int index1)
        {
            Vector4 point0 = GetVertexByIndex(face.VertexIndexList[index0]).Coordinates;
            Vector4 point1 = GetVertexByIndex(face.VertexIndexList[index1]).Coordinates;

            DrawLine(point0, point1);
        }

        /*private void DrawLine(Vector4 point0, Vector4 point1)
        {
            float dx = Math.Abs(point1.X - point0.X);
            float dy = Math.Abs(point1.Y - point0.Y);

            int steps = dx > dy ? (int)Math.Round(dx) : (int)Math.Round(dy);
            
            float xInc = dx / steps;
            float yInc = dy / steps;

            float x = point0.X;
            float y = point0.Y;
            for (int i = 0; i < steps; i++)
            {
                
                _bitmap[(int)Math.Round(x), (int)Math.Round(y)] = Color;

                x += xInc;
                y += yInc;
            }
        }*/

        private void DrawLine(Vector4 src, Vector4 dest)
        {
            Point srcPoint = new Point((int)Math.Round(src.X), (int)Math.Round(src.Y));
            Point destPoint = new Point((int)Math.Round(dest.X), (int)Math.Round(dest.Y));

            int dx = Math.Abs(destPoint.X - srcPoint.X);
            int dy = - Math.Abs(destPoint.Y - srcPoint.Y);
            
            int signX = srcPoint.X < destPoint.X ? 1 : -1;
            int signY = srcPoint.Y < destPoint.Y ? 1 : -1;

            int err = dx + dy;

            Point p = srcPoint;

            while (p.X != destPoint.X || p.Y != destPoint.Y)
            {
                _bitmap[p.X, p.Y] = Color;

                int err2 = err * 2;     

                if (err2 > dy)
                {
                    p.X += signX;        
                    err += dy;           
                }

                if (err2 < dx)
                {
                    p.Y += signY;            
                    err += dx;                 
                }
            }

            _bitmap[destPoint.X, destPoint.Y] = Color;
        }

        private bool IsFaceVisible(Face face)
        {
            return GetFaceNormal(face).Z < 0;
        }

        private Vector3 GetFaceNormal(Face face)
        {
            Vertex vertex0 = GetVertexByIndex(face.VertexIndexList[0]);
            Vertex vertex1 = GetVertexByIndex(face.VertexIndexList[1]);
            Vertex vertex2 = GetVertexByIndex(face.VertexIndexList[2]);

            Vector3 vector0 = Vector4ToVector3(vertex1.Coordinates) - Vector4ToVector3(vertex0.Coordinates);
            Vector3 vector1 = Vector4ToVector3(vertex2.Coordinates) - Vector4ToVector3(vertex0.Coordinates);

            Vector3 cross = Vector3.Cross(vector0, vector1);

            return Vector3.Normalize(cross);
        }
        
        private Vertex GetVertexByIndex(int index)
        {
            return _model.VertexList[index];
        }

        private Vector3 Vector4ToVector3(Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}

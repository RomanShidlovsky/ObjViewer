using ObjViewer.Model.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjViewer.Model
{
    public class ObjModel : ICloneable
    {
        public List<Vertex> VertexList;
        public List<TextureVertex> TextureVertexList;
        public List<NormalVertex> NormalVertexList;
        public List<Face> FaceList;
        public Extent Size { get; set; }

        public ObjModel()
        {
            VertexList = new List<Vertex>();
            TextureVertexList = new List<TextureVertex>();
            NormalVertexList = new List<NormalVertex>();
            FaceList = new List<Face>();
            Size = new Extent();
        }

        public void LoadObjFromFile(string filePath)
        {
            LoadObj(File.ReadAllLines(filePath));
        }

        public void LoadObjFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                LoadObj(reader.ReadToEnd().Split(Environment.NewLine));
            }
        }

        public void LoadObj(IEnumerable<string> data)
        {
            foreach (var line in data)
            {
                ParseLine(line);
            }

            UpdateSize();
        }

        private void ParseLine(string line)
        {
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "v":
                        Vertex v = new Vertex();
                        v.LoadFromStringArray(parts);
                        VertexList.Add(v);
                        v.Index = VertexList.Count - 1;
                        break;
                    case "vt":
                        TextureVertex vt = new TextureVertex();
                        vt.LoadFromStringArray(parts);
                        TextureVertexList.Add(vt);
                        vt.Index = TextureVertexList.Count - 1;
                        break;
                    case "vn":
                        NormalVertex vn = new NormalVertex();
                        vn.LoadFromStringArray(parts);
                        NormalVertexList.Add(vn);
                        vn.Index = NormalVertexList.Count - 1;
                        break;
                    case "f":
                        Face f = new Face();
                        f.LoadFromStringArray(parts);
                        FaceList.Add(f);
                        break;
                }
            }
        }

        public void UpdateSize()
        {
            if (VertexList.Count== 0)
            {
                Size = new Extent
                {
                    XMax = 0,
                    XMin = 0,
                    YMax = 0,
                    YMin = 0,
                    ZMax = 0,
                    ZMin = 0
                };
                return;
            }

            Size = new Extent
            {
                XMax = VertexList.Max(v => v.Coordinates.X),
                XMin = VertexList.Min(v => v.Coordinates.X),
                YMax = VertexList.Max(v => v.Coordinates.Y),
                YMin = VertexList.Min(v => v.Coordinates.Y),
                ZMax = VertexList.Max(v => v.Coordinates.Z),
                ZMin = VertexList.Min(v => v.Coordinates.Z)
            };
        }

        public bool IsPointInObjectRect(double x, double y)
        {
            return (x > Size.XMin && x < Size.XMax && 
                y > Size.YMin && y < Size.YMax);
        }

        public object Clone()
        {
            ObjModel clonedObjModel = new ObjModel();

            // Copy the vertex data
            clonedObjModel.VertexList.AddRange(this.VertexList.Select(v => (Vertex)v.Clone()));

            // Copy the texture vertex data
            clonedObjModel.TextureVertexList.AddRange(this.TextureVertexList.Select(vt => (TextureVertex)vt.Clone()));

            // Copy the normal vertex data
            clonedObjModel.NormalVertexList.AddRange(this.NormalVertexList.Select(vn => (NormalVertex)vn.Clone()));

            // Copy the face data
            clonedObjModel.FaceList.AddRange(this.FaceList.Select(f => (Face)f.Clone()));

            // Copy the size extent
            clonedObjModel.Size = new Extent
            {
                XMax = this.Size.XMax,
                XMin = this.Size.XMin,
                YMax = this.Size.YMax,
                YMin = this.Size.YMin,
                ZMax = this.Size.ZMax,
                ZMin = this.Size.ZMin
            };

            return clonedObjModel;
        }
    }
}

using ObjViewer.Model.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjViewer.Model
{
    public class ObjModel
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
                        v.Index = VertexList.Count;
                        break;
                    case "vt":
                        TextureVertex vt = new TextureVertex();
                        vt.LoadFromStringArray(parts);
                        TextureVertexList.Add(vt);
                        vt.Index = TextureVertexList.Count;
                        break;
                    case "vn":
                        NormalVertex vn = new NormalVertex();
                        vn.LoadFromStringArray(parts);
                        NormalVertexList.Add(vn);
                        vn.Index = NormalVertexList.Count;
                        break;
                    case "f":
                        Face f = new Face();
                        f.LoadFromStringArray(parts);
                        FaceList.Add(f);
                        break;
                }
            }
        }

        private void UpdateSize()
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
                XMax = VertexList.Max(v => v.X),
                XMin = VertexList.Min(v => v.X),
                YMax = VertexList.Max(v => v.Y),
                YMin = VertexList.Min(v => v.Y),
                ZMax = VertexList.Max(v => v.Z),
                ZMin = VertexList.Min(v => v.Z)
            };
        }
    }
}

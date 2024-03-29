﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ObjViewer.Model.Types
{
    public class Face : Type, ICloneable
    {
        public List<int> VertexIndexList { get; set; }
        public List<int> TextureIndexList { get; set; }
        public List<int> NormalIndexList { get; set; }

        public Face()
        {
            MinDataLength = 4;
            Prefix = "f";
            VertexIndexList = new List<int>();
            TextureIndexList = new List<int>();
            NormalIndexList = new List<int>();
        }

        protected override void ParseAndSetValues(string[] data)
        {
            int vcount = data.Length - 1;

            for (int i = 0; i < vcount; i++)
            {
                string[] parts = data[i+1].Split('/');

                if (!int.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var index))
                {
                    throw new ArgumentException("Could not parse vertex index as int");
                }

                VertexIndexList.Add(index - 1);

                if (parts.Length > 1 && int.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out index))
                {
                    TextureIndexList.Add(index - 1);
                }

                if (parts.Length > 2 && int.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out index))
                {
                    NormalIndexList.Add(index - 1);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append('f');

            for (int i = 0; i < VertexIndexList.Count; i++)
            {
                if (i < TextureIndexList.Count && i < NormalIndexList.Count)
                {
                    b.Append($" {VertexIndexList[i]}/{TextureIndexList[i]}/{NormalIndexList[i]}");
                }
                else if (i < TextureIndexList.Count)
                {
                    b.Append($" {VertexIndexList[i]}/{TextureIndexList[i]}");
                }
                else
                {
                    b.Append($" {VertexIndexList[i]}");
                }
            }

            return b.ToString();
        }

        public object Clone()
        {
            return new Face
            {
                VertexIndexList = new List<int>(VertexIndexList),
                NormalIndexList = new List<int>(NormalIndexList),
                TextureIndexList = new List<int>(TextureIndexList)
            };
        }
    }
}

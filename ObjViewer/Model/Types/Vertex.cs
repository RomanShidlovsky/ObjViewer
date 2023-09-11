using System;
using System.Globalization;
using System.Numerics;

namespace ObjViewer.Model.Types
{
    public class Vertex : Type, ICloneable
    {

        public Vector4 Coordinates { get; set; }
        public int Index { get; set; }

        public Vertex()
        {
            MinDataLength = 4;
            Prefix = "v";
        }

        protected override void ParseAndSetValues(string[] data)
        {
            if (!TryParseFloat(data[1], out float x) ||
                !TryParseFloat(data[2], out float y) ||
                !TryParseFloat(data[3], out float z))
            {
                throw new ArgumentException("Could not parse X, Y, or Z parameter as double", nameof(data));
            }

            Vector4 vector = new Vector4 { X = x, Y = y, Z = z, W = 1 };

            if (data.Length > 4 && float.TryParse(data[4], NumberStyles.Any, CultureInfo.InvariantCulture, out float w))
            {
                vector.W = w;
            }

            Coordinates = vector;
        }

        public object Clone()
        {
            return new Vertex()
            {
                Coordinates = this.Coordinates,
                Index = this.Index
            };
        }
    }
}

using System;
using System.Globalization;
using System.Numerics;

namespace ObjViewer.Parser.Types
{
    public class Vertex : Type
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
            if (!float.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float x) ||
                !float.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out float y) ||
                !float.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out float z))
            {
                throw new ArgumentException("Could not parse X, Y, or Z parameter as double", nameof(data));
            }

            if (data.Length > 4 && float.TryParse(data[4], NumberStyles.Any, CultureInfo.InvariantCulture, out float w))
            {
                Coordinates = new Vector4(x, y, z, w);
            }
            else
            {
                Coordinates = new Vector4(x, y, z, 1);
            }
        }

        public override string ToString()
        {
            return Coordinates.W == 1.0 ? $"v {Coordinates.X} {CY} {Z}" : $"v {X} {Y} {Z} {W}";
        }
    }
}

using System;
using System.Globalization;

namespace ObjViewer.Parser.Types
{
    public class Vertex : Type
    {

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; } = 1.0;
        public int Index { get; set; }

        public Vertex()
        {
            MinDataLength = 4;
            Prefix = "v";
        }

        protected override void ParseAndSetValues(string[] data)
        {
            if (!double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) ||
                !double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double y) ||
                !double.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double z))
            {
                throw new ArgumentException("Could not parse X, Y, or Z parameter as double", nameof(data));
            }

            X = x;
            Y = y;
            Z = z;

            if (data.Length > 4 && double.TryParse(data[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double w))
            {
                W = w;
            }
        }

        public override string ToString()
        {
            return W == 1.0 ? $"v {X} {Y} {Z}" : $"v {X} {Y} {Z} {W}";
        }
    }
}

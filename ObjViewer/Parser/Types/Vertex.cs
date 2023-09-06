using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjViewer.Parser.Types
{
    public class Vertex : IType
    {
        public const int MinDataLength = 4;
        public const string Prefix = "v";

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; } = 1.0;

        public void LoadFromStringArray(string[] data)
        {
            ValidateInputData(data);

            ParseAndSetValues(data);
        }

        private void ValidateInputData(string[] data)
        {
            if (data.Length < MinDataLength)
            {
                throw new ArgumentException($"Input array must have a minimum length of {MinDataLength}");
            }

            if (!data[0].Equals(Prefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Data prefix must be equal to {Prefix}", nameof(data));
            }
        }

        private void ParseAndSetValues(string[] data)
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

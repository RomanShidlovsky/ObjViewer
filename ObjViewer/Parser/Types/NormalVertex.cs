using System;
using System.Globalization;

namespace ObjViewer.Parser.Types
{
    public class NormalVertex : Type
    {
        public double I { get; set; }
        public double J { get; set; }
        public double K { get; set; }
        public int Index { get; set; }

        public NormalVertex()
        {
            MinDataLength = 4;
            Prefix = "vn";
        }

        protected override void ParseAndSetValues(string[] data)
        {
            if (!double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double i) ||
                !double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double j) ||
                !double.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double k))
            {
                throw new ArgumentException("Could not parse X, Y, or Z parameter as double", nameof(data));
            }

            I = i;
            J = j;
            K = k;
        }

        public override string ToString()
        {
            return $"vn {I} {J} {K}";
        }
    }
}

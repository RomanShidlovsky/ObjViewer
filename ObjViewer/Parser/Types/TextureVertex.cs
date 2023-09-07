using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjViewer.Parser.Types
{
    public class TextureVertex : Type
    {
        public double U { get; set; }
        public double V { get; set; } = 0;
        public double W { get; set; } = 0;
        public int Index { get; set; }

        public TextureVertex()
        {
            MinDataLength = 2;
            Prefix = "vt";
        }

        protected override void ParseAndSetValues(string[] data)
        {
            if (!double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double u))
            {
                throw new ArgumentException("Could not parse U parameter as double", nameof(data));
            }

            U = u;

            if (data.Length > 2 && double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
            {
                V = v;
            }

            if (data.Length > 3 && double.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double w))
            {
                W = w;
            }
        }

        public override string ToString()
        {
            return $"vt {U} {V} {W}";
        }
    }
}

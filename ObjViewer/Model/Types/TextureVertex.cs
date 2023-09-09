using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ObjViewer.Model.Types
{
    public class TextureVertex : Type
    {
        public Vector3 Coordinates { get; set; }
        public int Index { get; set; }

        public TextureVertex()
        {
            MinDataLength = 2;
            Prefix = "vt";
        }

        protected override void ParseAndSetValues(string[] data)
        {
            if (!TryParseFloat(data[1], out float x))
            {
                throw new ArgumentException("Could not parse first coordinate of parameter as double", nameof(data));
            }

            Vector3 vector = new Vector3 { X = x, Y = 0, Z = 0 };

            if (data.Length > 2 && TryParseFloat(data[2], out float y))
            {
                vector.Y = y;

                if (data.Length > 3 && TryParseFloat(data[3], out float z))
                {
                    vector.Z = z;
                }
            }

            Coordinates = vector;
        }
    }
}

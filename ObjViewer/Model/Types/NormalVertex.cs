using System;
using System.Globalization;
using System.Numerics;

namespace ObjViewer.Model.Types
{
    public class NormalVertex : Type
    {
        public Vector3 Coordinates { get; set; }
        public int Index { get; set; }

        public NormalVertex()
        {
            MinDataLength = 4;
            Prefix = "vn";
        }

        protected override void ParseAndSetValues(string[] data)
        {
            if (!TryParseFloat(data[1], out float x) ||
                !TryParseFloat(data[2], out float y) ||
                !TryParseFloat(data[3], out float z))
            {
                throw new ArgumentException("Could not parse X, Y, or Z parameter as double", nameof(data));
            }

            Coordinates = new Vector3(x, y, z);
        }
    }
}

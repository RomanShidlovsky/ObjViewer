using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjViewer.Parser.Types
{
    public abstract class Type
    {
        public int MinDataLength { get; protected set; }
        public string Prefix { get; protected set; }

        protected virtual void ValidateInputData(string[] data)
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

        protected abstract void ParseAndSetValues(string[] data);

        public void LoadFromStringArray(string[] data)
        {
            ValidateInputData(data);

            ParseAndSetValues(data);
        }

        protected bool TryParseFloat(string value, out float result)
        {
            return float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
    }
}

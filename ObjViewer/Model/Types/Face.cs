using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjViewer.Parser.Types
{
    public class Face : Type
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
                int index;

                if (!int.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out index))
                {
                    throw new ArgumentException("Could not parse vertex index as int");
                }

                VertexIndexList.Add(index);

                if (parts.Length > 1 && int.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out index))
                {
                    TextureIndexList.Add(index);
                }

                if (parts.Length > 2 && int.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out index))
                {
                    NormalIndexList.Add(index);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("f");

            for (int i = 0; i < VertexIndexList.Count(); i++)
            {
                if (i < TextureIndexList.Count && i < NormalIndexList.Count)
                {
                    b.AppendFormat(" {0}/{1}/{2}", VertexIndexList[i], TextureIndexList[i], NormalIndexList[i]);
                }
                else if (i < TextureIndexList.Count)
                {
                    b.AppendFormat(" {0}/{1}", VertexIndexList[i], TextureIndexList[i]);
                }
                else
                {
                    b.AppendFormat(" {0}", VertexIndexList[i]);
                }
            }

            return b.ToString();
        }

    }
}

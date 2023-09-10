namespace ObjViewer.Model
{
    public class Extent
    {
        public float XMax { get; set; }
        public float XMin { get; set; }
        public float YMax { get; set; }
        public float YMin { get; set; }
        public float ZMax { get; set; }
        public float ZMin { get; set; }

        public float XSize { get { return XMax - XMin; } }
        public float YSize { get { return YMax - YMin; } }
        public float ZSize { get { return ZMax - ZMin; } }

        public float XCenter { get { return (XMin + XMax) / 2; } }
        public float YCenter { get { return (YMin + YMax) / 2; } }
        public float ZCenter { get { return (ZMin + ZMax) / 2; } }
    }
}

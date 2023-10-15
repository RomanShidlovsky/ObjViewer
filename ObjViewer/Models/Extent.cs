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

        public float XSize => XMax - XMin;
        public float YSize => YMax - YMin;
        public float ZSize => ZMax - ZMin;

        public float XCenter => (XMin + XMax) / 2;
        public float YCenter => (YMin + YMax) / 2;
        public float ZCenter => (ZMin + ZMax) / 2;
    }
}

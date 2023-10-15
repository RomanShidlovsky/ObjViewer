using System.Windows.Media;
using ObjViewer.Model;

namespace ObjViewer.Core.Shaders;

public class FlatShading : Bresenham
{
    
    
    public FlatShading(Bgra32Bitmap bitmap, Color? color = null) : base(bitmap, color)
    {
        
    }
}
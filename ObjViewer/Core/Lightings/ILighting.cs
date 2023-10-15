using System.Numerics;
using System.Windows.Media;

namespace ObjViewer.Core.Lightings;

public interface ILighting
{
    Color GetPointColor(Vector3 normalVector, Color color);
}
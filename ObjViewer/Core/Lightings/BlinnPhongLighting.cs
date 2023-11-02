using System;
using System.Numerics;
using System.Windows.Media;

namespace ObjViewer.Core.Lightings;

public class BlinnPhongLighting : ILighting
{
    private Vector3 lightPos;
    private Vector3 viewPos;
    private float ambientIntensity;
    private float diffuseIntensity;
    private float specularIntensity;
    private float shinessIntensity;
    private Vector3 ambientColor;
    private Vector3 specularColor;

    public BlinnPhongLighting(Vector3 lightPos, Vector3 viewPos, float ambientIntensity, float diffuseIntensity,
        float specularIntensity, float shinessIntensity, Vector3 ambientColor,
        Vector3 specularColor)
    {
        this.lightPos = lightPos;
        this.viewPos = viewPos;
        this.ambientIntensity = ambientIntensity;
        this.diffuseIntensity = diffuseIntensity;
        this.specularIntensity = specularIntensity;
        this.shinessIntensity = shinessIntensity;
        this.ambientColor = ambientColor;
        this.specularColor = specularColor;
    }


    public Color GetPointColor(Vector3 normalVector, Color color, Vector3? point = null)
    {
        if (point is null)
            return color;
        
        Vector3 lightDir = Vector3.Normalize(lightPos - point.Value);
        Vector3 viewDir = Vector3.Normalize(viewPos - point.Value);
        Vector3 halfwayDir = Vector3.Normalize(lightDir + viewDir);
        
        Vector3 iAmbient = ambientIntensity * ambientColor;
        Vector3 iDiffuse = diffuseIntensity * new Vector3(color.R, color.G, color.B) / 255f * Math.Max(Vector3.Dot(normalVector, lightDir), 0);
        float spec = Math.Max(Vector3.Dot(normalVector, halfwayDir), 0);
        Vector3 iSpecular = (float)(specularIntensity * Math.Pow(spec, shinessIntensity)) * specularColor;
        
        Vector3 finalColor = Vector3.Clamp(iAmbient + iDiffuse + iSpecular, Vector3.Zero, Vector3.One) * 255f;
        
        return Color.FromRgb((byte)finalColor.X, (byte)finalColor.Y, (byte)finalColor.Z);
    }
}
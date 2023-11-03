﻿using System;
using System.Numerics;
using System.Windows.Media;

namespace ObjViewer.Core.Lightings;

public class BlinnPhongLighting : ILighting
{
    private Vector3 _lightPos;
    private Vector3 _viewPos;
    private float _ambientIntensity;
    private float _diffuseIntensity;
    private float _specularIntensity;
    private float _shinessIntensity;
    private float _lightIntensity;
    private Vector3 _ambientColor;
    private Vector3 _specularColor;
    private Vector3 _lightColor;

    public BlinnPhongLighting(Vector3 lightPos, Vector3 viewPos, float ambientIntensity, float diffuseIntensity,
        float specularIntensity, float shinessIntensity, Vector3 ambientColor,
        Vector3 specularColor, float lightIntensity, Vector3 lightColor)
    {
        _lightPos = lightPos;
        _viewPos = viewPos;
        _ambientIntensity = ambientIntensity;
        _diffuseIntensity = diffuseIntensity;
        _specularIntensity = specularIntensity;
        _shinessIntensity = shinessIntensity;
        _ambientColor = ambientColor;
        _specularColor = specularColor;
        _lightIntensity = lightIntensity;
        _lightColor = lightColor;
    }

    public void SetParams(Vector3 lightPos, Vector3 viewPos, float ambientIntensity, float diffuseIntensity,
        float specularIntensity, float shinessIntensity, Vector3 ambientColor,
        Vector3 specularColor,float lightIntensity, Vector3 lightColor)
    {
        _lightPos = lightPos;
        _viewPos = viewPos;
        _ambientIntensity = ambientIntensity;
        _diffuseIntensity = diffuseIntensity;
        _specularIntensity = specularIntensity;
        _shinessIntensity = shinessIntensity;
        _ambientColor = ambientColor;
        _specularColor = specularColor;
        _lightColor = lightColor;
        _lightIntensity = lightIntensity;
    }
    
    public Color GetPointColor(Vector3 normalVector, Color color, Vector3? point = null)
    {
        if (point is null)
            return color;
        
        normalVector = Vector3.Normalize(normalVector);
        float lightLength = (_lightPos - point.Value).Length();
        Vector3 lightDir = Vector3.Normalize(_lightPos - point.Value);
        Vector3 viewDir = Vector3.Normalize(_viewPos - point.Value);
        Vector3 halfwayDir = Vector3.Normalize(lightDir + viewDir);

        Vector3 irradiance = _lightIntensity * _lightColor * Vector3.Dot(normalVector, lightDir) / (lightLength * lightLength);//инт света * цвет света NL / растояние ^ 2; 
        Vector3 iAmbient = _ambientIntensity * _ambientColor;
        Vector3 iDiffuse = _diffuseIntensity * new Vector3(color.R, color.G, color.B) / 255f;
        float spec = Math.Max(Vector3.Dot(normalVector, halfwayDir), 0);
        Vector3 iSpecular = (float)(_specularIntensity * Math.Pow(spec, _shinessIntensity)) * _specularColor;
        
        Vector3 finalColor = Vector3.Clamp(iAmbient + (iDiffuse + iSpecular) * irradiance, Vector3.Zero, Vector3.One) * 255f;
        
        return Color.FromRgb((byte)finalColor.X, (byte)finalColor.Y, (byte)finalColor.Z);
    }
}
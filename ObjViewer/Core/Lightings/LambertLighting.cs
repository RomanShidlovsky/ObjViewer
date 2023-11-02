﻿using System;
using System.Numerics;
using System.Windows.Media;
using ObjViewer.Models;

namespace ObjViewer.Core.Lightings;

public class LambertLighting : ILighting
{
    private Vector3 _lightVector;

    public LambertLighting(Vector3 lightVector)
    {
        _lightVector = lightVector;
    }

    public void SetLightVector(Vector3 lightVector)
    {
        _lightVector = lightVector;
    }

    public Color GetPointColor(Vector3 normalVector, Color color, Vector3? point = null)
    {
        double k = Math.Max(Vector3.Dot(normalVector, Vector3.Normalize(_lightVector)), 0);

        byte r = (byte)Math.Round(color.R * k);
        byte g = (byte)Math.Round(color.G * k);
        byte b = (byte)Math.Round(color.B * k);

        return Color.FromArgb(255, r, g, b);
    }
}
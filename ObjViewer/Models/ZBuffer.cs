using System;

namespace ObjViewer.Models;

public class ZBuffer
{
    private readonly double[,] _buffer;
    public int Width { get; set; }
    public int Height { get; set; }

    public double this[int x, int y]
    {
        get => IsValidCoordinate(x, y) ? _buffer[x, y] : throw new ArgumentOutOfRangeException(nameof(x));
        set => _buffer[x, y] = IsValidCoordinate(x, y) ? value : throw new ArgumentOutOfRangeException(nameof(y));
    }

    public ZBuffer(int width, int height)
    {
        Width = width;
        Height = height;
        _buffer = new double[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                _buffer[i, j] = double.MaxValue;
            }
        }
    }

    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x <= Width && y >= 0 && y <= Height;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ObjViewer.Models;

public class Bgra32Bitmap
{
    public WriteableBitmap Source { get; private set; }
    public int PixelWidth { get; private set; }
    public int PixelHeight { get; private set; }
    private readonly nint _backBuffer;
    private readonly int _backBufferStride;
    private readonly int _bytesPerPixel;

    public Bgra32Bitmap(WriteableBitmap source)
    {
        FormatConvertedBitmap formatConvertedBitmap =
            new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
        Source = new WriteableBitmap(formatConvertedBitmap);

        _backBuffer = Source.BackBuffer;
        _backBufferStride = Source.BackBufferStride;
        _bytesPerPixel = Source.Format.BitsPerPixel / 8;
        PixelWidth = Source.PixelWidth;
        PixelHeight = Source.PixelHeight;
    }

    private unsafe byte* GetPixelAddress(int x, int y)
    {
        return (byte*)(_backBuffer + (y * _backBufferStride) + (x * _bytesPerPixel));
    }

    public unsafe Color this[int x, int y]
    {
        get
        {
            byte* address = GetPixelAddress(x, y);
            return Color.FromArgb(address[3], address[2], address[1], address[0]);
        }
        set
        {
            if (x < PixelWidth && y < PixelHeight && x >= 0 && y >= 0)
            {
                byte* address = GetPixelAddress(x, y);
                address[0] = Convert.ToByte(value.B);
                address[1] = Convert.ToByte(value.G);
                address[2] = Convert.ToByte(value.R);
                address[3] = Convert.ToByte(value.A);
            }
        }
    }

    public void Clear(Color clearColor)
    {
        byte[] pixels = new byte[_backBufferStride * PixelWidth];

        for (int y = 0; y < PixelHeight; y++)
        {
            for (int x = 0; x < PixelWidth; x++)
            {
                int index = y * _backBufferStride + 4 * x;
                pixels[index + 0] = clearColor.B;
                pixels[index + 1] = clearColor.G;
                pixels[index + 2] = clearColor.R;
                pixels[index + 3] = clearColor.A;
            }
        }

        Source.WritePixels(new Int32Rect(0, 0, PixelWidth, Source.PixelHeight), pixels, _backBufferStride, 0);
    }
}
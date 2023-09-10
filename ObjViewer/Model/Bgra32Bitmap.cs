﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ObjViewer.Model
{
    public class Bgra32Bitmap
    {
        public WriteableBitmap Source { get; private set; }

        private nint _backBuffer;
        private int _backBufferStride;
        private int _bytesPerPixel;
        private int _pixelWidth;
        private int _pixelHeight;

        public Bgra32Bitmap(WriteableBitmap source)
        { 
            FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
            Source = new WriteableBitmap(formatConvertedBitmap);

            _backBuffer = Source.BackBuffer;
            _backBufferStride = Source.BackBufferStride;
            _bytesPerPixel = Source.Format.BitsPerPixel / 8;
            _pixelWidth = Source.PixelWidth;
            _pixelHeight = Source.PixelHeight;
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
                if (x < _pixelWidth && y < _pixelHeight && x >= 0 && y >= 0)
                {
                    byte* address = GetPixelAddress(x, y);
                    address[0] = Convert.ToByte(value.B);
                    address[1] = Convert.ToByte(value.G);
                    address[2] = Convert.ToByte(value.R);
                    address[3] = Convert.ToByte(value.A);
                }
            }
        }
    }
}
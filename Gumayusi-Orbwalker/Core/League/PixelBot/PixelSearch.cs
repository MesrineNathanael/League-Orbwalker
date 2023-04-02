using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Gumayusi_Orbwalker.Core.League.PixelBot
{
    public class PixelSearch
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        public static Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                (int)(pixel & 0x0000FF00) >> 8,
                (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }

        private readonly Bitmap _bmp = new(1, 1);

        public Color GetColorAt(int x, int y)
        {
            var bounds = new Rectangle(x, y, 1, 1);
            using (Graphics g = Graphics.FromImage(_bmp))
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            return _bmp.GetPixel(0, 0);
        }

        private int _monitor;

        public Point[] Search(Rectangle rect, Color pixelColor, int shadeVariation)
        {
            var points = new ArrayList();
            var regionInBitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);

            if (_monitor >= Screen.AllScreens.Length)
            {
                _monitor = 0;
            }

            var xOffset = Screen.AllScreens[_monitor].Bounds.Left;
            var yOffset = Screen.AllScreens[_monitor].Bounds.Top;

            using (var gfx = Graphics.FromImage(regionInBitmap))
            {
                gfx.CopyFromScreen(rect.X + xOffset, rect.Y + yOffset, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }

            var regionInBitmapData = regionInBitmap.LockBits(
                new Rectangle(0, 0, regionInBitmap.Width, regionInBitmap.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            var formattedColor = new int[] { pixelColor.B, pixelColor.G, pixelColor.R }; //bgr

            unsafe
            {
                for (var y = 0; y < regionInBitmapData.Height; y++)
                {
                    var row = (byte*)regionInBitmapData.Scan0 + y * regionInBitmapData.Stride;
                    for (var x = 0; x < regionInBitmapData.Width; x++)
                    {
                        if (row[x * 3] >= formattedColor[0] - shadeVariation &
                            row[x * 3] <= formattedColor[0] + shadeVariation) //blue
                            if (row[x * 3 + 1] >= formattedColor[1] - shadeVariation &
                                row[x * 3 + 1] <= formattedColor[1] + shadeVariation) //green
                                if (row[x * 3 + 2] >= formattedColor[2] - shadeVariation &
                                    row[x * 3 + 2] <= formattedColor[2] + shadeVariation) //red
                                    points.Add(new Point(x + rect.X, y + rect.Y));
                    }
                }
            }

            regionInBitmap.Dispose();
            return (Point[])points.ToArray(typeof(Point));
        }

        public Point[] SearchInSpiral(Rectangle rect, Color pixelColor, int shadeVariation)
{
    var points = new List<Point>();
    var centerX = rect.Width / 2 + rect.Left;
    var centerY = rect.Height / 2 + rect.Top;
    var radius = Math.Max(centerX - rect.Left, centerY - rect.Top);
    var x = centerX;
    var y = centerY;
    var dx = 0;
    var dy = -1;
    var i = 0;

    using (var bitmap = new Bitmap(rect.Width, rect.Height))
    {
        using (var g = Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
        }

        while (i <= radius * radius)
        {
            if (rect.Contains(x, y))
            {
                var pixel = bitmap.GetPixel(x - rect.Left, y - rect.Top);
                if (IsColorMatch(pixel, pixelColor, shadeVariation))
                {
                    points.Add(new Point(x, y));
                }
            }

            if (x == y || x < 0 && x == -y || x > 0 && x == 1 - y)
            {
                var temp = dx;
                dx = -dy;
                dy = temp;
            }

            x += dx;
            y += dy;
            i++;
        }
    }

    return points.ToArray();
}

private bool IsColorMatch(Color color, Color targetColor, int shadeVariation)
{
    return Math.Abs(color.R - targetColor.R) <= shadeVariation
        && Math.Abs(color.G - targetColor.G) <= shadeVariation/2
        && Math.Abs(color.B - targetColor.B) <= shadeVariation/2;
}
    }
}

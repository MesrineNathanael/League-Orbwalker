using System;
using System.Collections;
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

        public Color GetPixelColor(int x, int y)
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

        public Point[] Search_Spiral(Rectangle rect, Color pixelColor, int shadeVariation)
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
                //search in RegionIn_BitmapData in spiral pattern
                var xMax = regionInBitmapData.Width;
                var yMax = regionInBitmapData.Height;
                const int xMin = 0;
                const int yMin = 0;
                var xStep = 1;
                var yStep = 1;
                var xDirection = 1;
                var yDirection = 1;
                var xDistance = xMax - xMin;
                var yDistance = yMax - yMin;
                var xDistanceStep = xDistance;
                var yDistanceStep = yDistance;
                while (xDistance > 0 || yDistance > 0)
                {
                    int x;
                    int y;
                    for (var i = 0; i < xDistance; i++)
                    {
                        x = xMin + xStep * xDirection;
                        y = yMin + yStep * yDirection;
                        var row = (byte*)regionInBitmapData.Scan0 + y * regionInBitmapData.Stride;
                        if (row[x * 3] >= formattedColor[0] - shadeVariation &
                            row[x * 3] <= formattedColor[0] + shadeVariation) //blue
                            if (row[x * 3 + 1] >= formattedColor[1] - shadeVariation &
                                row[x * 3 + 1] <= formattedColor[1] + shadeVariation) //green
                                if (row[x * 3 + 2] >= formattedColor[2] - shadeVariation &
                                    row[x * 3 + 2] <= formattedColor[2] + shadeVariation)
                                    points.Add(new Point(x + rect.X, y + rect.Y));
                        xStep += xDirection;
                        if (xStep <= xMax && xStep >= xMin) continue;
                        
                        xDirection *= -1;
                        xStep += xDirection * 2;
                        xDistanceStep--;
                        if (xDistanceStep != 0) continue;

                        xDistanceStep = xDistance;
                        xDistance -= xDistanceStep;
                    }

                    for (var i = 0; i < yDistance; i++)
                    {
                        x = xMin + xStep * xDirection;
                        y = yMin + yStep * yDirection;
                        var row = (byte*)regionInBitmapData.Scan0 + y * regionInBitmapData.Stride;
                        if (row[x * 3] >= formattedColor[0] - shadeVariation &
                            row[x * 3] <= formattedColor[0] + shadeVariation) //blue
                            if (row[x * 3 + 1] >= formattedColor[1] - shadeVariation &
                                row[x * 3 + 1] <= formattedColor[1] + shadeVariation) //green
                                if (row[x * 3 + 2] >= formattedColor[2] - shadeVariation &
                                    row[x * 3 + 2] <= formattedColor[2] + shadeVariation) //red
                                    points.Add(new Point(x + rect.X, y + rect.Y));
                        yStep += yDirection;
                        if (yStep > yMax || yStep < yMin)
                        {
                            yDirection *= -1;
                            yStep += yDirection * 2;
                            yDistanceStep--;
                            if (yDistanceStep == 0)
                            {
                                yDistanceStep = yDistance;
                                yDistance -= yDistanceStep;
                            }
                        }
                    }
                }
            }

            regionInBitmap.Dispose();
            return (Point[])points.ToArray(typeof(Point));
        }

        //search for a pixel color on screen
        //in a spiral pattern from the center of the screen
        public Point[] Search_Spiral(Color pixelColor, int shadeVariation)
        {
            var points = new ArrayList();
            var rect = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
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
            var formattedColor = new int[3] { pixelColor.B, pixelColor.G, pixelColor.R }; //bgr

            unsafe
            {
                //search in RegionIn_BitmapData in spiral pattern
                var xMax = regionInBitmapData.Width;
                var yMax = regionInBitmapData.Height;
                const int xMin = 0;
                const int yMin = 0;
                var xStep = 1;
                var yStep = 1;
                var xDirection = 1;
                var yDirection = 1;
                var xDistance = xMax - xMin;
                var yDistance = yMax - yMin;
                var xDistanceStep = xDistance;
                var yDistanceStep = yDistance;
                var xDistanceDirection = 1;
                var yDistanceDirection = 1;
                while (xDistance > 0 || yDistance > 0)
                {
                    int x;
                    int y;
                    for (var i = 0; i < xDistance; i++)
                    {
                        x = xMin + xStep * xDirection;
                        y = yMin + yStep * yDirection;
                        var row = (byte*)regionInBitmapData.Scan0 + y * regionInBitmapData.Stride;
                        if (row[x * 3] >= formattedColor[0] - shadeVariation &
                            row[x * 3] <= formattedColor[0] + shadeVariation) //blue
                            if (row[x * 3 + 1] >= formattedColor[1] - shadeVariation &
                                row[x * 3 + 1] <= formattedColor[1] + shadeVariation) //green
                                if (row[x * 3 + 2] >= formattedColor[2] - shadeVariation &
                                    row[x * 3 + 2] <= formattedColor[2] + shadeVariation)
                                    points.Add(new Point(x + rect.X, y + rect.Y));
                        xStep += xDirection;
                        if (xStep <= xMax && xStep >= xMin) continue;
                        
                        xDirection *= -1;
                        xStep += xDirection * 2;
                        xDistanceStep--;
                        if (xDistanceStep != 0) continue;

                        xDistanceStep = xDistance;
                        xDistance -= xDistanceStep;
                        xDistanceDirection *= -1;

                    }

                    for (var i = 0; i < yDistance; i++)
                    {
                        x = xMin + xStep * xDirection;
                        y = yMin + yStep * yDirection;
                        var row = (byte*)regionInBitmapData.Scan0 + y * regionInBitmapData.Stride;
                        if (row[x * 3] >= formattedColor[0] - shadeVariation &
                            row[x * 3] <= formattedColor[0] + shadeVariation) //blue
                            if (row[x * 3 + 1] >= formattedColor[1] - shadeVariation &
                                row[x * 3 + 1] <= formattedColor[1] + shadeVariation) //green
                                if (row[x * 3 + 2] >= formattedColor[2] - shadeVariation &
                                    row[x * 3 + 2] <= formattedColor[2] + shadeVariation) //red
                                    points.Add(new Point(x + rect.X, y + rect.Y));
                        yStep += yDirection;
                        if (yStep <= yMax && yStep >= yMin) continue;
                        
                        yDirection *= -1;
                        yStep += yDirection * 2;
                        yDistanceStep--;
                        if (yDistanceStep != 0) continue;

                        yDistanceStep = yDistance;
                        yDistance -= yDistanceStep;
                        yDistanceDirection *= -1;
                    }
                }
            }

            regionInBitmap.Dispose();
            return (Point[])points.ToArray(typeof(Point));
        }
    }
}

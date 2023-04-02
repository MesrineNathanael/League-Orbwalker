using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
        Bitmap bmp = new Bitmap(1, 1);
        public Color GetColorAt(int x, int y)
        {
            Rectangle bounds = new Rectangle(x, y, 1, 1);
            using (Graphics g = Graphics.FromImage(bmp))
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            return bmp.GetPixel(0, 0);
        }

        private int monitor;
        public unsafe Point[] SearchPixel(Rectangle rect, Color PixelColor, int ShadeVariation)
        {
            ArrayList arrayList = new ArrayList();
            using (var tile = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb))
            {
                if (monitor >= Screen.AllScreens.Length)
                {
                    monitor = 0;
                }
                int left = Screen.AllScreens[monitor].Bounds.Left;
                int top = Screen.AllScreens[monitor].Bounds.Top;
                using (var g = Graphics.FromImage(tile))
                {
                    g.CopyFromScreen(rect.X + left, rect.Y + top, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
                }
                BitmapData bitmapData = tile.LockBits(new Rectangle(0, 0, tile.Width, tile.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int[] array = new int[]
                    {
                     PixelColor.B,
                     PixelColor.G,
                     PixelColor.R
                };

                for (int i = 0; i < bitmapData.Height; i++)
                {
                    byte* ptr = (byte*)(void*)bitmapData.Scan0 + i * bitmapData.Stride;
                    for (int j = 0; j < bitmapData.Width; j++)
                    {
                        if (ptr[j * 3] >= array[0] - ShadeVariation & ptr[j * 3] <= array[0] + ShadeVariation && ptr[j * 3 + 1] >= array[1] - ShadeVariation & ptr[j * 3 + 1] <= array[1] + ShadeVariation && ptr[j * 3 + 2] >= array[2] - ShadeVariation & ptr[j * 3 + 2] <= array[2] + ShadeVariation)
                        {
                            arrayList.Add(new Point(j + rect.X, i + rect.Y));
                        }
                    }
                }
                return (Point[])arrayList.ToArray(typeof(Point));
            }
        }
        public Point[] Search(Rectangle rect, Color Pixel_Color, int Shade_Variation)
        {
            ArrayList points = new ArrayList();
            Bitmap RegionIn_Bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);

            if (monitor >= Screen.AllScreens.Length)
            {
                monitor = 0;
            }

            int xOffset = Screen.AllScreens[monitor].Bounds.Left;
            int yOffset = Screen.AllScreens[monitor].Bounds.Top;

            using (Graphics GFX = Graphics.FromImage(RegionIn_Bitmap))
            {
                GFX.CopyFromScreen(rect.X + xOffset, rect.Y + yOffset, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }
            BitmapData RegionIn_BitmapData = RegionIn_Bitmap.LockBits(new Rectangle(0, 0, RegionIn_Bitmap.Width, RegionIn_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] Formatted_Color = new int[3] { Pixel_Color.B, Pixel_Color.G, Pixel_Color.R }; //bgr

            unsafe
            {
                for (int y = 0; y < RegionIn_BitmapData.Height; y++)
                {
                    byte* row = (byte*)RegionIn_BitmapData.Scan0 + y * RegionIn_BitmapData.Stride;
                    for (int x = 0; x < RegionIn_BitmapData.Width; x++)
                    {
                        if (row[x * 3] >= Formatted_Color[0] - Shade_Variation & row[x * 3] <= Formatted_Color[0] + Shade_Variation) //blue
                            if (row[x * 3 + 1] >= Formatted_Color[1] - Shade_Variation & row[x * 3 + 1] <= Formatted_Color[1] + Shade_Variation) //green
                                if (row[x * 3 + 2] >= Formatted_Color[2] - Shade_Variation & row[x * 3 + 2] <= Formatted_Color[2] + Shade_Variation) //red
                                    points.Add(new Point(x + rect.X, y + rect.Y));
                    }
                }
            }
            RegionIn_Bitmap.Dispose();
            return (Point[])points.ToArray(typeof(Point));
        }

        public Point[] SearchInSpiral(Rectangle rect, Color pixelColor, int shadeVariation)
        {
            var centerX = rect.X + rect.Width / 2;
            var centerY = rect.Y + rect.Height / 2;

            var points = new List<Point>();
            var steps = 0;
            var direction = 0; // 0 = right, 1 = down, 2 = left, 3 = up
            var x = centerX;
            var y = centerY;
            var found = false;

            while (steps < rect.Width * rect.Height && !found)
            {
                // Check current pixel
                var currentColor = GetPixelColor2(x, y);
                if (ColorsMatch(currentColor, pixelColor, shadeVariation))
                {
                    points.Add(new Point(x, y));
                    found = true;
                }

                // Move to next pixel in spiral pattern
                switch (direction)
                {
                    case 0: // right
                        x++;
                        if (x > rect.X + rect.Width - 1)
                        {
                            direction = 1;
                            x--;
                            y++;
                        }
                        break;

                    case 1: // down
                        y++;
                        if (y > rect.Y + rect.Height - 1)
                        {
                            direction = 2;
                            y--;
                            x--;
                        }
                        break;

                    case 2: // left
                        x--;
                        if (x < rect.X)
                        {
                            direction = 3;
                            x++;
                            y--;
                        }
                        break;

                    case 3: // up
                        y--;
                        if (y < rect.Y)
                        {
                            direction = 0;
                            y++;
                            x++;
                        }
                        break;
                }

                steps++;
            }

            return points.ToArray();
        }

        private Color GetPixelColor2(int x, int y)
        {
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var gfx = Graphics.FromImage(bitmap))
                {
                    gfx.CopyFromScreen(x, y, 0, 0, new Size(1, 1));
                }

                return bitmap.GetPixel(0, 0);
            }
        }

        private bool ColorsMatch(Color color1, Color color2, int shadeVariation)
        {
            var redDiff = Math.Abs(color1.R - color2.R);
            var greenDiff = Math.Abs(color1.G - color2.G);
            var blueDiff = Math.Abs(color1.B - color2.B);
            return redDiff <= shadeVariation && greenDiff <= shadeVariation && blueDiff <= shadeVariation;
        }
    }
}

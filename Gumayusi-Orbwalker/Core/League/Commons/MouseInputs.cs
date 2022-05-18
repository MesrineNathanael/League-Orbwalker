using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Gumayusi_Orbwalker.Core.League.Commons
{
    public class MouseInputs
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lppoint);

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int X { get; set; }
            public int Y { get; set; }
            public static implicit operator System.Drawing.Point(POINT point)
            {
                return new System.Drawing.Point(point.X, point.Y);
            }
        }

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        public void leftClick()
        {
            mouse_event((int)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(2);
            mouse_event((int)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
        }

        public void rightClick()
        {
            mouse_event((int)MouseEventFlags.RIGHTDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlags.RIGHTUP, 0, 0, 0, 0);
        }

        public void middleClick()
        {
            mouse_event((int)MouseEventFlags.MIDDLEDOWN, 0, 0, 0, 0);
            mouse_event((int)MouseEventFlags.MIDDLEUP, 0, 0, 0, 0);
        }

        public void SetPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public System.Drawing.Point GetPosition()
        {
            GetCursorPos(out POINT point);
            var drawingPoint = new System.Drawing.Point(point.X, point.Y);
            return drawingPoint;
        }
    }
}

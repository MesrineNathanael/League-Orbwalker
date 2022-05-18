using Gumayusi_Orbwalker.Core.League.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gumayusi_Orbwalker.Core.League.Commons
{
    public class KeyInjection
    {
        [DllImport("user32.dll")]
        private static extern bool InjectKeyboardInput(ref TagKeyInput input, uint count);


        //Async key injector
        public void TypeKeysAsync(string text, int delay = 10)
        {
            Task.Run(() =>
            {
                var keys = text.ToCharArray();
                foreach (var key in keys)
                {
                    TypeKey(KeyCodeCharWrapper.GetKey(key), delay);
                }
            });
        }

        public void TypeKeyAsync(KeyCode code, int delay = 10)
        {
            if (code == 0)
            {
                Console.WriteLine($"[ERROR] KeyCode <{code}> is not valid");
                return;
            }

            Task.Run(() =>
            {
                TypeKey(false, code);
                Sleep(delay);
                TypeKey(true, code);
            });
        }

        public void PressKeyAsync(KeyCode code)
        {
            Task.Run(() =>
            {
                TypeKey(false, code);
            });
        }

        public void PressKeyAsync(KeyCode code, bool isUp)
        {
            Task.Run(() =>
            {
                TypeKey(isUp, code);
            });
        }

        public void UpKeyAsync(KeyCode code)
        {
            Task.Run(() =>
            {
                TypeKey(true, code);
            });
        }

        private void TypeKey(KeyCode code, int delay = 10)
        {
            TypeKey(false, code);
            Sleep(delay);
            TypeKey(true, code);
        }

        private void TypeKey(bool up, KeyCode code)
        {
            TagKeyInput input = new TagKeyInput()
            {
                wScan = code
            };
            if (up)
            {
                input.dwFlags = KeyEventFlags.KEYEVENTF_KEYUP;
            }
            InjectKeyboardInput(ref input, 1);
        }

        private void Sleep(int microseconds)
        {
            var frequency = microseconds + Stopwatch.Frequency / 1000;
            var stopWatch = Stopwatch.StartNew();
            while (stopWatch.ElapsedTicks < frequency)
            {

            }
            stopWatch.Stop();
        }
    }
}

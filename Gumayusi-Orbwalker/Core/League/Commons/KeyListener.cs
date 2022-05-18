using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gumayusi_Orbwalker.Core.League.Commons
{
    public abstract class KeyListener
    {
        [DllImport("user32.dll")]
        protected static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        public static extern byte VkKeyScan(char ch);

        protected Thread ListenerThread;

        protected KeyInjection KeyInjector;

        protected MouseInputs MouseInputs;

        protected Key LastKeyPressed = Key.None;

        protected bool WaitForUpKey = false;

        public KeyListener(KeyInjection keyInjection, MouseInputs mouseInputs)
        {
            ListenerThread = new Thread(Listen);
            KeyInjector = keyInjection;
            MouseInputs = mouseInputs;
        }

        public void Start()
        {
            Log.WriteInfo("A new key Listener starting...");

            ListenerThread.SetApartmentState(ApartmentState.STA);
            ListenerThread.Start();

            if (ListenerThread.IsAlive)
            {
                Log.WriteInfo($"Key Listener started in thread {ListenerThread.ManagedThreadId}");
            }
        }

        protected virtual void Listen()
        {
            //This is the base piece of code that will be called on a new thread.
            //Don't call base.Listen().
            //Just copy this code and modify it as you need in your override method.
            while (true)
            {
                Thread.Sleep(10);

                WaitingForUpKey();

                //Exemple below
                /*
                while (true)
                {
                    Thread.Sleep(10);

                    WaitingForUpKey();

                    foreach (var shortcut in Shortcuts)
                    {
                        bool allKeyPressed = false;
                        foreach(var key in shortcut.Keys)
                        {
                            if (!Keyboard.IsKeyDown(key))
                            {
                                allKeyPressed = false;
                                break;
                            }
                            if (shortcut.LastKeyNeedToBeUp)
                            {
                                LastKeyPressed = key;
                            }

                            allKeyPressed = true;
                        }

                        if (allKeyPressed && !WaitForUpKey)
                        {
                            SetAndTypeKeys(shortcut.Keys.Last(), ParseSugarText(shortcut.ToString()));
                            WaitForUpKey = true;
                        }
                    }
                }
                */
            }
        }

        protected void SetAndTypeKeys(Key key, string text, int delay = 10)
        {
            WaitForUpKey = true;
            LastKeyPressed = key;
            Log.WriteDebug($"Attempt writing [{text.Replace("\r", "\\r")}]");
            KeyInjector.TypeKeysAsync(text, 10);
        }

        protected void TypeKey(string text, int delay = 10)
        {
            Log.WriteDebug($"Attempt writing [{text.Replace("\r", "\\r")}]");
            KeyInjector.TypeKeysAsync(text, delay);
        }

        protected void WaitingForUpKey()
        {
            if (LastKeyPressed == Key.None) return;
            if (WaitForUpKey)
            {
                if (Keyboard.IsKeyUp(LastKeyPressed))
                {
                    WaitForUpKey = false;
                }
            }
        }

        /// <summary>
        /// Sleep more precisely than Thread.Sleep
        /// 100000 ticks = 10 ms
        /// </summary>
        /// <param name="ticks">100000 = 10 ms</param>
        protected void Sleep(int ticks)
        {
            var frequency = ticks + Stopwatch.Frequency / 1000;
            var stopWatch = Stopwatch.StartNew();
            while (stopWatch.ElapsedTicks < frequency)
            {

            }
            stopWatch.Stop();
        }
    }
}

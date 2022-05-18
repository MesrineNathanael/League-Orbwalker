using Gumayusi_Orbwalker.Core.League.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace Gumayusi_Orbwalker.Core.League
{
    public class LeagueLogic : KeyListener
    {
        public List<Shortcut> Shortcuts = new List<Shortcut>
        {
            new Shortcut
            {
                Text1 = "\rTOP GET FLASH AT",
                Var1 = "%5",
                Var2 = "\r",
                Keys = new List<Key>
                {
                    Key.I,
                    Key.D7
                }
            },
            new Shortcut
            {
                Text1 = "\rJGL GET FLASH AT",
                Var1 = "%5",
                Var2 = "\r",
                Keys = new List<Key>
                {
                    Key.I,
                    Key.D8
                }
            },
            new Shortcut
            {
                Text1 = "\rMID GET FLASH AT",
                Var1 = "%5",
                Var2 = "\r",
                Keys = new List<Key>
                {
                    Key.I,
                    Key.D9
                }
            },
            new Shortcut
            {
                Text1 = "\rADC GET FLASH AT",
                Var1 = "%5",
                Var2 = "\r",
                Keys = new List<Key>
                {
                    Key.I,
                    Key.D0
                }
            },
            new Shortcut
            {
                Text1 = "\rUWU",
                Var2 = "\r",
                Keys = new List<Key>
                {
                    Key.I,
                    Key.U
                }
            },
            new Shortcut
            {
                Text1 = "\r/MUTE ALL",
                Var2 = "\r",
                Keys = new List<Key>
                {
                    Key.J,
                    Key.K
                }
            }

        };

        private DateTime _startTime;

        public LeagueLogic(KeyInjection keyInjection, MouseInputs mouseInputs) : base(keyInjection, mouseInputs)
        {

        }

        protected override void Listen()
        {
            Log.WriteInfo("League logic start listening");
            while (true)
            {
                Thread.Sleep(10);

                WaitingForUpKey();
                WaitingNewGame();

                foreach (var shortcut in Shortcuts)
                {
                    bool allKeyPressed = false;
                    foreach (var key in shortcut.Keys)
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
                        SetAndTypeKeys(shortcut.Keys.Last(), ParseSugarText(shortcut.ToString()), 500000);
                        WaitForUpKey = true;
                    }
                }
            }
        }

        private void WaitingNewGame()
        {

            if (Keyboard.IsKeyDown(Key.N) && Keyboard.IsKeyDown(Key.G))
            {
                StartNewGame();
            }
        }

        private void StartNewGame()
        {
            _startTime = DateTime.Now;
            Log.WriteInfo("New game of League Of Legends started at " + _startTime);
        }

        private string ParseSugarText(string text)
        {
            if (text.Contains("%5"))
            {
                var time = DateTime.Now - _startTime;
                var flashTime = time.Add(new TimeSpan(0, 5, 0));
                text = text.Replace("%5", flashTime.ToString("mm\\:ss"));
            }
            return text;
        }
    }
}

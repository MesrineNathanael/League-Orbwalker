using Gumayusi_Orbwalker.Core.League.Commons;
using Gumayusi_Orbwalker.Core.League.Commons.Enums;
using Gumayusi_Orbwalker.Core.League.PixelBot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gumayusi_Orbwalker.Core.League.OrbWalker
{
    public class OrbWalkerLogic : KeyListener
    {
        List<Shortcut> Shortcuts = new List<Shortcut>
        {
            new Shortcut()
            {
                //Attack range switch
                Keys = new List<Key>
                {
                    Key.M
                },
                Text1 = "rng"
            }
        };

        PixelSearchArray searchArray = new PixelSearchArray();

        private Key _orbWalkerActivationKey = Key.L;

        private char _showRangeKey = 'C';

        private char _attackChampionOnlyKey = 'Z';

        private char _attackOnCursorKey = 'X';

        private char _moveChampionKey = 'V';

        //move = V
        //Attack on cursor = X
        //Attcak champion only on down = Z

        private Thread _orbWalkerThread;

        private Thread _statScraperThread;

        private Thread _enemyHpScanner;

        private bool _gameStarted = false;

        private bool _showRange = false;

        private bool _attackChampionOnly = false;

        private double _currentPlayerAttackSpeed = 1;

        private double _windupOffset = 300;

        private double _windup = 1500; // in ms

        private int _championAnimationPause = 250;

        private string _currentChampion = "";

        public static int offsetX = 0;
        public static int offsetY = 95;

        private Color _enemyHpBarColor = Color.White;

        public OrbWalkerLogic(KeyInjection keyInjection, MouseInputs mouseInputs) : base(keyInjection, mouseInputs)
        {
            Log.WriteInfo($"OrbWalker starting...");
            _orbWalkerThread = new Thread(OrbWalk);
            _orbWalkerThread.SetApartmentState(ApartmentState.STA);
            _orbWalkerThread.Start();

            _statScraperThread = new Thread(PlayerStatsScraper);
            _statScraperThread.SetApartmentState(ApartmentState.STA);
            _statScraperThread.Start();

            _enemyHpScanner = new Thread(ScanEnemyHP);
            _enemyHpScanner.SetApartmentState(ApartmentState.STA);
            _enemyHpScanner.Start();

            if (_orbWalkerThread.IsAlive)
            {
                Log.WriteInfo($"OrbWalker started in thread {_orbWalkerThread.ManagedThreadId}");
            }

            if (_statScraperThread.IsAlive)
            {
                Log.WriteInfo($"Player stats scraper started in thread {_statScraperThread.ManagedThreadId}");
            }

            if (_enemyHpScanner.IsAlive)
            {
                Log.WriteInfo($"Enemy HP scanner started in thread {_enemyHpScanner.ManagedThreadId}");
            }
        }

        private void OrbWalk()
        {
            var canAttack = true;
            var championAttackOnlyIsToggled = false;

            var stopwatch = new Stopwatch();
            var lastMousePos = new Point();
            var moveTime = 3;
            var moveTimeCount = 0;
            while (true)
            {
                if (_gameStarted)
                {
                    if (Keyboard.IsKeyDown(_orbWalkerActivationKey))
                    {
                        CalculateWindup();

                        if (!championAttackOnlyIsToggled && _attackChampionOnly)
                        {
                            KeyInjector.PressKeyAsync(KeyCodeCharWrapper.GetKey(_attackChampionOnlyKey), false);
                            championAttackOnlyIsToggled = true;
                        }

                        if (canAttack)
                        {
                            lastMousePos = MouseInputs.GetPosition();

                            Log.WriteDebug("Attacking...");
                            var enemyPos = new Point();
                            if (searchArray.enemyHPArrayGlobal.Count() > 0)
                            {
                                enemyPos = searchArray.enemyHPArrayGlobal[searchArray.enemyHPArrayGlobal.Count() / 2];
                            }

                            if (searchArray.enemyHPArrayGlobal.Count() > 0)
                            {
                                if (searchArray.enemyHPArrayGlobal.Count() < 20)
                                {
                                    Log.WriteWarning("Low enemy array, click will not perfom");
                                }
                                else
                                {
                                    MouseInputs.SetPosition(enemyPos.X, enemyPos.Y + offsetY);

                                }
                            }

                            TypeKey(_attackOnCursorKey.ToString(), 10000);

                            //Thread.Sleep(30);
                            Sleep(300000);

                            if (lastMousePos.X != 0)
                            {
                                MouseInputs.SetPosition(lastMousePos.X, lastMousePos.Y);
                            }

                            //Thread.Sleep(_championAnimationPause - 30);
                            Sleep(_championAnimationPause * 10000 - 30);

                            canAttack = false;

                            stopwatch.Start();
                        }
                        else
                        {
                            if (stopwatch.ElapsedMilliseconds > _windup - _windupOffset)
                            {
                                canAttack = true;
                                stopwatch.Reset();
                            }

                            if (moveTimeCount >= moveTime)
                            {
                                TypeKey(_moveChampionKey.ToString(), 10000);
                                moveTimeCount = 0;
                            }
                            else
                            {
                                moveTimeCount++;
                            }

                        }

                        Thread.Sleep(10);
                    }
                    else
                    {
                        if (championAttackOnlyIsToggled && _attackChampionOnly)
                        {
                            KeyInjector.PressKeyAsync(KeyCodeCharWrapper.GetKey(_attackChampionOnlyKey), true);
                            championAttackOnlyIsToggled = false;
                        }
                        Thread.Sleep(10);
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        protected override void Listen()
        {
            Log.WriteInfo("OrbWalker logic start listening");
            while (true)
            {
                //Sleep(100000);
                Thread.Sleep(10);

                WaitingForUpKey();

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
                        ParseSugarText(shortcut.ToString());
                        WaitForUpKey = true;
                    }
                }
            }
        }

        private void CalculateWindup()
        {
            if (_currentPlayerAttackSpeed == 0) return;
            var cache = 1 / _currentPlayerAttackSpeed;
            _windup = cache * 1000;
        }

        private void PlayerStatsScraper()
        {
            while (true)
            {
                if (_gameStarted)
                {
                    if (_currentPlayerAttackSpeed == 0)
                    {
                        Log.WriteWarning("Player current attack speed can't be detected.");
                        _currentPlayerAttackSpeed = 1;
                    }
                    var autoA = ApiScraper.PlayerAttackSpeed();
                    autoA = autoA.Replace(',', '.');
                    if (autoA != "")
                        _currentPlayerAttackSpeed = Convert.ToDouble(autoA);

                    //Log.WriteDebug($"Current player auto attack speed is {_currentPlayerAttackSpeed}");
                }
                Thread.Sleep(500);
            }
        }

        private void PlayerChampionScraper()
        {
            _currentChampion = ApiScraper.PlayerChampionName();

            _championAnimationPause = ChampionWindupData.GetChampionWindup(_currentChampion);

            Log.WriteInfo($"Your champion is : {_currentChampion}");
            Log.WriteInfo($"Base windup for {_currentChampion} is {_championAnimationPause}ms");
        }

        private void ScanEnemyHP()
        {
            var pxbot = new PixelSearch();
            //string ENEMYHP = "#a52c21"; // set to user liking
            //string ENEMYHP = "#a52c21";
            // ENEMYcolor = ColorTranslator.FromHtml(ENEMYHP);
            while (true)
            {
                if (_gameStarted)
                {
                    Point[] enemyArray = pxbot.Search(new Rectangle(0, 0, 1920, 1080), _enemyHpBarColor, 0);
                    searchArray.enemyHPArrayGlobal = enemyArray;
                    if (enemyArray.Count() > 0)
                    {
                        Log.WriteDebug($"Enemy found at {enemyArray[enemyArray.Count() / 2].X};{enemyArray[enemyArray.Count() / 2].Y}");
                    }

                    Thread.Sleep(50);
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private string ParseSugarText(string text)
        {
            if (text.Contains("orb"))
            {
                _gameStarted = !_gameStarted;
                Log.WriteInfo($"OrbWalker : game started set to {_gameStarted}");
            }
            else if (text.Contains("rng"))
            {
                _showRange = !_showRange;
                Log.WriteInfo($"OrbWalker : show range set to {_showRange}");
                KeyInjector.PressKeyAsync(KeyCodeCharWrapper.GetKey(_showRangeKey), !_showRange);
            }
            else if (text.Contains("ach"))
            {
                _attackChampionOnly = !_attackChampionOnly;
                Log.WriteInfo($"OrbWalker : attack champion only set to {_attackChampionOnly}");
            }
            return text;
        }

        public void SetActivationState(bool isStarted)
        {
            _gameStarted = isStarted;
        }

        public bool GetActivationState()
        {
            return _gameStarted;
        }

        public void SetChampWindup(int windup)
        {
            _championAnimationPause = windup;
        }

        public void SetEnemyHpColorBar(string color)
        {
            _enemyHpBarColor = ColorTranslator.FromHtml(color);
        }

    }
}

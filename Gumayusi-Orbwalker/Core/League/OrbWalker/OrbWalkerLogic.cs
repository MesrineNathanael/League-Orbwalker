using Gumayusi_Orbwalker.Core.League.Commons;
using Gumayusi_Orbwalker.Core.League.Commons.Enums;
using Gumayusi_Orbwalker.Core.League.PixelBot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace Gumayusi_Orbwalker.Core.League.OrbWalker
{
    public class OrbWalkerLogic : KeyListener
    {
        readonly List<Shortcut> _shortcuts = new()
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

        readonly PixelSearchArray _searchArray = new();

        private const Key OrbWalkerActivationKey = Key.L;

        private const Key OrbWalkerWithoutEnemyActivationKey = Key.K;

        private const char ShowRangeKey = 'C';

        private const char AttackChampionOnlyKey = 'Z';

        private const char AttackOnCursorKey = 'X';

        private const char MoveChampionKey = 'V';

        //move = V
        //Attack on cursor = X
        //Attack champion only on down = Z

        private bool _gameStarted = false;

        private bool _showRange = false;

        private bool _attackChampionOnly = false;

        private double _currentPlayerAttackSpeed = 1;

        private readonly double _windupOffset = 300;

        private double _windup = 1500; // in ms

        private int _championAnimationPause = 250;

        private string _currentChampion = "";

        public static int OffsetX = 0;

        public static int OffsetY = 95;
        
        private Color _enemyHpBarColor = Color.White;

        public bool ScanEnemyEnabled = true;

        public OrbWalkerLogic(KeyInjection keyInjection, MouseInputs mouseInputs) : base(keyInjection, mouseInputs)
        {
            Log.WriteInfo($"OrbWalker starting...");
            var orbWalkerThread = new Thread(OrbWalk);
            orbWalkerThread.SetApartmentState(ApartmentState.STA);
            orbWalkerThread.Start();

            var statScraperThread = new Thread(PlayerStatsScraper);
            statScraperThread.SetApartmentState(ApartmentState.STA);
            statScraperThread.Start();

            var enemyHpScanner = new Thread(ScanEnemyHp);
            enemyHpScanner.SetApartmentState(ApartmentState.STA);
            enemyHpScanner.Start();

            if (orbWalkerThread.IsAlive)
            {
                Log.WriteInfo($"OrbWalker started in thread {orbWalkerThread.ManagedThreadId}");
            }

            if (statScraperThread.IsAlive)
            {
                Log.WriteInfo($"Player stats scraper started in thread {statScraperThread.ManagedThreadId}");
            }

            if (enemyHpScanner.IsAlive)
            {
                Log.WriteInfo($"Enemy HP scanner started in thread {enemyHpScanner.ManagedThreadId}");
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

                    if (Keyboard.IsKeyDown(OrbWalkerActivationKey) || Keyboard.IsKeyDown(OrbWalkerWithoutEnemyActivationKey))
                    {
                        if (Keyboard.IsKeyDown(OrbWalkerWithoutEnemyActivationKey))
                        {
                            ScanEnemyEnabled = false;
                        }
                        else
                        {
                            ScanEnemyEnabled = true;
                        }

                        CalculateWindup();

                        if (!championAttackOnlyIsToggled && _attackChampionOnly)
                        {
                            KeyInjector.PressKeyAsync(KeyCodeCharWrapper.GetKey(AttackChampionOnlyKey), false);
                            championAttackOnlyIsToggled = true;
                        }

                        if (canAttack)
                        {
                            lastMousePos = MouseInputs.GetPosition();

                            Log.WriteDebug("Attacking...");
                            var enemyPos = new Point();
                            if (_searchArray.EnemyHpArrayGlobal.Any())
                            {
                                enemyPos = _searchArray.EnemyHpArrayGlobal[_searchArray.EnemyHpArrayGlobal.Count() / 2];
                                //enemyPos = _searchArray.EnemyHpArrayGlobal[0];
                            }
                            else
                            {
                            }

                            if (_searchArray.EnemyHpArrayGlobal.Any() && ScanEnemyEnabled)
                            {
                                if (_searchArray.EnemyHpArrayGlobal.Count() < 20)
                                {
                                    Log.WriteWarning("Low enemy array, click will not perfom");
                                }
                                else
                                {
                                    MouseInputs.SetPosition(enemyPos.X, enemyPos.Y + OffsetY);

                                }
                            }

                            TypeKey(AttackOnCursorKey.ToString(), 10000);

                            //Thread.Sleep(30);
                            Sleep(300000);

                            if (lastMousePos.X != 0 && ScanEnemyEnabled)
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
                                TypeKey(MoveChampionKey.ToString(), 10000);
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
                            KeyInjector.PressKeyAsync(KeyCodeCharWrapper.GetKey(AttackChampionOnlyKey), true);
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

                foreach (var shortcut in _shortcuts)
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

                    if (!allKeyPressed || WaitForUpKey) continue;

                    ParseSugarText(shortcut.ToString());
                    WaitForUpKey = true;
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
                    var culture = CultureInfo.CurrentCulture;
                    var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;

                    autoA = autoA.Replace(",", decimalSeparator).Replace(".", decimalSeparator);
                    if (autoA != "")
                    {
                        _currentPlayerAttackSpeed = Convert.ToDouble(autoA);
                    }

                    //Log.WriteDebug($"Current player auto attack speed is {_currentPlayerAttackSpeed}");
                }
                Thread.Sleep(500);
            }
        }
        
        private void ScanEnemyHp()
        {
            var pxbot = new PixelSearch();
            //string ENEMYHP = "#a52c21"; // set to user liking
            //string ENEMYHP = "#a52c21";
            // ENEMYcolor = ColorTranslator.FromHtml(ENEMYHP);
            while (true)
            {
                if (_gameStarted)
                {
                    Point[] enemyArray = pxbot.Search(new Rectangle(450, 175, 1350, 860), _enemyHpBarColor, 2);
                    //Point[] enemyArray = pxbot.SearchInSpiral(new Rectangle(0, 0, 1920, 1080), _enemyHpBarColor, 60);
                    _searchArray.EnemyHpArrayGlobal = enemyArray;
                    if (enemyArray.Any())
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
                KeyInjector.PressKeyAsync(KeyCodeCharWrapper.GetKey(ShowRangeKey), !_showRange);
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
            try
            {
                _enemyHpBarColor = ColorTranslator.FromHtml(color);
            }
            catch (Exception e)
            {
                Log.WriteError($"Error while setting enemy hp color bar : {e.Message}.");
            }
        }

    }
}

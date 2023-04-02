using Gumayusi_Orbwalker.Core.League.Commons;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using System.Timers;
using Timer = System.Timers.Timer;
using System.Diagnostics;
using Gumayusi_Orbwalker.Core;
using MessageBox = HandyControl.Controls.MessageBox;

namespace Gumayusi_Orbwalker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : GlowWindow
    {
        private readonly List<Champion> _champions;

        private Champion _selectedChampion = new ();

        private readonly LeagueHolder _leagueHolder;

        public Champion SelectedChampion
        {
            get => _selectedChampion;
            set
            {
                if (_selectedChampion != value)
                {
                    _selectedChampion = value;
                    OnSelectedChampionChanged();
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            var uiTimer = new Timer
            {
                Interval = 500
            };
            uiTimer.Elapsed += UiTimer_Elapsed;

            _champions = new List<Champion>()
            {
                new Champion()
                {
                    Id = 0,
                    Name = "Vayne",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/Vayne_12.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 1,
                    Name = "Tristana",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/Tristana_33.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 2,
                    Name = "Kai'sa",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/Kaisa_0.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 3,
                    Name = "KogMaw",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/KogMaw_9.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 4,
                    Name = "Xayah",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/Xayah_4.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 5,
                    Name = "Jinx",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/Jinx_29.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 6,
                    Name = "Miss Fortune",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/MissFortune_16.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 7,
                    Name = "Caitlyn",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/Caitlyn_13.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 8,
                    Name = "Windup 200 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 200
                },
                new Champion()
                {
                    Id = 9,
                    Name = "Windup 210 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 210
                },
                new Champion()
                {
                    Id = 10,
                    Name = "Windup 220 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 220
                },
                new Champion()
                {
                    Id = 11,
                    Name = "Windup 230 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 12,
                    Name = "Windup 240 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 240
                },
                new Champion()
                {
                    Id = 13,
                    Name = "Windup 250 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 250
                },
                new Champion()
                {
                    Id = 14,
                    Name = "Windup 260 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 260
                }
            };

            foreach (var champ in _champions)
            {
                ChampionFlow.Add(champ.PictureUri);
            }

            _leagueHolder = new LeagueHolder();

            SelectedChampion = _champions[0];

            uiTimer.Start();
            UpdateChampionUi();
            UpdateUi();
        }

        private void UiTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateSelectedChampion(ChampionFlow.PageIndex);
                });

            }
            catch (Exception)
            {
                Environment.Exit(0);
            }
        }

        private void UpdateSelectedChampion(int index)
        {
            if (_selectedChampion.Id == index) return;
            SelectedChampion = _champions.FirstOrDefault(ch => ch.Id == index) ?? _champions[0];
        }

        private void OnSelectedChampionChanged()
        {
            //update UI etc
            UpdateChampionUi();

            _leagueHolder.OrbWalker.SetChampWindup(SelectedChampion.Windup);

            Trace.WriteLine($"champ name : {SelectedChampion.Name}");
        }

        private void UpdateChampionUi()
        {
            ChampNameTextBlock.Text = SelectedChampion.Name;
            WindupTextBlock.Text = "Base windup : " + SelectedChampion.Windup.ToString() + "ms";
        }

        private void UpdateUi()
        {
            var actState = _leagueHolder.OrbWalker.GetActivationState();
            StatusTextBlock.Text = actState ? "Running" : "Disabled";
            StatusTextBlock.Foreground = actState ? new SolidColorBrush(Color.FromRgb(0,255,0)) : new SolidColorBrush(Color.FromRgb(255, 0, 0));

            ActivateButton.Content = actState ? "Disable Orb." : "Activate Orb.";

            ChampionFlow.IsEnabled = !actState;

        }

        private void ActiveButton_Click(object sender, RoutedEventArgs e)
        {
            _leagueHolder.OrbWalker.SetActivationState(!_leagueHolder.OrbWalker.GetActivationState());
            
            UpdateUi();
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x =>Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private void SaveConfig()
        {
            var conf = _leagueHolder.Settings.Config;
            conf.isHighAccuracyModeChecked = HighAccuracyIsChecked;
            conf.EnemyHpColorHtml = _enemyHpColor;
            _leagueHolder.SaveSettingsAndApply();
        }

        private void LoadConfig(bool loadFile = true)
        {
            if(loadFile)
                _leagueHolder.LoadSettingsAndApply();

            var conf = _leagueHolder.Settings.Config;

            HighAccuracyIsChecked = conf.isHighAccuracyModeChecked;
            EnemyHpColor = conf.EnemyHpColorHtml;
            EnemyHpBarColorTextBlock.Text = EnemyHpColor[1..];
            EnemyHpColorTextBox.Text = EnemyHpColor[1..];
        }

        #region UI stuff

        private bool _isSettingsPanelOpen;

        public bool IsSettingsPanelOpen
        {
            get => _isSettingsPanelOpen;
            set
            {
                if (_isSettingsPanelOpen == value) return;
                
                _isSettingsPanelOpen = value;
                SettingsPanelGrid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                if(value == false)
                {
                    SaveConfig();
                }
                else
                {
                    LoadConfig();
                }
            }
        }

        public bool HighAccuracyIsChecked { get; set; }

        private string _enemyHpColor = string.Empty;

        public string EnemyHpColor
        {
            get => _enemyHpColor;
            set
            {
                if(value.Length != 7)
                {
                    EnemyHpColorRectangle.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                else
                {
                    if (value.Length != 7) return;

                    _enemyHpColor = value;
                    try
                    {
                        var rgb = StringToByteArray(value.Substring(1));
                        EnemyHpColorRectangle.Fill = new SolidColorBrush(Color.FromRgb(rgb[0], rgb[1], rgb[2]));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Invalid color format. Please use hex format. Example : FFFFFF", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }
        }

        private int _windupOffset;

        public int WindupOffset
        {
            get => _windupOffset;
            set
            {
                _windupOffset = value;
                WindupOffsetNumericUpDown.Value = _windupOffset;
            }
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            IsSettingsPanelOpen = !IsSettingsPanelOpen;
        }

        private void HighAccuracy_Click(object sender, RoutedEventArgs e)
        {
            HighAccuracyIsChecked = AccuracyCheckBox.IsChecked ?? false;
            AccuracyModeTextBlock.Text = HighAccuracyIsChecked ? "High" : "Low";
        }

        private void EnemyHpColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EnemyHpColorRectangle == null) return;
            if (EnemyHpColorTextBox.Text.Length == 6)
            {
                EnemyHpColor = "#" + EnemyHpColorTextBox.Text;
            }
            else
            {
                EnemyHpColor = "#FFFFFF";
            }
        }

        private void WindupOffsetNumericUpDown_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            WindupOffset = Convert.ToInt32(WindupOffsetNumericUpDown.Value);
        }

        #endregion

        private void GlowWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void ButtonAbout_OnClick(object sender, RoutedEventArgs e)
        {
            //message box
            MessageBox.Show("League of Legends Orbwalker by Nathanael Mesrine\n\n" +
                            "This tool is made for educational purposes only.\n" +
                            "I am not responsible for any bans or any other consequences.", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

using Gumayusi_Orbwalker.Core.League.Commons;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Timers;
using Timer = System.Timers.Timer;
using System.Diagnostics;
using Gumayusi_Orbwalker.Core;

namespace Gumayusi_Orbwalker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : GlowWindow
    {
        private static Timer UiTimer; 
        
        private readonly List<Champion> champions;

        private Champion _selectedChampion = new Champion();

        private readonly LeagueHolder leagueHolder;

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

            UiTimer = new Timer
            {
                Interval = 500
            };
            UiTimer.Elapsed += UiTimer_Elapsed;

            champions = new List<Champion>()
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
                    Name = "Windup 220 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 220
                },
                new Champion()
                {
                    Id = 9,
                    Name = "Windup 230 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 10,
                    Name = "Windup 240 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 240
                },
                new Champion()
                {
                    Id = 11,
                    Name = "Windup 250 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 250
                },
                new Champion()
                {
                    Id = 12,
                    Name = "Windup 260 ms",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/BgLissandra.jpg"),
                    Windup = 260
                }
            };

            foreach (var champ in champions)
            {
                ChampionFlow.Add(champ.PictureUri);
            }

            leagueHolder = new LeagueHolder();

            SelectedChampion = champions[0];

            UiTimer.Start();
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
            SelectedChampion = champions.FirstOrDefault(ch => ch.Id == index) ?? champions[0];
        }

        private void OnSelectedChampionChanged()
        {
            //update UI etc
            UpdateChampionUi();

            leagueHolder.OrbWalker.SetChampWindup(SelectedChampion.Windup);

            Trace.WriteLine($"champ name : {SelectedChampion.Name}");
        }

        private void UpdateChampionUi()
        {
            ChampNameTextBlock.Text = SelectedChampion.Name;
            WindupTextBlock.Text = "Base windup : " + SelectedChampion.Windup.ToString() + "ms";
        }

        private void UpdateUi()
        {
            var actState = leagueHolder.OrbWalker.GetActivationState();
            StatusTextBlock.Text = actState ? "Running" : "Disabled";
            StatusTextBlock.Foreground = actState ? new SolidColorBrush(Color.FromRgb(0,255,0)) : new SolidColorBrush(Color.FromRgb(255, 0, 0));

            ActivateButton.Content = actState ? "Disable Orb." : "Activate Orb.";

            ChampionFlow.IsEnabled = !actState;

        }

        private void ActiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!leagueHolder.OrbWalker.GetActivationState())
            {
                leagueHolder.OrbWalker.SetActivationState(true);
            }
            else
            {
                leagueHolder.OrbWalker.SetActivationState(false);
            }
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
            var conf = leagueHolder.Settings.Config;
            conf.isHighAccuracyModeChecked = _highAccuracyIsChecked;
            conf.EnemyHpColorHtml = _enemyHpColor;
            leagueHolder.SaveSettingsAndApply();
        }

        private void LoadConfig(bool loadFile = true)
        {
            if(loadFile)
                leagueHolder.LoadSettingsAndApply();

            var conf = leagueHolder.Settings.Config;

            HighAccuracyIsChecked = conf.isHighAccuracyModeChecked;
            EnemyHpColor = conf.EnemyHpColorHtml ?? "#FFFFFF";
            EnemyHpBarColorTextBlock.Text = EnemyHpColor.Substring(1) ?? "FFFFFF";
            EnemyHpColorTextBox.Text = EnemyHpColor.Substring(1) ?? "FFFFFF";
        }

        #region UI stuff

        private bool _isSettingsPanelOpen = false;

        public bool IsSettingsPanelOpen
        {
            get => _isSettingsPanelOpen;
            set
            {
                if(_isSettingsPanelOpen != value)
                {
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
        }

        private bool _highAccuracyIsChecked;

        public bool HighAccuracyIsChecked
        {
            get => _highAccuracyIsChecked;
            set
            {
                _highAccuracyIsChecked = value;
            }
        }

        private string _enemyHpColor;

        public string EnemyHpColor
        {
            get => _enemyHpColor;
            set
            {
                if(value.Length != 7 || value == null)
                {
                    EnemyHpColorRectangle.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                else
                {
                    if(value.Length == 7)
                    {
                        _enemyHpColor = value;

                        var RGB = StringToByteArray(value.Substring(1));

                        EnemyHpColorRectangle.Fill = new SolidColorBrush(Color.FromRgb(RGB[0], RGB[1], RGB[2]));
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
            HighAccuracyIsChecked = !HighAccuracyIsChecked;
            AccuracyCheckBox.IsChecked = _highAccuracyIsChecked;
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
    }
}

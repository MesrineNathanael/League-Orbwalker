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
        
        private List<Champion> champions;

        private Champion _selectedChampion = new Champion();

        private LeagueHolder leagueHolder;

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

            UiTimer = new Timer();
            UiTimer.Interval = 500;
            UiTimer.Elapsed += UiTimer_Elapsed;

            champions = new List<Champion>()
            {
                new Champion()
                {
                    Id = 0,
                    Name = "Vayne",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/Vayne_loading.jpg"),
                    Windup = 230
                },
                new Champion()
                {
                    Id = 1,
                    Name = "Tristana",
                    PictureUri = new Uri(@"pack://application:,,,/Gumayusi-Orbwalker;component/Resources/Img/Tristana_loading.jpg"),
                    Windup = 230
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
            Dispatcher.Invoke(() =>
            {
                UpdateSelectedChampion(ChampionFlow.PageIndex);
            });
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

    }
}

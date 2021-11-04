using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;
using Model;
using Controller;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            VisualiseImages.InitializeCache();
            Data.Initialize();
            Data.NextRace();
            Initialize();
        }

        public void Initialize()
        {
            Data.CurrentRace.DriversChanged += OnDriversChanged;
        }


        private void OnDriversChanged(Object sender, DriversChangedEventArgs e)
        {
            ImageCanvas.Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
                {
                    ImageCanvas.Source = null; 
                    ImageCanvas.Source = Visualise.DrawTrack(Data.CurrentRace.Track);
                }));
            Data.CurrentRace.NextRace += OnNextRace;
        }

        private void OnNextRace(Object sender, NextRaceEventArgs e)
        {
            Initialize();
        }


        //ACTIONS ON SCREEN
        public  void Open_Race_Details(object sender, RoutedEventArgs e)
        {
/*            RaceDetails = new RaceStatisticsWindow();

            Data.CurrentRace.NextRace += ((RaceDataContext)RaceDetails.DataContext).OnNextRaceEvent;
            ((RaceDataContext)RaceDetails.DataContext).OnNextRaceEvent(null, new RaceStartEventArgs(Data.CurrentRace));

            RaceDetails.Show();*/
        }

        private void Open_Competition_Details(object sender, RoutedEventArgs e)
        {
/*            CompetitionDetails = new StatisticsWindow();
            Data.CurrentRace.NextRace += ((CompetitionDataContext)CompetitionDetails.DataContext).OnNextRaceEvent;

            ((CompetitionDataContext)CompetitionDetails.DataContext).OnNextRaceEvent(null, new RaceStartEventArgs(Data.CurrentRace));

            CompetitionDetails.Show();*/
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}

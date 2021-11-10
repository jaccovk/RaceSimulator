using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
            Data.Initialize();
            Data.NextRaceEvent += OnNextRace;
            Data.NextRace();
            
        }

        private void OnDriversChanged(Object sender, DriversChangedEventArgs e)
        {
            //Visualise.Initialize();
            ImageCanvas.Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
                {
                    ImageCanvas.Source = null; 
                    ImageCanvas.Source = Visualise.DrawTrack(e.Track);
                }));
            //Data.CurrentRace.NextRace += OnNextRace;
        }

        private void OnNextRace(Object sender, NextRaceEventArgs e)
        {
            VisualiseImages.InitializeCache();
            Visualise.Initialize(e.Race);
            e.Race.DriversChanged += OnDriversChanged;
        }


        //ACTIONS ON SCREEN
        public  void Open_Race_Details(object sender, RoutedEventArgs e)
        { 
            ShowDriverData RaceDetails = new ShowDriverData();

            Data.CurrentRace.DriversChanged += ((ShowDriverDataContext)RaceDetails.DataContext).OnDriversChanged;
            


            RaceDetails.Show();
        }

        private void Open_Competition_Details(object sender, RoutedEventArgs e)
        {
            ShowDriverCompetitionStats CompetitionDetails = new ShowDriverCompetitionStats();

            CompetitionDetails.Show();
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}

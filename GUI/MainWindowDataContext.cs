using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Controller;
using GUI.Annotations;
using Model;

namespace GUI
{
    public class MainWindowDataContext: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _trackName;
        public string TrackName { get => _trackName; set { _trackName = value; }
        }



        public void OnDriversChanged(Object sender, DriversChangedEventArgs e)
        {
            TrackName = Data.CurrentRace.Track.Name;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
            
        }
    }
}

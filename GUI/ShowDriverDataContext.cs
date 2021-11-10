using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Controller;
using Model;

namespace GUI
{
    class ShowDriverDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<IParticipant> Driver { get; set; }
        public string RaceName { get; set; }
        public List<Section> RaceSections { get; set; }


        public void OnDriversChanged(object sender, DriversChangedEventArgs eventArgs)
        {
            Driver = Data.CurrentRace.Participants.ToList();
            RaceName = Data.CurrentRace.Track.Name;
            RaceSections = Data.CurrentRace.Track.Sections.ToList();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }
}

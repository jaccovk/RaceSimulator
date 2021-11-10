using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Controller;
using GUI.Annotations;
using Model;

namespace GUI
{
    class CompetitionDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<IParticipant> Driver => Data.CurrentRace.Participants.ToList();
        public Competition competition => Data.competition;


    }
}

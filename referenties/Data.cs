using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Controller;
using Model;
namespace Controller
{
    public static class Data
    {
        public static Competition competition;
        public static Race CurrentRace { get; set; }
        public static int SectionLength = 100;
        public static int Rounds= 2;
        public static event EventHandler<NextRaceEventArgs> NextRaceEvent;

        //set the pointsize given to the drivers after finished the track
        public static int FirstPlace = 25;
        public static int SecondPlace= 20;
        public static int ThirdPlace = 10;

        public static void AddParticipants()
        {
            Driver JaccoVerstappen = new Driver("Jacco Verstappen", 0, new Car(10, 7, 10, false), TeamColors.Red);
            Driver MamaVerstappen = new Driver("Mama Verstappen", 0, new Car(10, 5, 9, false), TeamColors.Yellow);
            Driver JosB = new Driver("Jos Brech", 0, new Car(5, 10, 7, false), TeamColors.Blue);
            Driver RicoVerhoeven= new Driver("Rico Verhoeven", 0, new Car(10, 3, 9, false), TeamColors.Green);

            competition.Participants.Add(RicoVerhoeven);
            competition.Participants.Add(JosB);
            competition.Participants.Add(JaccoVerstappen);
            competition.Participants.Add(MamaVerstappen);

            
            
            
        }

        public static void AddTracks()
        {
            competition.Tracks.Enqueue(new Track("Zandvoort", new SectionTypes[] { SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.LeftCorner, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.LeftCorner }));
            competition.Tracks.Enqueue(new Track("Test", new SectionTypes[] { SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner }));
            competition.Tracks.Enqueue(new Track("Zandvoort1", new SectionTypes[] { SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.RightCorner }));
            competition.Tracks.Enqueue(new Track("Turkije", new SectionTypes[] { SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.LeftCorner, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.LeftCorner }));
        }



        public static void NextRace()
        {
            var r = competition.NextTrack();
            if(r != null)
            {
              CurrentRace = new Race(r, competition.Participants);
              NextRaceEvent?.Invoke(null, new NextRaceEventArgs()
              {
                  Race = CurrentRace
              });
            }
            else
            {
                NextRaceEvent?.Invoke(null, new NextRaceEventArgs()
                {
                    Race = CurrentRace
                });
                CurrentRace = null;
                Console.WriteLine("De podiumplekken zijn voor :::");
            }
        }

        public static void Initialize()
        {
            competition = new Competition();
            competition.CompetitionName = "Busracing";
            AddParticipants();
            AddTracks();
        }

    }
}

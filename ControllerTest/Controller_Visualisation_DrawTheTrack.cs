using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ConsoleView;
using NUnit.Framework;
using Model;
using Controller;
using NUnit.Framework.Constraints;

namespace ControllerTest
{
    [TestFixture]
    public class Model_Visualisation_DrawTheTrack
    {
        private Track track;
        private Race race;
        private List<IParticipant> participants;
        private IEquipment equipment;

        #region graphics

        private static string[] _finishHorizontal = {"----",
                                                     " 1# ",
                                                     " 2# ",
                                                     "----"};

        private static string[] _finishVertical = { "|  |",
                                                    "|1#|",
                                                    "|2#|",
                                                    "|  |" };

        private static string[] _startHorizontaal = { "----",
                                                      "  1]",
                                                      " 2] ",
                                                      "----" };

        private static string[] _startVerticaal = { "-  -",
                                                    "^1  ",
                                                    " ^2 ",
                                                    "-  -" };

        private static string[] _straightHorizontaal = {"----",
                                                        " 1  ",
                                                        "  2 ",
                                                        "----"};
        private static string[] _straightVertical = {"|  |",
                                                     "|1 |",
                                                     "| 2|",
                                                     "|  |"};

        private static string[] _cornerLEast = { "   |", " 1 |", "  2/", "--/ " };
        private static string[] _cornerLNorth = { @"--\ ", @" 1 \", @"  2|", @"   |" };
        private static string[] _cornerLWest = { @" /--", @"/ 1 ", @"|  2", @"|   " };
        private static string[] _cornerLSouth = { @"|   ", @"|1  ", @"\ 2 ", @" \--" };

        private static string[] _cornerREast = { @"--\ ", @"1  \", @" 2 |", @"   |" };
        private static string[] _cornerRSouth = { "   |", " 1 |", "  2/", "--/ " };
        private static string[] _cornerRWest = { "|   ", "|1  ", @"\ 2 ", @" \--" };
        private static string[] _cornerRNorth = { " /--", "/1  ", "| 2 ", "|   " };

        #endregion


        [SetUp]
        public void SetUp()
        {
            Track tr = new Track("Rondje",
                new SectionTypes[]
                {
                    SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner
                });

            participants = new List<IParticipant>();
            equipment = new Car(0, 0, 0, false);
            participants.Add(new Driver("Jacco Verstappen", 0, new Car(10, 3, 10, false), TeamColors.Red));
            participants.Add(new Driver("Mama Verstappen", 0, new Car(10, 3, 9, false), TeamColors.Yellow));

            track = tr;
            race = new Race(track, participants);
        }



        [Test]
        public void Test_GetSectionData_IsNotNull()
        {
            Section section = track.Sections.First?.Value;
            race.GetSectionData(section);
            Assert.IsNotNull(section);
        }


        [Test]
        public void ReplacePlaceHolders_PlaceParticipants_IsNotNull()
        {
            race.PlaceParticipantsOnStartGrid();

            string output;

                output = Visualisation.ReplacePlaceHoldersAndPlaceParticipantsOnGrid(_startHorizontaal[1],
                    race.GetSectionData(track.Sections.First?.Value).Left,
                    race.GetSectionData(track.Sections.First?.Value).Right);
            Assert.IsNotEmpty(output);
        }

        [Test]
        public void Test_Quality()
        {
            Assert.IsNotNull(participants[0].Equipment.Quality);
        }

        [Test]
        public void Test_Points()
        {
            participants[0].Points = 10;
            Assert.IsNotNull(participants[0].Points);
        }

        [Test]
        public void Test_TeamColor()
        {
            Assert.IsNotNull(participants[0].TeamColor);
        }

        [Test]
        public void Test_NextRaceEventArgs_IsNotNull()
        {
            var NextRace = new NextRaceEventArgs()
            {
                Race = race
            };

            Assert.IsNotNull(NextRace);
            Assert.IsNotNull(NextRace.Race);
        }
        
        
    }
}

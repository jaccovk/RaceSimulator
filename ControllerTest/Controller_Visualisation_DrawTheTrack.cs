using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Model;
using Controller;
using NUnit.Framework.Constraints;

namespace ControllerTest
{
    [TestFixture]
    public class Model_Visualisation_DrawTheTrack
    {
        private Competition competition;
        private Track track;

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
            Track tr = new Track("Spa_Section2",
                new SectionTypes[]
                {
                    SectionTypes.StartGrid, SectionTypes.Finish, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner
                });
            Competition comp = new Competition();
            comp.Tracks.Enqueue(tr);

            Driver JaccoVerstappen = new Driver("Jacco Verstappen", 0, new Car(10, 3, 10, false), TeamColors.Red);
            Driver MamaVerstappen = new Driver("Mama Verstappen", 0, new Car(10, 3, 9, false), TeamColors.Yellow);

            competition.Participants.Add(JaccoVerstappen);
            competition.Participants.Add(MamaVerstappen);

            competition.NextTrack();

            competition = comp;
            track = tr;
        }

        [Test]
        public void DrawTrack()
        {
            Track track = new Track("Spa_Section1",
                new SectionTypes[]
                {
                    SectionTypes.StartGrid, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight,
                    SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Finish
                });
            Competition competition = new Competition();
            competition.Tracks.Enqueue(track);
            competition.NextTrack();
            Visualisation.DrawTrack(track);
        }


        [Test]

        public void ReplacePlaceHolders_PlaceParticipants_IsNull()
        {
            
            Visualisation.DrawTrack(track);

            Competition comp2 = new Competition();
            comp2.Tracks.Enqueue(track);
            Data.CurrentRace = new Race(competition.NextTrack(), comp2.Participants);
            Race race = Data.CurrentRace;

            List<string> gridStrings = new List<string>();


            Direction dir = Direction.East;
            Visualisation.Initialize(race);
            Console.SetCursorPosition(Visualisation.x, Visualisation.y);
            foreach (var sect in track.Sections)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;


                switch (sect.SectionType)
                {
                    case SectionTypes.Straight:
                        if (dir == Direction.East || dir == Direction.West)
                        {
                            Visualisation.toDraw = _straightHorizontaal;
                        }
                        else Visualisation.toDraw = _straightVertical;

                        break;

                    case SectionTypes.StartGrid:
                        if (dir == Direction.East || dir == Direction.West)
                        {
                            Visualisation.toDraw = _startHorizontaal;
                        }
                        else Visualisation.toDraw = _startVerticaal;

                        break;

                    case SectionTypes.Finish:
                        if (dir == Direction.East || dir == Direction.West)
                        {
                            Visualisation.toDraw = _finishHorizontal;
                        }
                        else Visualisation.toDraw = _finishVertical;

                        break;

                    case SectionTypes.LeftCorner:
                        if (dir == Direction.East) Visualisation.toDraw = _cornerLEast;
                        if (dir == Direction.North) Visualisation.toDraw = _cornerLNorth;
                        if (dir == Direction.South) Visualisation.toDraw = _cornerLSouth;
                        if (dir == Direction.West) Visualisation.toDraw = _cornerLWest;
                        Visualisation.ChangeDirectionToLeft();
                        break;

                    case SectionTypes.RightCorner:
                        if (dir == Direction.East) Visualisation.toDraw = _cornerREast;
                        if (dir == Direction.North) Visualisation.toDraw = _cornerRNorth;
                        if (dir == Direction.South) Visualisation.toDraw = _cornerRSouth;
                        if (dir == Direction.West) Visualisation.toDraw = _cornerRWest;
                        Visualisation.ChangeDirectionToRight();
                        break;
                }

                IParticipant part1 = race.GetSectionData(sect).Left;
                IParticipant part2 = race.GetSectionData(sect).Right;
                string grid;
                for (int col = 0; col < 4; col++)
                {
                    grid = Visualisation.toDraw[col];
                    Console.SetCursorPosition(Visualisation.x, Visualisation.y);
                    
                    if (part1 != null)
                    {
                        grid = grid.Replace("1", part1.Name.Substring(0, 1));

                    }
                    else
                    {
                        grid = grid.Replace("1", " ");
                    }

                    if (part2 != null)
                    {
                        grid = grid.Replace("2", part2.Name.Substring(0, 1));
                    }
                    else
                    {
                        grid = grid.Replace("2", " ");
                    }

                    
                    gridStrings.Add(grid);
                    Visualisation.y++;
                }
                Visualisation.ChangeCursor();
            }
            

                Assert.AreEqual(track.Sections.First, gridStrings);
            }

        
        
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using Model;

namespace Controller
{

    public enum Direction
    {
        North, //0
        East, //1
        South, //2
        West //3
    }

    public static class Visualisation
    {
        public static int x = 20;
        public static int y = 20;

        public static int defaultX = 20;
        public static int defaultY = 20;

        public static string[] toDraw;
        public static Race race { get; set; } = Data.CurrentRace;
        public static Direction dir;
        public static bool side = false;

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




        public static void Initialize(Race _race)
        {
            x = defaultX;
            y = defaultY;
            race = _race;
            dir = Direction.East;
            //Console.Clear();
        }

        public static void ChangeCursor()
        {
            if (dir == Direction.North)
            {
                x += 0;
                y -= 8;
            }
            if (dir == Direction.East)
            {
                x += 4;
                y -= 4;
            }
            if (dir == Direction.South)
            {
                x += 0;
                y += 0;
            }
            if (dir == Direction.West)
            {
                x -= 4;
                y -= 4;
            }

        }
        public static void ChangeDirectionToLeft()
        {
            if (dir == Direction.North)
            {
                dir = Direction.West;
            }

            else
            {
                dir--;
            }
        }
        public static void ChangeDirectionToRight()
        {
            if (dir == Direction.West)
            {
                dir = Direction.North;
            }
            else
            {
                dir++;
            }
        }



        public static void DrawTrack(Track track)
        {
            dir = Direction.East;
            Initialize(race);
            Console.SetCursorPosition(x, y);
            foreach (var sect in track.Sections)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;


                switch (sect.SectionType)
                {
                    case SectionTypes.Straight:
                        if (dir == Direction.East || dir == Direction.West)
                        {
                            toDraw = _straightHorizontaal;
                        }
                        else toDraw = _straightVertical;

                        break;

                    case SectionTypes.StartGrid:
                        if (dir == Direction.East || dir == Direction.West)
                        {
                            toDraw = _startHorizontaal;
                        }
                        else toDraw = _startVerticaal;

                        break;

                    case SectionTypes.Finish:
                        if (dir == Direction.East || dir == Direction.West)
                        {
                            toDraw = _finishHorizontal;
                        }
                        else toDraw = _finishVertical;

                        break;

                    case SectionTypes.LeftCorner:
                        if (dir == Direction.East) toDraw = _cornerLEast;
                        if (dir == Direction.North) toDraw = _cornerLNorth;
                        if (dir == Direction.South) toDraw = _cornerLSouth;
                        if (dir == Direction.West) toDraw = _cornerLWest;
                        ChangeDirectionToLeft();
                        break;

                    case SectionTypes.RightCorner:
                        if (dir == Direction.East) toDraw = _cornerREast;
                        if (dir == Direction.North) toDraw = _cornerRNorth;
                        if (dir == Direction.South) toDraw = _cornerRSouth;
                        if (dir == Direction.West) toDraw = _cornerRWest;
                        ChangeDirectionToRight();
                        break;
                }

                for (int col = 0; col < 4; col++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(ReplacePlaceHoldersAndPlaceParticipantsOnGrid(toDraw[col], race.GetSectionData(sect).Left, race.GetSectionData(sect).Right));

                    y++;
                }
                ChangeCursor();
            }
        }

        public static string ReplacePlaceHoldersAndPlaceParticipantsOnGrid(string grid, IParticipant part1, IParticipant part2)
        {
            if (part1 != null)
            {
                if (part1.Equipment.IsBroken == true)
                {
                    grid = grid.Replace("1", "+");
                }
                else
                {
                    grid = grid.Replace("1", part1.Name.Substring(0, 1));
                }
            }
            else
            {
                grid = grid.Replace("1", " ");
            }

            if (part2 != null)
            {
                if (part2.Equipment.IsBroken == true)
                {
                    grid = grid.Replace("2", "+");
                }

                else
                {
                    grid = grid.Replace("2", part2.Name.Substring(0, 1));
                }
            }
            else
            {
                grid = grid.Replace("2", " ");
            }
            return grid;
        }


        public static void OnDriversChanged(Object obj, DriversChangedEventArgs args)
        {
            DrawTrack(args.Track);
        }
        public static void OnNextRaceEvent(object sender, NextRaceEventArgs e)
        {

            Data.Initialize();
            // link events, draw track first time
            Data.CurrentRace.DriversChanged += OnDriversChanged;
            //DrawTrack(race.Track);

        }
    }
}

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ConsoleView;
using Controller;
using Model;
using Track = Model.Track;

namespace GUI
{
    public static class Visualise
    {
        public static Bitmap canvas;
        public static Race race; //= Data.CurrentRace;
        public static event EventHandler<DriversChangedEventArgs> DriverChangedWPF;

        private static int currentX ;
        private static int currentY ;

        private static int xMax;
        private static int yMax;

        static Direction dir ;
        private static int sizeSection = Data.SectionLength;
        private static int sizeDriver = 40;//80;

        #region graphics

        private static string _cornerLEast = ".\\Pictures\\Road\\CornerLeftEast.png";
        private static string _cornerLNorth = ".\\Pictures\\Road\\CornerLeftNorth.png";
        private static string _cornerLWest = ".\\Pictures\\Road\\CornerLeftWest.png";
        private static string _cornerLSouth = ".\\Pictures\\Road\\CornerLeftSouth.png";

        private static string _cornerREast = ".\\Pictures\\Road\\CornerRightEast.png";
        private static string _cornerRSouth = ".\\Pictures\\Road\\CornerRightSouth.png";
        private static string _cornerRWest = ".\\Pictures\\Road\\CornerRightWest.png";
        private static string _cornerRNorth = ".\\Pictures\\Road\\CornerRightNorth.png";

        private static string _finishHorizontal = ".\\Pictures\\Road\\Finish.png";
        private static string _finishVertical = ".\\Pictures\\Road\\FinishVertical.png";
        private static string _startHorizontaal = ".\\Pictures\\Road\\Start.png";
        private static string _startVerticaal = ".\\Pictures\\Road\\StartVertical.png";

        private static string _straightHorizontal = ".\\Pictures\\Road\\StraightHorizontal.png";
        private static string _straightVertical = ".\\Pictures\\Road\\StraightVertical.png";

        private static string _isBroken = ".\\Pictures\\Broken.png";
        private static string _grass = ".\\Pictures\\Grass_Tile.png";
        private static string _water = ".\\Pictures\\Water.jpg";
        private static string _bus = ".\\Pictures\\Bus.png";
        #endregion
        #region teamColor
        private static string _blueEast = ".\\Pictures\\Car\\East\\bus_blue.png";
        private static string _greenEast = ".\\Pictures\\Car\\East\\bus_green.png";
        private static string _yellowEast = ".\\Pictures\\Car\\East\\bus_yellow.png";
        private static string _greyEast = ".\\Pictures\\Car\\East\\bus_grey.png";
        private static string _redEast = ".\\Pictures\\Car\\East\\bus_red.png";

        private static string _blueWest = ".\\Pictures\\Car\\West\\bus_blue.png";
        private static string _greenWest = ".\\Pictures\\Car\\West\\bus_green.png";
        private static string _yellowWest = ".\\Pictures\\Car\\West\\bus_yellow.png";
        private static string _greyWest = ".\\Pictures\\Car\\West\\bus_grey.png";
        private static string _redWest = ".\\Pictures\\Car\\West\\bus_red.png";

        private static string _blueNorth = ".\\Pictures\\Car\\North\\bus_blue.png";
        private static string _greenNorth = ".\\Pictures\\Car\\North\\bus_green.png";
        private static string _yellowNorth = ".\\Pictures\\Car\\North\\bus_yellow.png";
        private static string _greyNorth = ".\\Pictures\\Car\\North\\bus_grey.png";
        private static string _redNorth = ".\\Pictures\\Car\\North\\bus_red.png";

        private static string _blueSouth = ".\\Pictures\\Car\\South\\bus_blue.png";
        private static string _greenSouth = ".\\Pictures\\Car\\South\\bus_green.png";
        private static string _yellowSouth = ".\\Pictures\\Car\\South\\bus_yellow.png";
        private static string _greySouth = ".\\Pictures\\Car\\South\\bus_grey.png";
        private static string _redSouth = ".\\Pictures\\Car\\South\\bus_red.png";

        #endregion



        public static void Initialize(Race _race)
        {
            race = _race;
            //race.NextRace += OnNextRaceWPF;
        }


        public static BitmapSource DrawTrack(Track track)
        {
            int xBitmap;
            int yBitmap;

                currentX = 1;
            currentY = 1;
            dir = Direction.East;

            //de xMax en yMax van de bitmap moet berekend worden. hiervoor moet eerst de track in de memory uitgetekend worden.
            (xBitmap,yBitmap) = GetSizeTrack(track);
            
            //de output van de GetSizeTrack is xBitmap,yBitmap. deze worden aan de canvas meegegeven.
            canvas = VisualiseImages.CreateBitmap(xBitmap * (sizeSection * 2 + 50), yBitmap * (sizeSection * 2 + 50));
            Graphics trackGraphics = Graphics.FromImage(canvas);

            //add grass on background
            for (int row = 0; row < 1000; row+=500)
            {
                for (int col = 0; col < 1000; col+=500)
                {
                    
                    trackGraphics.DrawImage(VisualiseImages.GetImage(_grass), row, col, 500, 500);
                    /*if((row ==  700  ||row == 800 || row == 900) && col == 700) trackGraphics.DrawImage(VisualiseImages.GetImage(_water), row, col, 100, 100);
                    if((row == 700 || row == 800 || row == 900) && col == 800) trackGraphics.DrawImage(VisualiseImages.GetImage(_water), row, col, 100, 100);
                    if((row == 700 || row == 800 || row == 900) && col == 900) trackGraphics.DrawImage(VisualiseImages.GetImage(_water), row, col, 100, 100);*/
                }
            }
            /*
            for (int row = 0; row < canvas.Width; row += 100)
            {
                for (int col = 0; col < canvas.Height; col += 100)
                {
                    if (row == 700 && col == 100) trackGraphics.DrawImage(VisualiseImages.GetImage(_bus), row, col, 200, 200);
                }
            }*/

            //build a track
            AddTrack(track, trackGraphics, xBitmap, yBitmap);


            //return canvas with track
            return VisualiseImages.CreateBitmapSourceFromGdiBitmap(canvas);
            }

        public static void PlaceDrivers(Graphics graphics, Track track, int xBitmap, int yBitmap, Section sect)
        {

            /*foreach (var sect in track.Sections)
            {*/
                IParticipant leftParticipant = race.GetSectionData(sect).Left;
                IParticipant rightParticipant = race.GetSectionData(sect).Right;

                    if (leftParticipant != null)
                {
                    if (!leftParticipant.Equipment.IsBroken)
                    {
                        graphics.DrawImage(VisualiseImages.GetImage(GetDriverImage(leftParticipant, dir)), xBitmap, yBitmap, sizeDriver, sizeDriver);
                    ChangePosition(ref xBitmap, ref yBitmap);
                }
                    else graphics.DrawImage(VisualiseImages.GetImage(_isBroken),xBitmap,yBitmap,sizeDriver-30,sizeDriver-30);
                }
                if (rightParticipant!= null)
                {
                    if (!rightParticipant.Equipment.IsBroken)
                    {
                        graphics.DrawImage(VisualiseImages.GetImage(GetDriverImage(rightParticipant, dir)), xBitmap, yBitmap-20, sizeDriver, sizeDriver);
                        ChangePosition(ref xBitmap,ref yBitmap);
                    }
                    else graphics.DrawImage(VisualiseImages.GetImage(_isBroken), xBitmap, yBitmap, sizeDriver-10, sizeDriver-10);
                }
            //}
        }

        public static string GetDriverImage(IParticipant participant, Direction dir)
        {
            string output = null;
            switch (dir)
            {
                case Direction.East:
                    switch (participant.TeamColor)
                    {
                        case TeamColors.Blue:
                            output = _blueEast;
                            break;
                        case TeamColors.Green:
                            output = _greenEast;
                            break;
                        case TeamColors.Grey:
                            output = _greyEast;
                            break;
                        case TeamColors.Red:
                            output = _redEast;
                            break;
                        case TeamColors.Yellow:
                            output = _yellowEast;
                            break;
                    }
                    break;
                case Direction.West:
                    switch (participant.TeamColor)
                    {
                        case TeamColors.Blue:
                            output = _blueWest;
                            break;
                        case TeamColors.Green:
                            output = _greenWest;
                            break;
                        case TeamColors.Grey:
                            output = _greyWest;
                            break;
                        case TeamColors.Red:
                            output = _redWest;
                            break;
                        case TeamColors.Yellow:
                            output = _yellowWest;
                            break;
                    }
                    break;
                case Direction.North:
                    switch (participant.TeamColor)
                    {
                        case TeamColors.Blue:
                            output = _blueNorth;
                            break;
                        case TeamColors.Green:
                            output = _greenNorth;
                            break;
                        case TeamColors.Grey:
                            output = _greyNorth;
                            break;
                        case TeamColors.Red:
                            output = _redNorth;
                            break;
                        case TeamColors.Yellow:
                            output = _yellowNorth;
                            break;
                    }
                    break;
                case Direction.South:
                    switch (participant.TeamColor)
                    {
                        case TeamColors.Blue:
                            output = _blueSouth;
                            break;
                        case TeamColors.Green:
                            output = _greenSouth;
                            break;
                        case TeamColors.Grey:
                            output = _greySouth;
                            break;
                        case TeamColors.Red:
                            output = _redSouth;
                            break;
                        case TeamColors.Yellow:
                            output = _yellowSouth;
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("no color");
            }

            return output;
        }
        public static void AddTrack(Track track, Graphics graphics, int xBitmap, int yBitmap)
        {
            if (Data.CurrentRace != null)
            {
                xBitmap *= sizeSection * 2;
                xBitmap /= 2;
                yBitmap *= sizeSection + 30 ;
                foreach (var sect in track.Sections)
                {
                    switch (sect.SectionType)
                    {
                        case SectionTypes.Straight:
                            if (dir == Direction.East || dir == Direction.West)
                                graphics.DrawImage(VisualiseImages.GetImage(_straightHorizontal), xBitmap, yBitmap, sizeSection, sizeSection);
                            else
                                graphics.DrawImage(VisualiseImages.GetImage(_straightVertical), xBitmap, yBitmap, sizeSection, sizeSection);
                            break;

                        case SectionTypes.StartGrid:
                            if (dir == Direction.East || dir == Direction.West)
                                graphics.DrawImage(VisualiseImages.GetImage(_startHorizontaal), xBitmap, yBitmap, sizeSection, sizeSection);
                            else graphics.DrawImage(VisualiseImages.GetImage(_startVerticaal), xBitmap, yBitmap, sizeSection, sizeSection);
                            break;

                        case SectionTypes.Finish:
                            if (dir == Direction.East || dir == Direction.West)
                                graphics.DrawImage(VisualiseImages.GetImage(_finishHorizontal), xBitmap, yBitmap, sizeSection, sizeSection);
                            else graphics.DrawImage(VisualiseImages.GetImage(_finishVertical), xBitmap, yBitmap, sizeSection, sizeSection);
                            break;

                        case SectionTypes.LeftCorner:
                            if (dir == Direction.East) graphics.DrawImage(VisualiseImages.GetImage(_cornerLEast), xBitmap, yBitmap, sizeSection, sizeSection); 
                            
                            if (dir == Direction.North) graphics.DrawImage(VisualiseImages.GetImage(_cornerLNorth), xBitmap, yBitmap, sizeSection, sizeSection);  
                            if (dir == Direction.South) graphics.DrawImage(VisualiseImages.GetImage(_cornerLSouth), xBitmap, yBitmap, sizeSection, sizeSection);  
                            if (dir == Direction.West) graphics.DrawImage(VisualiseImages.GetImage(_cornerLWest), xBitmap, yBitmap, sizeSection, sizeSection);  
                            ChangeDirectionToLeft();
                            break;

                        case SectionTypes.RightCorner:
                            if (dir == Direction.East) graphics.DrawImage(VisualiseImages.GetImage(_cornerREast), xBitmap, yBitmap, sizeSection, sizeSection);  
                            if (dir == Direction.North) graphics.DrawImage(VisualiseImages.GetImage(_cornerRNorth), xBitmap, yBitmap, sizeSection, sizeSection);  
                            if (dir == Direction.South) graphics.DrawImage(VisualiseImages.GetImage(_cornerRSouth), xBitmap, yBitmap, sizeSection, sizeSection);  
                            if (dir == Direction.West) graphics.DrawImage(VisualiseImages.GetImage(_cornerRWest), xBitmap, yBitmap, sizeSection, sizeSection);  
                            ChangeDirectionToRight();
                            break;
                    }
                    //place the drivers on track
                    PlaceDrivers(graphics, track, xBitmap, yBitmap, sect);
                    ChangePosition(ref xBitmap,ref yBitmap);
                }
            }
        }
        public static (int xBitmap,int yBitmap) GetSizeTrack(Track track)
        {
            foreach (Section currentS in track.Sections)
            {
                switch (currentS.SectionType)
                {
                    case SectionTypes.StartGrid:
                    case SectionTypes.Finish:
                    case SectionTypes.Straight:
                        ChangeCursor();
                        break;
                    case SectionTypes.LeftCorner:
                        ChangeDirectionToLeft();
                        ChangeCursor();
                        break;
                    case SectionTypes.RightCorner:
                        ChangeDirectionToRight();
                        ChangeCursor();
                        break;
                }
            }

            int xBitmap = xMax + 1;
            int yBitmap = yMax + 1;
            return (xBitmap,yBitmap);
        }

        /*public static void OnNextRaceWPF(Object sender, EventArgs eventArgs)
        {
            DriverChangedWPF += OnDriversChangedWPF;
        }

        public static void OnDriversChangedWPF(Object sender, DriversChangedEventArgs eventArgs)
        {
            DrawTrack(eventArgs.Track);
        }*/

        public static void ChangeCursor()
        {
            switch(dir)
            {
                case Direction.North:
                    currentY--;
                    break;
                case Direction.East:
                    currentX++;
                    break;
                case Direction.South:
                    currentY++;
                    break;
                case Direction.West:
                    currentX--;
                    break;
            }

            if (currentY > xMax) xMax = currentX;
            if (currentX > yMax) yMax = currentY;
        }
        public static void ChangePosition(ref int x, ref int y)
        {
            switch (dir)
            {
                case Direction.North:
                    y-=sizeSection;
                    break;
                case Direction.East:
                    x += sizeSection;
                    break;
                case Direction.South:
                    y+= sizeSection;
                    break;
                case Direction.West:
                    x -= sizeSection;
                    break;
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

    }
}

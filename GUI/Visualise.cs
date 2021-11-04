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
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Controller;
using Model;
using Track = Model.Track;

namespace GUI
{
    public static class Visualise
    {
        public static Bitmap canvas;

        private static int currentX ;
        private static int currentY ;

        private static int xMax;
        private static int yMax;

        private static int xBitmap;
        private static int yBitmap;

        static Direction dir ;
        private static int sizeSection = Data.SectionLength;
        private static int sizeDriver = 80;

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
        private static string _startHorizontaal = ".\\Pictures\\Road\\Start.png";

        private static string _straightHorizontal = ".\\Pictures\\Road\\StraightHorizontal.png";
        private static string _straightVertical = ".\\Pictures\\Road\\StraightVertical.png";

        private static string _isBroken = ".\\Pictures\\broken.png";

        private static string _blue = ".\\Pictures\\Car\\Blue.png";
        private static string _green = ".\\Pictures\\Car\\Green.png";
        private static string _yellow = ".\\Pictures\\Car\\Yellow.png";
        private static string _grey = ".\\Pictures\\Car\\Grey.png";
        private static string _red = ".\\Pictures\\Car\\Red.png";

        private static string _grass = ".\\Pictures\\Grass_Tile.png";
        #endregion

        public static void Initialize()
        {
            currentX = 1;
            currentY = 1;

        }
        
        public static BitmapSource DrawTrack(Track track)
        {
            
            Initialize();
            dir = Direction.East;

            //de xMax en yMax van de bitmap moet berekend worden. hiervoor moet eerst de track in de memory uitgetekend worden.
            GetSizeTrack(track);
            
            //de output van de GetSizeTrack is xBitmap,yBitmap. deze worden aan de canvas meegegeven.
            canvas = VisualiseImages.CreateBitmap(xBitmap * (sizeSection * 2 + 54), yBitmap * (sizeSection * 2 + 54));
            Graphics trackGraphics = Graphics.FromImage(canvas);

            //add grass on background
            for (int row = 0; row < canvas.Width; row+=100)
            {
                for (int col = 0; col < canvas.Height; col+=100)
                {
                    trackGraphics.DrawImage(VisualiseImages.GetImage(_grass), row, col, 100, 100);
                }
            }

            //build a track
            AddTrack(track, trackGraphics);

            //place the drivers on the track
            //PlaceDrivers(track,trackGraphics);

            //return canvas with track
            return VisualiseImages.CreateBitmapSourceFromGdiBitmap(canvas);
            }

        public static void AddTrack(Track track, Graphics graphics)
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
                                graphics.DrawImage(VisualiseImages.GetImage(_startHorizontaal), xBitmap, yBitmap, sizeSection, sizeSection); ;
                            break;

                        case SectionTypes.Finish:
                            if (dir == Direction.East || dir == Direction.West)
                                graphics.DrawImage(VisualiseImages.GetImage(_finishHorizontal), xBitmap, yBitmap, sizeSection, sizeSection);
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
                    ChangePosition();
                }
            }
        }
        public static void GetSizeTrack(Track track)
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

            xBitmap = xMax + 1;
            yBitmap = yMax + 1;
        }

        public static void PlaceDrivers(Track track, Graphics graphics, IParticipant participant)
        {

        }

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
        public static void ChangePosition()
        {
            switch (dir)
            {
                case Direction.North:
                    yBitmap-=sizeSection;
                    break;
                case Direction.East:
                    xBitmap += sizeSection;
                    break;
                case Direction.South:
                    yBitmap+= sizeSection;
                    break;
                case Direction.West:
                    xBitmap -= sizeSection;
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

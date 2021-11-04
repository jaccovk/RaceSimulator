using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace GUI
{
    public static class VisualiseImages
    {
        //string(url afbeelding) omzetten naar afbeelding

        private static Dictionary<string, Bitmap> Cache;


        public static void InitializeCache()
        {
            Cache = new Dictionary<string, Bitmap>();
            CacheClear();
        }

        public static Bitmap GetImage(string url)
        {
            if (!Cache.ContainsKey(url))
            {
                Cache.Add(url, new Bitmap(url));
                return Cache[url];
            }
            else return Cache[url];
        }

        public static Bitmap CreateBitmap(int x, int y)
        {
            string empty = "empty";
            if (!Cache.ContainsKey(empty))
            {
                //voeg een nieuwe bitmap toe
                Cache.Add(empty,new Bitmap(x,y));
                //geef de background een kleur
                Graphics g = Graphics.FromImage(Cache[empty]);
                g.Clear(Color.DarkGreen);
            }
            return (Bitmap)Cache["empty"].Clone();
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        public static void CacheClear()
        {
            Cache.Clear();
        }
    }


}

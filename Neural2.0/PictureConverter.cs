using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural2._0
{
    public class PictureConverter
    {
        public int level { get; set; } = 128;
        public int Height { get; set; }
        public int Width { get; set; }
        public List<int> Convert(string path)
        {
            var res = new List<int>();
            var image = new Bitmap(path);
            Height = image.Height;
            Width = image.Width;
            var size = image.Width * image.Height;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    res.Add(Brightness(pixel));
                }
            }
            return res;
        }
        private int Brightness(Color pixel)
        {

            /* var res = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;*/
            return pixel.R == 255 && pixel.G == 255 && pixel.G == 255 ? 0 : 1;
        }
        public void Save(string path, List<int> pixels)
        {
            var image = new Bitmap(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var color = pixels[y * Width + x] == 1 ? Color.White : Color.Black;
                    image.SetPixel(x, y, color);
                }
            }
            image.Save(path);
        }
    }
}

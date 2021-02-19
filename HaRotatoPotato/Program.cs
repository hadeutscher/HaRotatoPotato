using ImageMagick;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace HaRotatoPotato
{
    class Program
    {

        private Bitmap RotateImage(Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                // Rotate
                g.RotateTransform(angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }
        static void Main(string[] args)
        {
            var total_time = int.Parse(args[2]);
            var fps = 30;
            var total_frames = total_time * fps;
            var angle_delta = 360.0f / total_frames;
            string base_path = args[1];
            Program p = new Program();
            using (Bitmap b = (Bitmap)Image.FromFile(args[0]))
            {
                for (int i = 0; i < total_frames; i++)
                {
                    p.RotateImage(b, i * angle_delta).Save(Path.Combine(base_path, $"{i}.png"));
                }
            }

            using (MagickImageCollection collection = new MagickImageCollection())
            {
                for (int i = 0; i < total_frames; i++)
                {
                    collection.Add(Path.Combine(base_path, $"{i}.png"));
                    collection[i].AnimationDelay = (int)Math.Round(100.0 / fps);
                }

                // Optionally reduce colors
                QuantizeSettings settings = new QuantizeSettings();
                settings.Colors = 256;
                collection.Quantize(settings);

                // Optionally optimize the images (images should have the same size).
                collection.Optimize();

                // Save gif
                collection.Write(Path.Combine(base_path, $"out.gif"));
            }
        }
    }
}

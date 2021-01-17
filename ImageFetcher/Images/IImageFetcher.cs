using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageFetcher.Images
{
    /// <summary>
    /// Interface for getting and transforming images.
    /// </summary>
    public interface IImageFetcher
    {
        /// <summary>
        /// Gets a byte array of an image based on criteria specified.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        byte[] Fetch(ImageCriteria criteria);
    }
    
    public enum ImageFormat { Png, Jpg, Gif };

    public static class ImageFormatExtensions
    {
        public static string MimeType(this ImageFormat format)
        {
            if (format == ImageFormat.Png)
            {
                return "image/png";
            }

            if (format == ImageFormat.Jpg)
            {
                return "image/jpeg";
            }

            if (format == ImageFormat.Gif)
            {
                return "image/gif";
            }

            throw new ArgumentException("Unknown mime type for format [" + format + "]");
        }
    }

    public class ImageCriteria
    {
        public readonly string File;

        public readonly int Width;
        public readonly int Height;

        public readonly string Watermark;

        public readonly Tuple<int, int, int> RGBBackgroundColour;

        public readonly ImageFormat Format;
        
        private ImageCriteria(string file, int width, int height, ImageFormat format, string watermark, Tuple<int, int, int> rgbackgroundColour) {

            File = file;
            Width = width;
            Height = height;
            Watermark = watermark;
            RGBBackgroundColour = rgbackgroundColour;
            Format = format;
        }

        public bool ShouldAddWatermark()
        {
            return !string.IsNullOrEmpty(Watermark);
        }

        public bool ShouldAddBackgroundColour()
        {
            return RGBBackgroundColour != null;
        }

        public static Builder NewBuilder()
        {
            return new Builder();
        }

        public string ToHash()
        {
            var rgb = "";
            if (RGBBackgroundColour != null)
            {
                rgb = RGBBackgroundColour.Item1 + "," + RGBBackgroundColour.Item2 + "," + RGBBackgroundColour.Item3;
            }
            
            return File + Width + "x" + Height + Watermark + rgb + Format;
        }

        public class Builder
        {
            private int _width;
            private int _height;
            private string _watermark;
            private Tuple<int, int, int> _backgroundColour;
            private ImageFormat _format = ImageFormat.Png;

            public ImageCriteria Build(string file)
            {
                return new ImageCriteria(file, _width, _height, _format, _watermark, _backgroundColour);
            }

            public Builder SetSize(int w, int h)
            {
                if (w > 2500 || h > 2500)
                {
                    throw new ArgumentException("Maximum width and height should be less than 2500");
                }

                if (w < 0 || h < 0)
                {
                    throw new ArgumentException("Width and height should be more than 0");
                }

                _width = w;
                _height = h;
                return this;
            }

            public Builder SetWatermark(string watermark)
            {
                _watermark = watermark;
                return this;
            }

            public Builder SetBackgroundColour(string backgroundColour)
            {
                var values = backgroundColour.Split(',');

                if (values.Length != 3)
                {
                    throw new ArgumentException("Invalid background colour [" + backgroundColour + "] requires R,G,B format");
                }

                var r = ParseColour(values[0]);
                var g = ParseColour(values[1]);
                var b = ParseColour(values[2]);

                _backgroundColour = new Tuple<int, int, int>(r, g, b);
                return this;
            }

            private int ParseColour(string val)
            {
                int colour;
                if (!int.TryParse(val, out colour))
                {
                    throw new ArgumentException("Invalid background colour [" + val + "]");
                }
                if (colour < 0 || colour > 255)
                {
                    throw new ArgumentException("Invalid background colour [" + val + "]");
                }

                return colour;
            }

            public Builder SetImageFormat(ImageFormat imageFormat)
            {
                _format = imageFormat;
                return this;
            }
        }
    }
}

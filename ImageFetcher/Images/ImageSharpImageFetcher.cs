using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.IO;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageFetcher.Images
{
    /// <summary>
    /// Uses the image sharp library for processing images.
    /// </summary>
    public class ImageSharpImageFetcher : IImageFetcher
    {
        private readonly string _imageDirectory;

        private readonly Font _font;
        
        public ImageSharpImageFetcher(string imageDirectory, string fontFile)
        {
            _imageDirectory = imageDirectory;

            FontCollection fonts = new FontCollection();
            FontFamily fontfamily = fonts.Install(fontFile);
            _font = new Font(fontfamily, 10, FontStyle.Bold);
        }

        public byte[] Fetch(ImageCriteria criteria)
        {
            //check if the image is there
            var file = Path.Combine(_imageDirectory, criteria.File);

            if (!File.Exists(file))
            {
                throw new ArgumentException("Invalid file provided [" + criteria.File + "]");
            }

            var imageFormat = GetFormat(criteria.Format);

            using (Image image = Image.Load(file))
            {
                image.Mutate(context =>
                {
                    if (criteria.Width != 0 && criteria.Height != 0)
                    {
                        context = context.Resize(criteria.Width, criteria.Height);
                    }                    

                    if (criteria.ShouldAddWatermark())
                    {
                        context = ApplyWaterMark(context, criteria.Watermark);
                    }

                    if (criteria.ShouldAddBackgroundColour())
                    {
                        context = ApplyBackgroundColor(context, criteria.RGBBackgroundColour);
                    }                    
                });

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, imageFormat);
                    return ms.ToArray();
                }
            }
        }

        private IImageProcessingContext ApplyWaterMark(IImageProcessingContext context,
            string text)
        {
            return context.DrawText(text, _font, Color.WhiteSmoke,
                new PointF(10, 10));
        }

        private IImageProcessingContext ApplyBackgroundColor(IImageProcessingContext context, Tuple<int, int, int> bgColour)
        {
            //Get the size required for drawing the file
            return context.BackgroundColor(new Rgba32(bgColour.Item1, bgColour.Item2, bgColour.Item3));
        }

        private IImageEncoder GetFormat(ImageFormat type)
        {
            if (type == ImageFormat.Jpg)
            {
                return new JpegEncoder();
            } else if (type == ImageFormat.Png)
            {
                return new PngEncoder();
            } else if (type == ImageFormat.Gif)
            {
                return new GifEncoder();
            }

            throw new ArgumentException("File type not supported [" + type + "]");
        }        
    }
}

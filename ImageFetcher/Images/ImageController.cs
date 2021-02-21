using Microsoft.AspNetCore.Mvc;
using ImageFetcher.Images;
using System;
using Microsoft.Extensions.Logging;

namespace ImageFetcher.Controllers
{
    [Route("api/images")]
    public class ImageController : Controller
    {
        private readonly IImageFetcher _imageFetcher;
        private readonly ILogger _logger;

        public ImageController(IImageFetcher fetcher, ILogger<ImageController> logger)
        {
            _imageFetcher = fetcher;
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult Get(string file, int width, int height, string backgroundColour = null, 
            string watermark = null, string fileType = "png")
        {
            try
            {
                var criteriaBuilder = ImageCriteria.NewBuilder()
                                .SetSize(width, height)
                                .SetImageFormat(ParseFormat(fileType));

                if (backgroundColour != null)
                {
                    criteriaBuilder.SetBackgroundColour(backgroundColour);
                }

                if (watermark != null)
                {
                    criteriaBuilder.SetWatermark(watermark);
                }

                var criteria = criteriaBuilder.Build(file);

                var imageBytes = _imageFetcher.Fetch(criteria);

                return File(imageBytes, criteria.Format.MimeType());

            } catch (ArgumentException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            } catch (Exception ex)
            {
                _logger.LogError("Failed to fetch image " + ex.Message);
                return StatusCode(500);
            }
        }

        private ImageFormat ParseFormat(string ft)
        {
            return Enum.Parse<ImageFormat>(ft, true);
        }
    }
}

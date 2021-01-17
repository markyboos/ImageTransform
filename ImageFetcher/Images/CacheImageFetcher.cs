using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageFetcher.Images
{
    /// <summary>
    /// Uses a memory cache to cache images fetched from a base source.
    /// </summary>
    public class CacheImageFetcher : IImageFetcher
    {
        private MemoryCache _cache;
        private IImageFetcher _source;

        public CacheImageFetcher(IImageFetcher source, long maxCacheSizeMb)
        {
            _source = source;
            _cache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = maxCacheSizeMb * 1000000
            });
        }

        public byte[] Fetch(ImageCriteria criteria)
        {
            var value = _cache.Get<byte[]>(criteria.ToHash());

            if (value == null)
            {
                var image = _source.Fetch(criteria);
                _cache.Set(criteria.ToHash(), image, new MemoryCacheEntryOptions()
                {
                    Size = image.Length
                });

                return image;
            }

            return value;
        }
    }
}

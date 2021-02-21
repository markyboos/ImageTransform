using ImageFetcher.Images;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImageFetcher
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(x => { x.EnableEndpointRouting = false; });

            var imageDirectory = Configuration.GetValue<string>("Configuration:ImageDirectory");
            var fontFile = Configuration.GetValue<string>("Configuration:FontFile");
            
            var imageFetcher = new ImageSharpImageFetcher(imageDirectory, fontFile);

            long maxCacheSizeMb = Configuration.GetValue<long>("Configuration:MaxImageCacheMemoryMb");

            services.AddSingleton<IImageFetcher>(new CacheImageFetcher(imageFetcher, maxCacheSizeMb));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMvc();
        }
    }
}
